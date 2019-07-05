using System;
using System.Collections.Generic;
using System.Linq;
using KernelPanic.Data;
using KernelPanic.Entities;
using KernelPanic.PathPlanning;
using KernelPanic.Table;
using Microsoft.Xna.Framework;

namespace KernelPanic
{
    internal class PositionProvider
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

        internal bool HasEntityAt(Vector2 point)
        {
            return mEntities.HasEntityAt(point);
        }

        #endregion

        #region Path Finding

        public Vector2 MovementVector(Point point)
        {
            return mVectorField[point];
        }

        public Vector2 MovementVectorThunderbird(Point point)
        {
            return mVectorFieldThunderbird[point];
        }

        internal AStar MakePathFinding(Entity entity, Point start, Point target)
        {
            var matrixObstacles = new ObstacleMatrix(Grid);
            matrixObstacles.Raster(mEntities, e => e != entity);
            var aStar = new AStar(start, target, matrixObstacles);
            aStar.CalculatePath();
            return aStar;
        }

        internal bool CheckPathExistance(Point? start, Point? target, Building building)
        {
            if (start == null || target == null)
                return false;

            var matrixObstacles = new ObstacleMatrix(Grid);
            var tmpEntities = mEntities.ToList();
            tmpEntities.Add(building);
            matrixObstacles.Raster(tmpEntities, e => !(e is Unit));
            var aStar = new AStar((Point)start, (Point)target, matrixObstacles);
            aStar.CalculatePath();

            if (aStar.Path != null && aStar.Path[aStar.Path.Count - 1] == target)
                return true;
            return false;
        }

        #endregion

        #region Visualization

        internal Visualizer Visualize(AStar pathPlanner)
        {
            return pathPlanner.CreateVisualization(Grid, mSpriteManager);
        }

        #endregion

        public void DamageBase(int damage) => Target.Power = Math.Max(0, Target.Power - damage);
    }
}
