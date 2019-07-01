using System;
using KernelPanic.Sprites;

namespace KernelPanic.Entities
{
    internal class Ventilator : StrategicTower
    {
        internal Ventilator(int price, float radius, TimeSpan cooldown, Sprite sprite, SpriteManager spriteManager
            , SoundManager sounds) : base(price, radius, cooldown, sprite, spriteManager, sounds)
        {
        }

        internal Ventilator(SpriteManager spriteManager, SoundManager soundManager) : base(spriteManager, soundManager)
        {
        }
    }
}