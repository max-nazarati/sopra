using System;
using System.Collections.Generic;
using System.Linq;
using KernelPanic.Data;
using KernelPanic.Entities;
using KernelPanic.Entities.Buildings;
using KernelPanic.Entities.Units;
using KernelPanic.Table;
using Microsoft.Xna.Framework;

namespace KernelPanic.PathPlanning
{
    /// <summary>
    /// Represents a 2-dimensional matrix which stores the existence or non-existence of obstacles on a specific tile.
    /// </summary>
    internal sealed class ObstacleMatrix
    {
        #region Properties

        private readonly Grid mGrid;
        private readonly bool[,] mObstacles;
        private readonly bool mHasBorder;

        internal int SubTileCount { get; }

        #endregion

        #region Constructor

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

            mGrid = grid;
            mHasBorder = includeBorder;
            SubTileCount = subTileCount;

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
                    cutoutXStart = Grid.LaneWidthInTiles;
                    cutoutXEnd = grid.LaneRectangle.Width;
                    break;
                case Lane.Side.Right:
                    cutoutXStart = 0;
                    cutoutXEnd = grid.LaneRectangle.Width - Grid.LaneWidthInTiles;
                    break;
                default:
                    throw new InvalidOperationException("Grid has an invalid side");
            }
            
            for (var i = Grid.LaneWidthInTiles * subTileCount; i < (grid.LaneRectangle.Height - Grid.LaneWidthInTiles) * subTileCount; ++i)
            {
                for (var j = cutoutXStart * subTileCount; j < cutoutXEnd * subTileCount; ++j)
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

        #endregion

        #region Accessing

        internal int Rows => mObstacles.GetLength(0) - (mHasBorder ? 2 : 0);
        internal int Columns => mObstacles.GetLength(1) - (mHasBorder ? 2 : 0);

        /// <summary>
        /// Queries whether there is an obstacle at the given point.
        /// </summary>
        /// <param name="point">The point to ask about.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="point"/> lies outside this matrix.</exception>
        internal bool this[Point point]
        {
            get => this[point.Y, point.X];
            set => this[point.Y, point.X] = value;
        }

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

        #endregion

        #region Adding obstacles

        /// <summary>
        /// Adds all elements from <paramref name="elements"/> for which the predicate returns <c>true</c> as obstacles
        /// into this <see cref="ObstacleMatrix"/>. Only those tiles containing the center of an entity are marked as
        /// blocked.
        /// </summary>
        /// <param name="elements">The elements to add, can be a <see cref="QuadTree{T}"/>.</param>
        /// <param name="predicate">A predicate to filter the elements.</param>
        /// <typeparam name="T">The type of the elements, must implement <see cref="IBounded"/>.</typeparam>
        internal void Raster<T>(IEnumerable<T> elements, Func<T, bool> predicate = null) where T: IBounded
        {
            foreach (var element in predicate == null ? elements : elements.Where(predicate))
            {
                var center = element.Bounds.At(RelativePosition.Center);
                if (mGrid.TileFromWorldPoint(center, SubTileCount) is TileIndex tile)
                {
                    this[tile.ToPoint()] = true;
                }
                else
                {
                    throw new InvalidOperationException($"Element {element} is outside the bounds");
                }
            }
        }

        #endregion

        #region Enumerating through the tiles

        internal IEnumerable<TileIndex> Obstacles => EnumerateIndices(true);

        private IEnumerable<TileIndex> EnumerateIndices(bool obstacles)
        {
            for (var row = 0; row < Rows; ++row)
            {
                for (var col = 0; col < Columns; ++col)
                {
                    if (this[row, col] == obstacles)
                        yield return new TileIndex(row, col, SubTileCount);
                }
            }
        }

        #endregion
    }
}
