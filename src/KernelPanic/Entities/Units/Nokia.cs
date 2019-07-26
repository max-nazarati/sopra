using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities.Units
{
    // This is instantiated via black magic originating from Unit.Create.
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal sealed class Nokia : Troupe
    {
        private static Point HitBoxSize => new Point(18, 39);

        internal Nokia(SpriteManager spriteManager)
            : base(100, 1, 100, 10, HitBoxSize, spriteManager.CreateNokia(), spriteManager)
        {
        }

        internal override bool IsSmall => false;
    }
}