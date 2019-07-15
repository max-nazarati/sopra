using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace KernelPanic.Table
{
    internal sealed class Base
    {
        [JsonProperty] public int Power { get; set; } = 100;
        [JsonProperty] internal Point[] HitBox { get; private set; }

        [JsonConstructor]
        private Base()
        {
        }

        internal Base(Point laneSize, Lane.Side side)
        {
            HitBox = new Point[Grid.LaneWidthInTiles];
            var offset = side == Lane.Side.Left ? laneSize.Y - Grid.LaneWidthInTiles : 0;
            for (var i = 0; i < Grid.LaneWidthInTiles; ++i)
            {
                if (side == Lane.Side.Left)
                {
                    HitBox[i] = new Point(laneSize.X - 1, offset + i);
                }
                else
                {
                    HitBox[i] = new Point(0, offset + i);
                }
            }
        }
    }
}
