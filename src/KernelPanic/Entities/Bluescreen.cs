using System;

namespace KernelPanic.Entities
{
    internal sealed class Bluescreen : Hero
    {
        internal Bluescreen(SpriteManager spriteManager)
            : base(50, 9, 15, 0, TimeSpan.FromSeconds(5), spriteManager.CreateBluescreen(), spriteManager)
        {
            mIndicator = spriteManager.CreateEMPIndicator();
        }
    }
}