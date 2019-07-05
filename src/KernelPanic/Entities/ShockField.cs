using System.Diagnostics.CodeAnalysis;

namespace KernelPanic.Entities
{
    // This is instantiated via black magic originating from Building.Create.
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal sealed class ShockField : Building
    {
        // The parameters are required for Building.Create to work!
        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        internal ShockField(SpriteManager spriteManager, SoundManager soundManager) 
            : base(1, spriteManager.CreateShockField(), spriteManager)
        {
            
        }
    }
}
