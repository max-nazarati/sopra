﻿using KernelPanic.Entities;
using System.Diagnostics.CodeAnalysis;
using KernelPanic.Entities;
using KernelPanic.Entities.Buildings;
using KernelPanic.Entities.Projectiles;

namespace KernelPanic
{
    internal static class CollisionManager
    {
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
                    // emp.Hit(tower, positionProvider);
                    return true;
                
                default:
                    return false;
            }

        }
    }
}
