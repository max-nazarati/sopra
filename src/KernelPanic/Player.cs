using System.Runtime.Serialization;
using KernelPanic.Table;
using Newtonsoft.Json;

namespace KernelPanic
{
    [DataContract]
    internal sealed class Player
    {
        // private List<Upgrade> Upgrades;
        [DataMember]
        internal Lane AttackingLane { get; }
        [DataMember]
        internal Lane DefendingLane { get; }

        [DataMember]
        public int Bitcoins { get; set; }

        [DataMember(Name = "Exp")]
        public int ExperiencePoints { get; set; }

        internal Base Base => DefendingLane.Target;

        internal Player(Lane defendingLane, Lane attackingLane) : this(50, defendingLane, attackingLane)
        {
        }

        [JsonConstructor]
        private Player(int bitcoins, Lane defendingLane, Lane attackingLane)
        {
            Bitcoins = bitcoins;
            AttackingLane = attackingLane;
            DefendingLane = defendingLane;
        }
    }
}
