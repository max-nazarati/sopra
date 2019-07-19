using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace KernelPanic.Table
{
    internal sealed class Base
    {
        [JsonProperty] public int Power { get; set; } = 100;

        internal static Point[] SpawnPoints(Point laneSize, Lane.Side side)
        {
            var spawns = new Point[Grid.LaneWidthInTiles];
            for (var i = 0; i < Grid.LaneWidthInTiles; ++i)
            {
                spawns[i] = side == Lane.Side.Left
                    ? new Point(laneSize.X - 1, i)
                    : new Point(0, laneSize.Y - 1 - i);
            }

            return spawns;
        }

        internal static Point[] TargetPoints(Point laneSize, Lane.Side side)
        {
            var targets = new Point[Grid.LaneWidthInTiles];
            for (var i = 0; i < Grid.LaneWidthInTiles; ++i)
            {
                targets[i] = side == Lane.Side.Left
                    ? new Point(laneSize.X - 1, laneSize.Y - 1 - i)
                    : new Point(0, i);
            }

            return targets;
        }
    }
}
