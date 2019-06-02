namespace KernelPanic
{
    class Player
    {
        public Base Base { get; set; }
        // private Base mBase;
        // private List<Upgrade> Upgrades;
        private Lane mAttackingLane;
        private Lane mDefendingLane;

        public Player (int bitcoins = 50)
        {
            Base = new Base();
            Bitcoins = bitcoins;
        }


        public int Bitcoins { get; set; }
        public int ExperiencePoints { get; set; }
    }
}
