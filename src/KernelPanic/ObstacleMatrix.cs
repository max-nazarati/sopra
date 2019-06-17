using System;
using System.Collections.Generic;
using System.Linq;
using KernelPanic.Data;
using KernelPanic.Table;
using Microsoft.Xna.Framework;

namespace KernelPanic
{
    /// <summary>
    /// Represents a 2-dimensional matrix which stores the existence or non-existence of obstacles on a specific tile.
    /// </summary>
    internal sealed class ObstacleMatrix
    {
        private readonly bool[,] mObstacles;

        /*internal*/ private int Rows => mObstacles.GetLength(0);
        /*internal*/ private int Columns => mObstacles.GetLength(1);

        /// <summary>
        /// Creates a new obstacle matrix without any obstacles.
        /// </summary>
        /// <param name="grid">The underlying grid.</param>
        /// <param name="subTileCount">The number of sub-tiles (in one dimension) for each big tile.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="subTileCount"/> is less than one.</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="grid"/> was not constructed with a valid <see cref="Lane.Side"/> value.</exception>
        internal ObstacleMatrix(Grid grid, int subTileCount = 1)
        {
            if (subTileCount < 1)
                throw new ArgumentOutOfRangeException(nameof(subTileCount), "should be at least 1");
            
            // Elements initialized to false by default.
            mObstacles = new bool[grid.LaneRectangle.Height * subTileCount, grid.LaneRectangle.Width * subTileCount];

            // Set all elements to true which represent fields outside the lane.
            int cutoutXStart, cutoutXEnd;
            switch (grid.LaneSide)
            {
                case Lane.Side.Left:
                    cutoutXStart = grid.LaneRectangle.Width - Grid.LaneWidthInTiles;
                    cutoutXEnd = grid.LaneRectangle.Width;
                    break;
                case Lane.Side.Right:
                    cutoutXStart = 0;
                    cutoutXEnd = grid.LaneRectangle.Width - Grid.LaneWidthInTiles;
                    break;
                default:
                    throw new InvalidOperationException("Grid has an invalid side");
            }
            
            for (var i = Grid.LaneWidthInTiles; i < grid.LaneRectangle.Height - Grid.LaneWidthInTiles; ++i)
            {
                for (var j = cutoutXStart; j < cutoutXEnd; ++j)
                    mObstacles[i, j] = true;
            }
        }

        /// <summary>
        /// Queries whether there is an obstacle at the given point.
        /// </summary>
        /// <param name="point">The point to ask about.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="point"/> lies outside this matrix.</exception>
        internal bool this[Point point]
        {
            get
            {
                VerifyRange("point.Y", point.Y, 0);
                VerifyRange("point.X", point.X, 1);
                return mObstacles[point.Y, point.X];
            }
        }

        private void VerifyRange(string name, int value, int dimension)
        {
            var size = mObstacles.GetLength(dimension);
            if (value < 0 || value > size)
                throw new ArgumentOutOfRangeException(name, value, $"not in range [0; {size})");
        }

        /// <summary>
        /// Adds all elements from <paramref name="elements"/> for which the predicate returns <c>true</c> as obstacles
        /// into this <see cref="ObstacleMatrix"/>. Every tile—even if only intersected by a small part—is marked as an
        /// obstacle.
        ///
        /// <para>
        /// Because an obstacle matrix has no inherent position or size in the world, <paramref name="rasterBounds"/>
        /// has to be provided to define where this matrix is located.
        /// </para>
        /// </summary>
        /// <param name="elements">The elements to add, can be a <see cref="QuadTree{T}"/>.</param>
        /// <param name="rasterBounds">Defines the area which is spanned by the <see cref="ObstacleMatrix"/>.</param>
        /// <param name="predicate">A predicate to filter the elements.</param>
        /// <typeparam name="T">The type of the elements, must implement <see cref="IBounded"/>.</typeparam>
        internal void Rasterize<T>(IEnumerable<T> elements, Rectangle rasterBounds, Func<T, bool> predicate = null) where T: IBounded
        {
            var xSize = (float) rasterBounds.Width / Columns;
            var ySize = (float) rasterBounds.Height / Rows;
            foreach (var element in predicate == null ? elements : elements.Where(predicate))
            {
                var bounds = Rectangle.Intersect(element.Bounds, rasterBounds);

                for (var i = (int) (bounds.Top / ySize); i * ySize < bounds.Bottom; ++i)
                {
                    for (var j = (int) (bounds.Left / xSize); j * xSize < bounds.Right; ++j)
                    {
                        mObstacles[i, j] = true;
                    }
                }
            }
        }
    }
}
