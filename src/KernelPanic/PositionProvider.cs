using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace KernelPanic
{
    internal class PositionProvider
    {
        private readonly SpriteManager mSpriteManager;
        private readonly Grid mGrid;
        private readonly EntityGraph mEntities;

        internal PositionProvider(Grid grid, EntityGraph entities, SpriteManager sprite)
        {
            mGrid = grid;
            mEntities = entities;
            mSpriteManager = sprite;
        }

        internal Vector2? GridCoordinate(Vector2 position, int subTileCount = 1)
        {
            return mGrid.GridPointFromWorldPoint(position, subTileCount)?.Position;
        }
        
        // Funktion mach Pfadplanung (benutzt Astar mithilfe von entity graph)
        internal List<Point> MakePathFinding()
        {
            // List<Point> obstacles = mEntities.Obstacles;
            var obstacles =  new List<Point>();
            var currentPosition = new Point(0, 0); // TODO get this right (entitygraph?)
            var targetPosition = new Point(5, 5); // TODO get this right
            var aStar = new AStar(mGrid.CoordSystem, currentPosition, targetPosition, mSpriteManager);
            aStar.CalculatePath();
            return aStar.Path;
        }
    }
}
