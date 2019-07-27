using Autofac.Util;
using KernelPanic.Data;
using KernelPanic.Players;

namespace KernelPanic.ArtificialIntelligence
{
    internal abstract class Planner : Disposable
    {
        private readonly WeakReference<Player> mPlayer;

        protected Player Player => mPlayer.Target;

        protected Planner(Player player)
        {
            mPlayer = player;
        }

        public virtual void Update()
        {
            // Do nothing by default.
        }
    }
}