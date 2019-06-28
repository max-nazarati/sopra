using System;
using KernelPanic.Sprites;

namespace KernelPanic.Entities
{
    internal class CursorShooter : StrategicTower
    {
        internal CursorShooter(int price, float radius, TimeSpan cooldown, Sprite sprite, SpriteManager sprites
            , SoundManager sounds) : base(price, radius, cooldown, sprite, sprites, sounds)
        {
        }

        internal CursorShooter(SpriteManager spriteManager, SoundManager soundManager)
            : base(spriteManager, soundManager)
        {
        }
    }
}