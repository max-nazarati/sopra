using System;
using KernelPanic.Sprites;

namespace KernelPanic.Entities
{
    internal class StrategicTower : Tower
    {
        internal TowerStrategy Strategy { get; set; } = TowerStrategy.First;
        
        internal StrategicTower(int price, float radius, TimeSpan cooldown, Sprite sprite, SpriteManager sprites, SoundManager sounds)
            : base(price, radius, cooldown, sprite, sprites, sounds)
        {
        }
    }

    internal enum TowerStrategy { First, Strongest, Weakest };
}