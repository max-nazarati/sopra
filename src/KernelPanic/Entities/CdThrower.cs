using System;
using KernelPanic.Sprites;

namespace KernelPanic.Entities
{
    internal class CdThrower : StrategicTower
    {
        internal CdThrower(int price, float radius, TimeSpan cooldown, Sprite sprite, SpriteManager sprites
            , SoundManager sounds) : base(price, radius, cooldown, sprite, sprites, sounds)
        {
        }

        internal CdThrower(SpriteManager spriteManager, SoundManager soundManager) : base(spriteManager, soundManager)
        {
        }
    }
}