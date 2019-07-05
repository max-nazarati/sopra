using System.Diagnostics.CodeAnalysis;

namespace KernelPanic.Entities
{
    // This is instantiated via black magic originating from Unit.Create.
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal sealed class Virus : Troupe
    {
        internal Virus(SpriteManager spriteManager)
            : base(3, 5, 10, 2, spriteManager.CreateVirus(), spriteManager)
        {
        }
    }
}