using System.Collections.Generic;
using System.Linq;
using KernelPanic.Entities;
using KernelPanic.PathPlanning;
using KernelPanic.Table;
using Microsoft.Xna.Framework;

namespace KernelPanic
{
    internal class PositionProvider
    {
        private readonly SpriteManager mSpriteManager;
        private readonly Grid mGrid;
        private readonly EntityGraph mEntities;

        internal PositionProvider(Grid grid, EntityGraph entities, SpriteManager spriteManager)
        {
            mGrid = grid;
            mEntities = entities;
            mSpriteManager = spriteManager;
        }

        internal Vector2? GridCoordinate(Vector2 position, int subTileCount = 1)
        {
            return mGrid.GridPointFromWorldPoint(position, subTileCount)?.Position;
        }

        internal AStar MakePathFinding(Entity entity, Point start, Point target)
        {
            var matrixObstacles = new ObstacleMatrix(mGrid);
            matrixObstacles.Rasterize(mEntities, mGrid.Bounds, e => e != entity);
            var aStar = new AStar(mGrid.CoordSystem, start, target, matrixObstacles);
            aStar.ChangeObstacleEnvironment(1);
            aStar.CalculatePath();
            return aStar;
        }

        internal Visualizer Visualize(AStar pathPlanner)
        {
            return pathPlanner.CreateVisualization(mGrid, mSpriteManager);
        }

        internal IEnumerable<T> NearEntities<T>(Entity entity, float radius) where T : Entity
        {
            return mEntities.QuadTree
                .NearEntities(entity.Bounds.Center.ToVector2(), radius)
                .Where(e => e is T)
                .Cast<T>();
        }
    }
}
