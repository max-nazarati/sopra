using System;
using Newtonsoft.Json;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities.Units
{
    internal sealed class Trojan : Troupe
    {
        [JsonProperty] internal int ChildCount { get; set; } = 5;

        private Rectangle mHitBox;

        public override Rectangle Bounds
        {
            get
            {
                mHitBox.X = Sprite.Bounds.X + 15;
                mHitBox.Y = Sprite.Bounds.Y + 8;
                return mHitBox;
            }
        }

        internal Trojan(SpriteManager spriteManager)
            : base(20, 3, 30, 6, spriteManager.CreateTrojan(), spriteManager)
        {
            mHitBox = Sprite.Bounds;
            mHitBox.Width = 34;
            mHitBox.Height = 45;
        }

        protected override void DidDie()
        {
            if (Wave.IsValid)
            {
                for (var i = 0; i < ChildCount; ++i)
                    Wave.SpawnChild(new Bug(SpriteManager) {Sprite = {Position = Sprite.Position}});
            }
            else
            {
                Console.WriteLine("I would like to make some bugs, but my wave is not set.");
            }

            base.DidDie();
        }
    }
}
