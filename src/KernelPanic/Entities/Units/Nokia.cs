using System.Diagnostics.CodeAnalysis;

namespace KernelPanic.Entities.Units
{
    // This is instantiated via black magic originating from Unit.Create.
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal sealed class Nokia : Troupe
    {
        internal Nokia(SpriteManager spriteManager)
            : base(30, 2, 100, 15, spriteManager.CreateNokia(), spriteManager)
        {
        }
    }
}