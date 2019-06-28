using System;
using KernelPanic.Sprites;

namespace KernelPanic.Entities
{
    internal class Antivirus : StrategicTower
    {
        internal Antivirus(int price, float radius, TimeSpan cooldown, Sprite sprite, SpriteManager sprites
            , SoundManager sounds) : base(price, radius, cooldown, sprite, sprites, sounds)
        {
        
        }

        internal Antivirus(SpriteManager spriteManager, SoundManager soundManager) : base(spriteManager, soundManager)
        {
        }
    }
}