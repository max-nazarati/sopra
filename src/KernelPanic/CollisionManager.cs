using System;
using System.Linq;
using KernelPanic.Data;
using KernelPanic.Entities;
using KernelPanic.Entities.Buildings;
using KernelPanic.Entities.Projectiles;
using KernelPanic.Entities.Units;
using KernelPanic.Table;

namespace KernelPanic
{
    internal static class CollisionManager
    {
        private static readonly Random sRandom = new Random();

        internal static bool HandleMovement(IGameObject object1, IGameObject object2, PositionProvider positionProvider)
        {
            if (!(object1 is Troupe a) || !(object2 is Troupe b))
                return false;

            if (a is Thunderbird ^ b is Thunderbird)
                return false;

            return ChooseTroupeToReset2(a, b, positionProvider).ResetMovement();
        }

        private static Troupe ChooseTroupeToReset2(Troupe a, Troupe b, PositionProvider positionProvider)
        {
            var preferred = DecideByTileHeat(a, b, positionProvider) ?? DecideByMovement(a, b, positionProvider);
            if (preferred != null)
                return preferred;

            Console.WriteLine("Using random :/");
            return sRandom.Next(0, 1) == 0 ? a : b;
        }

        private static Troupe DecideByTileHeat(Troupe a, Troupe b, PositionProvider positionProvider)
        {
            if (!(positionProvider.Grid.TileFromWorldPoint(a.Sprite.Position) is TileIndex tileA))
                return null;
        
            if (!(positionProvider.Grid.TileFromWorldPoint(b.Sprite.Position) is TileIndex tileB))
                return null;

            var pointA = tileA.Rescaled(1).First().ToPoint();
            var pointB = tileB.Rescaled(1).First().ToPoint();

            if (pointA == pointB)
                return null;

            var heatA = positionProvider.TroupeData.TileHeat(pointA);
            var heatB = positionProvider.TroupeData.TileHeat(pointB);

            if (heatA == heatB)
                return null;

            Console.WriteLine("Successful with heat.");
            return heatA < heatB ? a : b;
        }

        private static Troupe DecideByMovement(Troupe a, Troupe b, PositionProvider positionProvider)
        {
            if (!(positionProvider.Grid.TileFromWorldPoint(a.Sprite.Position) is TileIndex tileA))
                return null;
        
            if (!(positionProvider.Grid.TileFromWorldPoint(b.Sprite.Position) is TileIndex tileB))
                return null;

            var lastMovementA = a.LastMovement;
            if (Math.Abs(lastMovementA.LengthSquared()) < 0.001)
                return a;

            var lastMovementB = b.LastMovement;
            if (Math.Abs(lastMovementB.LengthSquared()) < 0.001)
                return b;

            var nextMovementA = positionProvider.TroupeData.RelativeMovement(a);
            var nextMovementB = positionProvider.TroupeData.RelativeMovement(b);

            var angleA = Geometry.Angle(nextMovementA, lastMovementA);
            var angleB = Geometry.Angle(nextMovementB, lastMovementB);
            if (Math.Abs(angleA - angleB) < 0.001)
                return null;

            Console.WriteLine("Successful with angles.");
            return angleA < angleB ? a : b;
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
                
                default:
                    return false;
            }

        }
    }
}
