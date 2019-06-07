namespace KernelPanic
{
    internal sealed class Player
    {
        // private List<Upgrade> Upgrades;
        private Lane mAttackingLane;
        private Lane mDefendingLane;

        public int Bitcoins { get; set; }
        public int ExperiencePoints { get; set; }

        internal Base Base => mDefendingLane.Target;

        internal Player(Lane defendingLane, Lane attackingLane) : this(50, defendingLane, attackingLane)
        {
        }

        private Player(int bitcoins, Lane defendingLane, Lane attackingLane)
        {
            Bitcoins = bitcoins;
            mAttackingLane = attackingLane;
            mDefendingLane = defendingLane;
        }
    }
}
