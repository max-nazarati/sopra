namespace KernelPanic
{
    class Player
    {
        private int mBitcoins;
        // private Base mBase;
        // private List<Upgrade> Upgrades;
        // private Lane mAttackingLane;
        // private Lane mDefendingLane;
        // private int mExperiencePoints;

        public Player(int bitcoins = 50)
        {
            mBitcoins = bitcoins;
        }


        public int Bitcoins => mBitcoins;

    }
}
