using System;

namespace KernelPanic
{
    internal class StrategicTower : Tower
    {
        internal TowerStrategy Strategy { get; set; } = TowerStrategy.First;
        
        public StrategicTower(TimeSpan timeSpan, SpriteManager sprites) : base(timeSpan, sprites)
        {
        }
    }

    internal enum TowerStrategy { First, Strongest, Weakest };
}
