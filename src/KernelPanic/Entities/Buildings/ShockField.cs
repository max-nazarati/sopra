using System;
using System.Diagnostics.CodeAnalysis;

namespace KernelPanic.Entities.Buildings
{
    // This is instantiated via black magic originating from Building.Create.
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal sealed class ShockField : Tower
    {
        internal ShockField(SpriteManager spriteManager, SoundManager soundManager)
            : base(1, 0, 2, TimeSpan.FromSeconds(3), spriteManager.CreateShockField(), spriteManager, soundManager)
        {
        }
    }
}
