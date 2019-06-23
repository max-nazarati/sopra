using System;
using KernelPanic.Sprites;

namespace KernelPanic.Entities
{
    internal sealed class Settings : Hero
    {
        private readonly ImageSprite mIndicator;
        
        internal Settings(SpriteManager spriteManager)
            : base(50, 4, 25, 0, TimeSpan.FromSeconds(5), spriteManager.CreateSettings(), spriteManager)
        {
            var mAbilityRange = 1000;
            mIndicator = spriteManager.CreateHealIndicator(mAbilityRange);
        }
    }
}