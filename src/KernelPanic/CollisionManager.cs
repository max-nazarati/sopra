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
    
        internal static bool HandleMovement(IGameObject object1, IGameObject object2, PositionProvider positionProvider)
        {
            if (!(object1 is Troupe a) || !(object2 is Troupe b) || !a.CollidesWith(b))
                return false;

            var toReset = TroupeToReset(a, b, positionProvider);
            return toReset != null && toReset.ResetMovement();
        }

        private static Troupe TroupeToReset(Troupe a, Troupe b, PositionProvider positionProvider)
        {
            // If one unit was already reset, we reset the other.
            if (a.MovementResettable && !b.MovementResettable)
                return a;
            if (b.MovementResettable && !a.MovementResettable)
                return b;

            if (!a.MovementResettable && !b.MovementResettable)
            {
                // Shouldn't they already have collided in the last frame?
                return null;
            }

            if (!(a.MoveTarget is Vector2 aMoveTarget) || !(b.MoveTarget is Vector2 bMoveTarget))
                return null;

            var subTiles = a.IsSmall ? 2 : 1;

            if (!(positionProvider.Grid.TileFromWorldPoint(aMoveTarget, subTiles) is TileIndex tileA))
                return null;
        
            if (!(positionProvider.Grid.TileFromWorldPoint(bMoveTarget, subTiles) is TileIndex tileB))
                return null;

            if (tileA == tileB)
            {
                var aDistance = Vector2.DistanceSquared(a.Sprite.Position, aMoveTarget);
                var bDistance = Vector2.DistanceSquared(b.Sprite.Position, bMoveTarget);
                
                return aDistance < bDistance ? b : a;
            }

            if (a is Thunderbird)
            {
                return ResetByTiles(a, b, tileA, tileB, positionProvider);
            }

            var heatA = positionProvider.TroupeData.TileHeat(tileA.BaseTile.ToPoint(), a);
            var heatB = positionProvider.TroupeData.TileHeat(tileB.BaseTile.ToPoint(), b);

            if (heatA == heatB)
            {
                return ResetByTiles(a, b, tileA, tileB, positionProvider);
            }

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

        private static Troupe ResetByTiles(Troupe a, Troupe b, TileIndex targetA, TileIndex targetB, PositionProvider positionProvider)
        {
            var subTiles = a.IsSmall ? 2 : 1;
            var currentA = positionProvider.RequireTile(a.Sprite.Position, subTiles);
            var currentB = positionProvider.RequireTile(b.Sprite.Position, subTiles);

            if (currentA == targetB)
            {
                // ›B‹ has to go where ›A‹ is. Reset ›B‹.
                return b;
            }

            if (currentB == targetA)
            {
                // ›A‹ has to go where ›B‹ is. Reset ›A‹.
                return a;
            }

            return null;
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
