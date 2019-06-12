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
        private AStar mAStar;

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
        
        internal List<Point> MakePathFinding(Vector2? start, Vector2? target)
        {
            Point startPoint = new Point((int)start?.X , (int)start?.Y);
            Point targetPoint = new Point((int)target?.X, (int)target?.Y);
            return MakePathFinding(startPoint, targetPoint);
        }
        
        internal List<Point> MakePathFinding(Point start, Point target)
        {
            // List<Point> obstacles = mEntities.Obstacles;
            // start = new Point(0, 0); // TODO
            // target = new Point(6, 10); // TODO
            var obstacles =  new List<Point>();
            var aStar = new AStar(mGrid.CoordSystem, start, target, mSpriteManager);
            aStar.ChangeObstacleEnvironment(1);
            aStar.CalculatePath();
            mAStar = aStar;
            return aStar.Path;
        }

        internal void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (mAStar != null)
            {
                DrawAStar(spriteBatch, gameTime);    
            }
            
        }
        
        private void DrawAStar(SpriteBatch spriteBatch, GameTime gameTime)
        {
            mAStar.CalculatePath();
            mAStar.Draw(spriteBatch, gameTime);
        }
    }
}
