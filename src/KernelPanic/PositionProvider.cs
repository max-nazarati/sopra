using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
        
        internal AStar MakePathFinding(Vector2? start, Vector2? target)
        {
            Point startPoint = new Point((int)start?.X, (int)start?.Y);
            Point targetPoint = new Point((int)target?.X, (int)target?.Y);
            return MakePathFinding(startPoint, targetPoint);
        }
        
        internal AStar MakePathFinding(Point start, Point target)
        {
            // List<Point> obstacles = mEntities.Obstacles;
            var obstacles =  new List<Point>();
            var aStar = new AStar(mGrid.CoordSystem, start, target, mSpriteManager);
            aStar.ChangeObstacleEnvironment(1);
            aStar.CalculatePath();
            return aStar;
        }
    }
}
