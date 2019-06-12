using System;
using System.Runtime.Serialization;

namespace KernelPanic
{
    /// <summary>
    /// Represents a 2-dimensional matrix which stores the existence or non-existence of obstacles on a specific tile.
    /// </summary>
    [DataContract]
    internal sealed class ObstacleMatrix
    {
        [DataMember]
        private readonly bool[,] mObstacles;

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
    }
}
