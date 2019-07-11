using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace KernelPanic.Table
{
    internal struct TileIndex
    {
        internal int Row { get; /*set;*/ }
        internal int Column { get; /*set;*/ }
        internal int SubTileCount { get; }
        
        internal Point ToPoint() => new Point(Column, Row);

        internal TileIndex(int row, int column, int subTileCount)
        {
            Row = row;
            Column = column;
            SubTileCount = subTileCount;
        }

        internal TileIndex(Point point, int subTileCount)
        {
            Row = point.Y;
            Column = point.X;
            SubTileCount = subTileCount;
        }

        /// <summary>
        /// Turns a <see cref="TileIndex"/> into a set of indices which cover the same area. This function only works
        /// correctly, if <paramref name="subTileCount"/> is a multiple <see cref="SubTileCount"/> and
        /// <paramref name="subTileCount"/> is greater than or equal to <see cref="SubTileCount"/>;
        /// </summary>
        /// <param name="subTileCount">The wanted <see cref="SubTileCount"/>.</param>
        /// <returns>A set of indices covering the same area.</returns>
        internal IEnumerable<TileIndex> Rescaled(int subTileCount)
        {
            var mul = subTileCount / SubTileCount;
            for (int row = Row * mul, i = 0; i < subTileCount; i++, row++)
            {
                for (int col = Column * mul, j = 0; j < subTileCount; j++, col++)
                {
                    yield return new TileIndex(row, col, subTileCount);
                }
            }
        }

        public override string ToString() => $"[row: {Row}, col: {Column}, sub-tiles: {SubTileCount}]";
    }
}
