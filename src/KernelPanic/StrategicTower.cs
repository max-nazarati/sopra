using System;

namespace KernelPanic
{
    class StrategicTower : Tower
    {
        private TowerStrategy mStrategy = TowerStrategy.First;
        public void Strategy()
        {

        }
        public StrategicTower(TimeSpan timeSpan) : base(timeSpan)
        {

        }
    }
    enum TowerStrategy { First, Strongest, Weakest };

}
