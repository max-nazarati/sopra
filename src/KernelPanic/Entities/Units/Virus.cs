using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities.Units
{
    // This is instantiated via black magic originating from Unit.Create.
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal sealed class Virus : Troupe
    {
        private Rectangle mHitBox;

        public override Rectangle Bounds
        {
            get
            {
                mHitBox.X = Sprite.Bounds.X + 21;
                mHitBox.Y = Sprite.Bounds.Y + 21;
                return mHitBox;
            }
        }

        internal Virus(SpriteManager spriteManager)
            : base(3, 3, 10, 2, spriteManager.CreateVirus(), spriteManager)
        {
            mHitBox = Sprite.Bounds;
            mHitBox.Width = 22;
            mHitBox.Height = 22;
        }
    }
}