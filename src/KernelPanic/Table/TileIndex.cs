using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace KernelPanic.Table
{
    internal struct TileIndex
    {
        internal int Row { get; set; }
        internal int Column { get; set; }
        internal int SubTileCount { get; }
        
        internal Point ToPoint() => new Point(Column, Row);

        public override string ToString() => $"[row: {Row}, col: {Column}, sub-tiles: {SubTileCount}]";
    }
}
