using System;

namespace KernelPanic
{
    internal class StrategicTower : Tower
    {
        internal TowerStrategy Strategy { get; set; } = TowerStrategy.First;
        
        internal StrategicTower(int price, float radius, TimeSpan cooldown, Sprite sprite, SpriteManager sprites)
            : base(price, radius, cooldown, sprite, sprites)
        {
        }
    }

    internal enum TowerStrategy { First, Strongest, Weakest };
}
