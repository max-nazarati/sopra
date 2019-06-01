namespace KernelPanic
{
    class Player
    {
        // private Base mBase;
        // private List<Upgrade> Upgrades;
        private Lane mAttackingLane;
        private Lane mDefendingLane;

        public Player(int bitcoins = 50)
        {
            Bitcoins = bitcoins;
        }


        public int Bitcoins { get; set; }
        public int ExperiencePoints { get; set; }
    }
}
