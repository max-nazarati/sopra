using System.Runtime.Serialization;
using KernelPanic.Table;

namespace KernelPanic
{
    [DataContract]
    internal sealed class Player
    {
        // private List<Upgrade> Upgrades;
        internal Lane AttackingLane { get; }
        internal Lane DefendingLane { get; }

        [DataMember]
        public int Bitcoins { get; set; }

        [DataMember(Name = "Exp")]
        public int ExperiencePoints { get; set; }

        internal Base Base => DefendingLane.Target;

        internal Player(Lane defendingLane, Lane attackingLane) : this(50, defendingLane, attackingLane)
        {
        }

        private Player(int bitcoins, Lane defendingLane, Lane attackingLane)
        {
            Bitcoins = bitcoins;
            AttackingLane = attackingLane;
            DefendingLane = defendingLane;
        }
    }
}
