using Microsoft.Xna.Framework;

namespace KernelPanic.Entities.Units
{
    internal sealed class Bug : Troupe
    {
        private static Point HitBoxSize => new Point(22, 11);

        internal Bug(SpriteManager spriteManager)
            : base(2, 4, 4, 1, HitBoxSize, spriteManager.CreateBug(), spriteManager)
        {
        }

        internal override bool IsSmall => true;
    }
}