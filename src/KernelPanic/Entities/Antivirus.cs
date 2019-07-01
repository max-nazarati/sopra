using System;
using KernelPanic.Sprites;

namespace KernelPanic.Entities
{
    internal class Antivirus : StrategicTower
    {
        internal Antivirus(Sprite sprite, SpriteManager spriteManager
            , SoundManager sounds) : base(price:30, radius:10, cooldown: TimeSpan.FromSeconds(3), sprite: sprite, spriteManager: spriteManager, sounds: sounds)
        {
        
        }

        internal Antivirus(SpriteManager spriteManager, SoundManager soundManager) : base(spriteManager, soundManager)
        {
        }
    }
}