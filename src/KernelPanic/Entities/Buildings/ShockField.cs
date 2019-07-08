using System;
using System.Diagnostics.CodeAnalysis;
using KernelPanic.Input;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities.Buildings
{
    // This is instantiated via black magic originating from Building.Create.
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal sealed class ShockField : Tower
    {
        internal ShockField(SpriteManager spriteManager, SoundManager soundManager)
            : base(1, 0, 2, 0,TimeSpan.FromSeconds(3), spriteManager.CreateShockField(), spriteManager, soundManager)
        {
            // The fire timer is not used by the Shockfield.
            FireTimer.Enabled = false;
        }
    }
}
