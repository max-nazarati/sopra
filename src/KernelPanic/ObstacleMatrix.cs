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
        private readonly bool mHasBorder;

        /*internal*/ private int Rows => mObstacles.GetLength(0);
        /*internal*/ private int Columns => mObstacles.GetLength(1);

        /// <summary>
        /// Creates a new obstacle matrix without any obstacles.
        /// </summary>
        /// <param name="grid">The underlying grid.</param>
        /// <param name="subTileCount">The number of sub-tiles (in one dimension) for each big tile.</param>
        /// <param name="includeBorder">If true an additional field is added in every direction acting as an obstacle.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="subTileCount"/> is less than one.</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="grid"/> was not constructed with a valid <see cref="Lane.Side"/> value.</exception>
        internal ObstacleMatrix(Grid grid, int subTileCount = 1, bool includeBorder = true)
        {
            if (subTileCount < 1)
                throw new ArgumentOutOfRangeException(nameof(subTileCount), "should be at least 1");

            mHasBorder = includeBorder;

            // Elements initialized to false by default.
            var borderCount = includeBorder ? 2 : 0;
            mObstacles = new bool[
                grid.LaneRectangle.Height * subTileCount + borderCount,
                grid.LaneRectangle.Width * subTileCount + borderCount];

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
                {
                    // Use our indexer because it automatically translates the coordinates in case there is a border.
                    this[i, j] = true;
                }
            }
            
            if (!mHasBorder)
                return;

            var rows = mObstacles.GetLength(0);
            var columns = mObstacles.GetLength(1);

            for (var i = 0; i < columns; ++i)
            {
                mObstacles[0, i] = true;
                mObstacles[rows - 1, i] = true;
            }

            for (var i = 1; i < rows - 1; ++i)
            {
                mObstacles[i, 0] = true;
                mObstacles[i, columns - 1] = true;
            }
        }

        /// <summary>
        /// Queries whether there is an obstacle at the given point.
        /// </summary>
        /// <param name="point">The point to ask about.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="point"/> lies outside this matrix.</exception>
        internal bool this[Point point] => this[point.Y, point.X];

        private bool this[int row, int column]
        {
            get
            {
                VerifyRange(nameof(row), ref row, 0);
                VerifyRange(nameof(column), ref column, 1);
                return mObstacles[row, column];
            }
            set
            {
                VerifyRange(nameof(row), ref row, 0);
                VerifyRange(nameof(column), ref column, 1);
                mObstacles[row, column] = value;
            }
        }

        private void VerifyRange(string name, ref int value, int dimension)
        {
            var size = mObstacles.GetLength(dimension);
            var lowerBound = mHasBorder ? -1 : 0;
            var upperBound = size - lowerBound;
            if (value < lowerBound || value > upperBound)
                throw new ArgumentOutOfRangeException(name, value, $"not in range [{lowerBound}; {upperBound})");

            value -= lowerBound;
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
                        // Use our indexer because it automatically translates the coordinates in case there is a border.
                        this[i, j] = true;
                    }
                }
            }
        }
    }
}
