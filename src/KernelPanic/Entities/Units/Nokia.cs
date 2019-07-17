using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities.Units
{
    // This is instantiated via black magic originating from Unit.Create.
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal sealed class Nokia : Troupe
    {
        protected override Point HitBoxSize => new Point(18, 39);

        internal Nokia(SpriteManager spriteManager)
            : base(50, 1, 100, 15, spriteManager.CreateNokia(), spriteManager)
        {
        }

        internal override bool IsSmall => false;
    }
}