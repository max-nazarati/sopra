using KernelPanic.Entities;
using KernelPanic.Players;

namespace KernelPanic.Table
{
    internal sealed class Owner
    {
        private Player mAttacker;
        private Player mDefender;

        public Owner(Player attacker, Player defender)
        {
            mAttacker = attacker;
            mDefender = defender;
        }

        internal Player this[Entity entity]
        {
            get
            {
                switch (entity)
                {
                    case Building _:
                        return mDefender;
                    case Unit _:
                        return mAttacker;
                    default:
                        return null;
                }
            }
        }
    }
}