using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities.Units
{
    // This is instantiated via black magic originating from Unit.Create.
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal sealed class Nokia : Troupe
    {
        private Rectangle mHitBox;

        public override Rectangle Bounds
        {
            get
            {
                mHitBox.X = Sprite.Bounds.X + 24;
                mHitBox.Y = Sprite.Bounds.Y + 12;
                return mHitBox;
            }
        }

        internal Nokia(SpriteManager spriteManager)
            : base(30, 2, 100, 15, spriteManager.CreateNokia(), spriteManager)
        {
            mHitBox = Sprite.Bounds;
            mHitBox.Width = 18;
            mHitBox.Height = 39;
        }
    }
}