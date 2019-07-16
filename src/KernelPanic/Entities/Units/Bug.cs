using Microsoft.Xna.Framework;

namespace KernelPanic.Entities.Units
{
    internal sealed class Bug : Troupe
    {
        private Rectangle mHitBox;

        public override Rectangle Bounds
        {
            get
            {
                mHitBox.X = Sprite.Bounds.X + 26;
                mHitBox.Y = Sprite.Bounds.Y + 35;
                return mHitBox;
            }
        }

        internal Bug(SpriteManager spriteManager)
            : base(2, 4, 4, 1, spriteManager.CreateBug(), spriteManager)
        {
            mHitBox = Sprite.Bounds;
            mHitBox.Width = 22;
            mHitBox.Height = 11;
        }
    }
}