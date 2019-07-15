using System;
using System.Collections.Generic;
using System.Linq;
using KernelPanic.Data;
using KernelPanic.Entities;
using KernelPanic.Entities.Buildings;
using KernelPanic.Entities.Projectiles;
using KernelPanic.PathPlanning;
using KernelPanic.Table;
using Microsoft.Xna.Framework;

namespace KernelPanic
{
    internal sealed class PositionProvider
    {
        private readonly SpriteManager mSpriteManager;
        private readonly EntityGraph mEntities;
        private readonly VectorField mVectorField;
        private readonly VectorField mVectorFieldThunderbird;

        internal Grid Grid { get; }
        internal Owner Owner { get; }
        internal Base Target { get; }

        internal PositionProvider(Grid grid, EntityGraph entities, SpriteManager spriteManager, VectorField vectorField, Base target, Owner owner)
        {
            Grid = grid;
            mEntities = entities;
            mSpriteManager = spriteManager;
            mVectorField = vectorField;
            mVectorFieldThunderbird = VectorField.GetVectorFieldThunderbird(vectorField, grid.LaneSide);
            Target = target;
            Owner = owner;
        }

        #region Position Calculations

        internal Rectangle TileBounds(Point tile)
        {
            var (position, size) = Grid.GetTile(new TileIndex(tile, 1));
            return Bounds.ContainingRectangle(position, new Vector2(size));
        }

        internal TileIndex RequireTile(Entity entity)
        {
            return RequireTile(entity.Sprite.Position);
        }

        internal TileIndex RequireTile(Vector2 position)
        {
            if (Grid.TileFromWorldPoint(position) is TileIndex tile)
                return tile;

            throw new InvalidOperationException(
                $"Required a tile for {position} but it is not inside the lane {Grid.Bounds}");
        }

        #endregion

        #region Querying Entities

        internal IEnumerable<T> NearEntities<T>(Entity entity, float radius) where T : Entity
        {
            return mEntities.QuadTree
                .NearEntities(entity.Bounds.Center.ToVector2(), radius)
                .OfType<T>();
        }

        internal IEnumerable<T> NearEntities<T>(Vector2 position, float radius) where T : Entity
        {
            return mEntities.QuadTree
                .NearEntities(position, radius)
                .OfType<T>();
        }

        /// <summary>
        /// Enumerates through the entities overlapping with <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">The entity with which the overlaps are required.</param>
        /// <typeparam name="T">The entities are filtered by this type.</typeparam>
        /// <returns>An enumerable through entities of type <typeparamref name="T"/> overlapping with <paramref name="entity"/>.</returns>
        internal IEnumerable<T> EntitiesAt<T>(Entity entity) where T : Entity
        {
            return mEntities.QuadTree.EntitiesAt(entity.Bounds).OfType<T>();
        }

        internal bool HasEntityAt(Vector2 point)
        {
            return mEntities.HasEntityAt(point);
        }

        #endregion

        #region Path Finding

        public Vector2 RelativeMovement(Point point)
        {
            var rectangle = new Rectangle(new Point(-Grid.KachelSize), new Point(Grid.KachelSize * 2));
            return rectangle.At(mVectorField[point]);
        }

        public Vector2 RelativeMovementThunderbird(Point point)
        {
            var rectangle = new Rectangle(new Point(-Grid.KachelSize), new Point(Grid.KachelSize * 2));
            return rectangle.At(mVectorFieldThunderbird[point]);
        }

        internal int? TileHeat(Point point)
        {
            return (int?) mVectorField.HeatMap[point];
        }

        internal AStar MakePathFinding(Entity entity, Point start, Point target)
        {
            var matrixObstacles = new ObstacleMatrix(Grid);
            matrixObstacles.Raster(mEntities.AllEntities, e => e != entity && e.GetType() != typeof(ShockField));
            var aStar = new AStar(start, target, matrixObstacles);
            aStar.CalculatePath();
            return aStar;
        }

        #endregion

        #region Visualization

        internal Visualizer Visualize(AStar pathPlanner)
        {
            return pathPlanner.CreateVisualization(Grid, mSpriteManager);
        }

        #endregion

        public void DamageBase(int damage) => Target.Power = Math.Max(0, Target.Power - damage);

        internal void AddProjectile(Projectile projectile)
        {
            mEntities.Add(projectile);
        }
    }
}
