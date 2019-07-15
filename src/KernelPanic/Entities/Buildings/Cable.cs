using System.Diagnostics.CodeAnalysis;

namespace KernelPanic.Entities.Buildings
{
    // This is instantiated via black magic originating from Building.Create.
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal sealed class Cable : Building
    {
        internal Cable(SpriteManager spriteManager)
            : base(10, spriteManager.CreateCable(), spriteManager)
        {
        }
    }
}
