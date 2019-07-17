using Microsoft.Xna.Framework;

namespace KernelPanic.Entities.Units
{
    internal sealed class Bug : Troupe
    {
        protected override Point HitBoxSize => new Point(22, 11);

        internal Bug(SpriteManager spriteManager)
            : base(2, 4, 4, 1, spriteManager.CreateBug(), spriteManager)
        {
        }

        internal override bool IsSmall => true;
    }
}