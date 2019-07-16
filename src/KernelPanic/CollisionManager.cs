using System.Linq;
using KernelPanic.Entities;
using KernelPanic.Entities.Buildings;
using KernelPanic.Entities.Projectiles;
using KernelPanic.Entities.Units;
using KernelPanic.Table;
using Microsoft.Xna.Framework;

namespace KernelPanic
{
    internal static class CollisionManager
    {
        internal static bool HandleMovement(IGameObject object1, IGameObject object2, PositionProvider positionProvider)
        {
            if (!(object1 is Troupe a) || !(object2 is Troupe b))
                return false;

            if (a is Thunderbird ^ b is Thunderbird)
                return false;

            if (a.IsSmall && b.IsSmall && a.GetType() != b.GetType())
                return false;

            var toReset = TroupeToReset(a, b, positionProvider);
            return toReset != null && toReset.ResetMovement();
        }

        private static Troupe TroupeToReset(Troupe a, Troupe b, PositionProvider positionProvider)
        {
            if (!(a.MoveTarget is Vector2 aMoveTarget) || !(b.MoveTarget is Vector2 bMoveTarget))
                return null;

            if (!(positionProvider.Grid.TileFromWorldPoint(aMoveTarget) is TileIndex tileA))
                return null;
        
            if (!(positionProvider.Grid.TileFromWorldPoint(bMoveTarget) is TileIndex tileB))
                return null;

            var pointA = tileA.Rescaled(1).First().ToPoint();
            var pointB = tileB.Rescaled(1).First().ToPoint();

            if (pointA == pointB)
            {
                var aDistance = Vector2.DistanceSquared(a.Sprite.Position, aMoveTarget);
                var bDistance = Vector2.DistanceSquared(b.Sprite.Position, bMoveTarget);
                
                return aDistance < bDistance ? b : a;
            }

            var heatA = positionProvider.TroupeData.TileHeat(pointA);
            var heatB = positionProvider.TroupeData.TileHeat(pointB);

            if (heatA == heatB)
                return null;

            if (heatA < heatB)
            {
                var aDistance = Vector2.DistanceSquared(a.Sprite.Position, aMoveTarget);
                var bDistance = Vector2.DistanceSquared(b.Sprite.Position, bMoveTarget) + Vector2.DistanceSquared(bMoveTarget, aMoveTarget);

                return aDistance < bDistance ? b : a;
            }
            else
            {
                var aDistance = Vector2.DistanceSquared(a.Sprite.Position, aMoveTarget) + Vector2.DistanceSquared(aMoveTarget, bMoveTarget);
                var bDistance = Vector2.DistanceSquared(b.Sprite.Position, bMoveTarget);

                return aDistance < bDistance ? b : a;
            }
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
