using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities.Units
{
    // This is instantiated via black magic originating from Unit.Create.
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal sealed class Virus : Troupe
    {
        protected override Point HitBoxSize => new Point(22, 22);

        internal Virus(SpriteManager spriteManager)
            : base(3, 3, 10, 2, spriteManager.CreateVirus(), spriteManager)
        {
        }

        internal override bool IsSmall => true;
    }
}