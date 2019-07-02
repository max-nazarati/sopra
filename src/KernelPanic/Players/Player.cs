using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using KernelPanic.Table;
using KernelPanic.ArtificialIntelligence;
using KernelPanic.Entities;
using KernelPanic.Upgrades;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace KernelPanic.Players
{
    [DataContract]
    internal sealed class Player: IPlayerDistinction
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
        public int ExperiencePoints { get; set; } = 5;

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

        #region Upgrades

        internal void AddUpgrade(Upgrade upgrade)
        {
            mUpgrades.Add(upgrade);

            // Apply the new upgrade to all existing entities.
            foreach (var unit in AttackingLane.EntityGraph.OfType<Unit>())
                upgrade.Apply(unit);
            foreach (var building in DefendingLane.EntityGraph.OfType<Building>())
                upgrade.Apply(building);
        }

        internal void ApplyUpgrades(Entity entity)
        {
            foreach (var upgrade in mUpgrades)
            {
                upgrade.Apply(entity);
            }
        }

        #endregion

        /// <inheritdoc />
        public T Select<T>(T ifActive, T ifPassive)
        {
            return mArtificialPlayer == null ? ifActive : ifPassive;
        }
    }
}
