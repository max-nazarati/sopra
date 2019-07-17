using System;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities.Units
{
    internal sealed class Trojan : Troupe
    {
        [JsonProperty] internal int ChildCount { get; set; } = 5;

        protected override Point HitBoxSize => new Point(34, 45);

        internal Trojan(SpriteManager spriteManager)
            : base(30, 2, 30, 6, spriteManager.CreateTrojan(), spriteManager)
        {
        }

        internal override bool IsSmall => false;

        protected override void DidDie(PositionProvider positionProvider)
        {
            if (Wave.IsValid)
            {
                var tile = positionProvider.RequireTile(this);
                var smallTiles = tile.Rescaled(2).ToList();
                for (var i = 0; i < ChildCount; ++i)
                {
                    Wave.SpawnChild(new Bug(SpriteManager), smallTiles[i % smallTiles.Count]);
                }
            }
            else
            {
                Console.WriteLine("I would like to make some bugs, but my wave is not set.");
            }

            base.DidDie(positionProvider);
        }
    }
}
