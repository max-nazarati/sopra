using Microsoft.Xna.Framework;

namespace KernelPanic.Table
{
    internal struct TileIndex
    {
        internal int Row { get; set; }
        internal int Column { get; set; }
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

        public override string ToString() => $"[row: {Row}, col: {Column}, sub-tiles: {SubTileCount}]";
    }
}
