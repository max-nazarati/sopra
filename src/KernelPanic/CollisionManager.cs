using KernelPanic.Entities;
using KernelPanic.Entities.Buildings;
using KernelPanic.Entities.Projectiles;
using KernelPanic.Entities.Units;

namespace KernelPanic
{
    internal static class CollisionManager
    {
        #region Movement Collisions

        internal static bool CollidesWith(this Troupe troupe, Troupe other)
        {
            var bird1 = troupe is Thunderbird;
            var bird2 = other is Thunderbird;

            if (bird1 || bird2)
            {
                // If at least one is a bird they'll collide if both are birds.
                return bird1 && bird2;
            }
            
            // If both are not small they'll collide.
            if (!troupe.IsSmall && !other.IsSmall)
                return true;

            // If one is small they won't collide.
            if (!troupe.IsSmall || !other.IsSmall)
                return false;

            // Otherwise we have to small troupes which will collide if they have the same type.
            return troupe.GetType() == other.GetType();
        }

        #endregion

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

                case Projectile projectile when object2 is LaneBorder border:
                    if (border.IsOutside)
                        projectile.RadiusReached();
                    return true;

                case Unit unit when object2 is LaneBorder border:
                    if (border.IsTargetBorder)
                        unit.DamageBase(positionProvider);
                    return true;

                case Emp emp when object2 is Tower tower:
                    emp.Hit(tower);
                    return true;
                
                default:
                    return false;
            }
        }
    }
}
