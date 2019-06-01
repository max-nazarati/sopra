using System;

namespace KernelPanic
{
    internal class StrategicTower : Tower
    {
        internal TowerStrategy Strategy { get; set; } = TowerStrategy.First;
        
        public StrategicTower(TimeSpan timeSpan) : base(timeSpan)
        {
        }
    }

    internal enum TowerStrategy { First, Strongest, Weakest };
}
