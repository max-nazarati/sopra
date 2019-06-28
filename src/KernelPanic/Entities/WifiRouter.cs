using System;
using KernelPanic.Sprites;

namespace KernelPanic.Entities
{
    internal class WifiRouter : StrategicTower
    {
        internal WifiRouter(int price, float radius, TimeSpan cooldown, Sprite sprite, SpriteManager sprites
            , SoundManager sounds) : base(price, radius, cooldown, sprite, sprites, sounds)
        {
        }

        internal WifiRouter(SpriteManager spriteManager, SoundManager soundManager)
            : base(spriteManager, soundManager)
        {
        }
    }
}