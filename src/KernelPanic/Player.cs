using System.Collections.Generic;
using System.Runtime.Serialization;
using KernelPanic.Table;
using KernelPanic.ArtificialIntelligence;
using KernelPanic.Upgrades;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace KernelPanic
{
    [DataContract]
    internal sealed class Player
    {
        [DataMember]
        internal Lane AttackingLane { get; }
        [DataMember]
        internal Lane DefendingLane { get; }

        [DataMember]
        public int Bitcoins { get; set; }

        [DataMember]
        private readonly List<Upgrade> mUpgrades = new List<Upgrade>();

        private readonly ArtificialPlayer mArtificialPlayer;

        [DataMember(Name = "Exp")]
        public int ExperiencePoints { get; set; }

        internal Base Base => DefendingLane.Target;

        internal Player(Lane defendingLane, Lane attackingLane, bool human = true) : this(9999, defendingLane, attackingLane)
        {
            if (!human)
            {
                mArtificialPlayer = new ArtificialPlayer(this, DefendingLane.mSpriteManager, DefendingLane.mSounds);
            }
        }

        [JsonConstructor]
        private Player(int bitcoins, Lane defendingLane, Lane attackingLane)
        {
            Bitcoins = bitcoins;
            AttackingLane = attackingLane;
            DefendingLane = defendingLane;
        }

        internal void Update(GameTime gameTime)
        {
            mArtificialPlayer?.Update(gameTime);
        }

        internal void AddUpgrade(Upgrade upgrade)
        {
            mUpgrades.Add(upgrade);
        }
    }
}
