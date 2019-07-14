using System;
using System.Diagnostics.CodeAnalysis;

namespace KernelPanic.Entities.Buildings
{
    // This is instantiated via black magic originating from Building.Create.
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal sealed class ShockField : Tower
    {
        internal ShockField(SpriteManager spriteManager, SoundManager soundManager)
            : base(1, 1, 2, 0,TimeSpan.FromSeconds(3), spriteManager.CreateShockField(), spriteManager, soundManager)
        {
            // The fire timer is not used by the Shockfield.
            FireTimer.Enabled = false;
        }

        internal override void UpdateInformation()
        {
            base.UpdateInformation();
            mInfoText.Text += $"\nStärke: {Damage}";
        }
    }
}
