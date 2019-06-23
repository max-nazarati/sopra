using System;
using KernelPanic.Sprites;

namespace KernelPanic.Entities
{
    internal sealed class Bluescreen : Hero
    {
        private readonly ImageSprite mIndicator;
        
        internal Bluescreen(SpriteManager spriteManager)
            : base(50, 9, 15, 0, TimeSpan.FromSeconds(5), spriteManager.CreateBluescreen(), spriteManager)
        {
            var mAbilityRange = 1000;
            mIndicator = spriteManager.CreateEmpIndicator(mAbilityRange);
        }
    }
}