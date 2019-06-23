using System;

namespace KernelPanic.Entities
{
    internal sealed class Settings : Hero
    {
        internal Settings(SpriteManager spriteManager)
            : base(50, 4, 25, 0, TimeSpan.FromSeconds(5), spriteManager.CreateSettings(), spriteManager)
        {
            mIndicator = spriteManager.CreateHealIndicator();
        }
    }
}