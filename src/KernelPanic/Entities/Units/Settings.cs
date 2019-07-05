using System;
using System.Diagnostics.CodeAnalysis;
using KernelPanic.Sprites;

namespace KernelPanic.Entities.Units
{
    // This is instantiated via black magic originating from Unit.Create.
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
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