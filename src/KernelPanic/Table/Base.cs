using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace KernelPanic.Table
{
    internal sealed class Base
    {
        [JsonProperty]
        public int Power { get; set; }

        [JsonProperty]
        public Point Position { get; set; }

        public Base(Point position)
        {
            Power = 1;
            Position = position;
        }

        internal IEnumerable<Point> HitBox => new[]
        {
            Position,
            new Point(Position.X + 1, Position.Y), new Point(Position.X - 1, Position.Y),
            new Point(Position.X, Position.Y + 1), new Point(Position.X, Position.Y - 1)
        };
    }
}
