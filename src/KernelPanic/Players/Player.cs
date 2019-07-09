﻿using System.Collections.Generic;
using System.Runtime.Serialization;
using KernelPanic.Table;
using KernelPanic.Entities;
using KernelPanic.Events;
using KernelPanic.Upgrades;
using Newtonsoft.Json;

namespace KernelPanic.Players
{
    [DataContract]
    internal class Player: IPlayerDistinction
    {
        [DataMember]
        internal Lane AttackingLane { get; }
        [DataMember]
        internal Lane DefendingLane { get; }

        [DataMember(Name = "bitcoins")]
        private int mBitcoin;

        internal int Bitcoins
        {
            get => mBitcoin;
            set
            {
                if (mBitcoin == value)
                    return;

                mBitcoin = value;
                EventCenter.Default.Send(Event.BitcoinChanged(this));
            }
        }

        [DataMember]
        internal bool IncreasedBitcoins { get; private set; }

        [DataMember]
        private readonly List<Upgrade> mUpgrades = new List<Upgrade>();

        [DataMember(Name = "Exp")]
        public int ExperiencePoints { get; set; } = 5;

        internal Base Base => DefendingLane.Target;

        [DataMember]
        internal int FirefoxMaximum { get; private set; } = 1;

        [JsonConstructor]
        internal Player(Lane defendingLane, Lane attackingLane, int bitcoins)
        {
            Bitcoins = bitcoins;
            AttackingLane = attackingLane;
            DefendingLane = defendingLane;
        }

        #region Upgrades

        internal void AddUpgrade(Upgrade upgrade)
        {
            EventCenter.Default.Send(Event.UpgradeBought(this, upgrade));

            switch (upgrade.Kind)
            {
                case Upgrade.Id.IncreaseBitcoins:
                    IncreasedBitcoins = true;
                    return;

                case Upgrade.Id.AdditionalFirefox1:
                case Upgrade.Id.AdditionalFirefox2:
                    FirefoxMaximum++;
                    return;
            }

            mUpgrades.Add(upgrade);

            // Apply the new upgrade to all existing entities.
            foreach (var unit in AttackingLane.EntityGraph.Entities<Unit>())
                upgrade.Apply(unit);
            foreach (var building in DefendingLane.EntityGraph.Entities<Building>())
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
        public virtual T Select<T>(T ifActive, T ifPassive)
        {
            // A player is active. If it is passive it is of type ArtificialPlayer
            // where this function is overriden accordingly.
            return ifActive;
        }
    }
}
