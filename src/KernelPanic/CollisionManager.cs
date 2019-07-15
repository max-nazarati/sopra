using System;
using KernelPanic.Entities;
using KernelPanic.Entities.Buildings;
using KernelPanic.Entities.Projectiles;
using KernelPanic.Entities.Units;

namespace KernelPanic
{
    internal static class CollisionManager
    {
        private static readonly Random sRandom = new Random();

        internal static bool HandleMovement(IGameObject object1, IGameObject object2)
        {
            if (!(object1 is Troupe a) || !(object2 is Troupe b))
                return false;

            if (!(a is Thunderbird ^ b is Thunderbird))
                return false;

            if (sRandom.Next(0, 1) == 0)
                a.ResetMovement();
            else
                b.ResetMovement();

            return true;
        }

        internal static void Handle(IGameObject object1, IGameObject object2, PositionProvider positionProvider)
        {
            if (!TryHandle(object1, object2, positionProvider))
                TryHandle(object2, object1, positionProvider);
        }

        private static bool TryHandle(IGameObject object1, IGameObject object2, PositionProvider positionProvider)
        {
            switch (object1)
            {
                case Projectile projectile when object2 is Unit unit:
                    projectile.Hit(unit, positionProvider);
                    return true;

                case Projectile projectile when object2 is LaneBorder:
                    projectile.RadiusReached();
                    return true;
                
                case Emp emp when object2 is Tower tower:
                    emp.Hit(tower);
                    return true;
                    
                case Troupe a when object2 is Troupe b:
                    if (sRandom.Next(0, 1) == 0)
                        a.ResetMovement();
                    else
                        b.ResetMovement();
                    return true;
                
                default:
                    return false;
            }

        }
    }
}
