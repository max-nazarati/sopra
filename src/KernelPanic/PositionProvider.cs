using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace KernelPanic
{
    internal class PositionProvider
    {
        private readonly Grid mGrid;
        private readonly EntityGraph mEntities;

        internal PositionProvider(Grid grid, EntityGraph entities)
        {
            mGrid = grid;
            mEntities = entities;
        }

        internal Vector2? GridCoordinate(Vector2 position, int subTileCount = 1)
        {
            return mGrid.GridPointFromWorldPoint(position, subTileCount)?.Position;
        }
        
        // Funktion mach Pfadplanung (benutzt Astar mithilfe von entit graph)
        internal MakePathfinding()
        {
            // List<Point> obstacles = EntityGraph.Obstacles;
            List<Point> obstacles = 
        }
    }
}
