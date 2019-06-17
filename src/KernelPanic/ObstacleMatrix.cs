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
        private readonly Rectangle mBounds;

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
            mBounds = grid.Bounds;

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
        /// Queries/modifies whether there is an obstacle at the given point.
        /// </summary>
        /// <param name="row">The row (zero-based).</param>
        /// <param name="column">The column (zero-based).</param>
        /// <exception cref="ArgumentOutOfRangeException">If either <paramref name="row"/> or <paramref name="column"/> are out of range.</exception>
        internal bool this[int row, int column]
        {
            get
            {
                VerifyRange(nameof(row), row, 0);
                VerifyRange(nameof(column), column, 1);
                return mObstacles[row, column];
            }
            set
            {
                VerifyRange(nameof(row), row, 0);
                VerifyRange(nameof(column), column, 1);
                mObstacles[row, column] = value;
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
        /// </summary>
        /// <param name="elements">The elements to add, can be a <see cref="QuadTree{T}"/>.</param>
        /// <param name="predicate">A predicate to filter the elements.</param>
        /// <typeparam name="T">The type of the elements, must implement <see cref="IBounded"/>.</typeparam>
        internal void Rasterize<T>(IEnumerable<T> elements, Func<T, bool> predicate = null) where T: IBounded
        {
            if (predicate != null)
                elements = elements.Where(predicate);
            foreach (var element in elements)
            {
                var bounds = Rectangle.Intersect(element.Bounds, mBounds);
                var xSize = (float) mBounds.Width / Columns;
                var ySize = (float) mBounds.Height / Rows;

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
