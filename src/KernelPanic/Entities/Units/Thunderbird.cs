using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities.Units
{
    // This is instantiated via black magic originating from Unit.Create.
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal sealed class Thunderbird : Troupe
    {
        protected override Point HitBoxSize => new Point(53, 55);

        internal Thunderbird(SpriteManager spriteManager)
            : base(15, 2, 15, 3, spriteManager.CreateThunderbird(), spriteManager)
        {
        }

        internal override bool IsSmall => false;
    }
}