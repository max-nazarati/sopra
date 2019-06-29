using System.Runtime.Serialization;
using KernelPanic.Table;
using KernelPanic.ArtificalIntelligence;
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

        private ArtificialPlayer mArtificalPlayer;

        [DataMember(Name = "Exp")]
        public int ExperiencePoints { get; set; }

        internal Base Base => DefendingLane.Target;
        public bool Human { get; } = true;

        internal Player(Lane defendingLane, Lane attackingLane, bool human=true) : this(9999, defendingLane, attackingLane)
        {
            if (!human)
            {
                mArtificalPlayer = new ArtificialPlayer();
                Human = false;
            }
        }

        [JsonConstructor]
        private Player(int bitcoins, Lane defendingLane, Lane attackingLane)
        {
            Bitcoins = bitcoins;
            AttackingLane = attackingLane;
            DefendingLane = defendingLane;
        }

        internal void Update()
        {
            if (!Human)
            {
                mArtificalPlayer.Update();
            }
        }
    }
}
