using System.Diagnostics.CodeAnalysis;

namespace KernelPanic.Entities
{
    // This is instantiated via black magic originating from Building.Create.
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal sealed class Cable : Building
    {
        // The parameters are required for Building.Create to work!
        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        internal Cable(SpriteManager spriteManager, SoundManager soundManager)
            : base(10, spriteManager.CreateCable(), spriteManager)
        {
        }
    }
}
