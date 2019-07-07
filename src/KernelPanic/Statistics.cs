using System;
using KernelPanic.Events;
using KernelPanic.Serialization;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace KernelPanic
{
    internal sealed class Statistics : IDisposable
    {
        internal struct Data
        {
            [JsonProperty] internal int NumberOfWins { get; set; }
            [JsonProperty] internal int NumberOfLoses { get; set; }

            [JsonProperty] internal int DamageDealt { get; set; }
            [JsonProperty] internal int NumberOfKilledUnits { get; set; }

            [JsonProperty] internal int AttackInvestments { get; set; }
            [JsonProperty] internal int DefenceInvestments { get; set; }
            [JsonProperty] internal int UpgradeInvestments { get; set; }

            [JsonProperty] internal TimeSpan OverallPlayTime { get; set; }
        }

        private Data mData;

        internal Statistics()
        {
            mData = StorageManager.LoadStatistics().GetValueOrDefault();

            var eventCenter = EventCenter.Default;

            eventCenter.Subscribe(Event.Id.GameWon, e => mData.NumberOfWins++);
            eventCenter.Subscribe(Event.Id.GameLost, e => mData.NumberOfLoses++);

            eventCenter.Subscribe(Event.Id.DamagedUnit, e => mData.DamageDealt += e.Get<int>(Event.Key.Damage));
            eventCenter.Subscribe(Event.Id.KilledUnit, e => mData.NumberOfKilledUnits++);

            eventCenter.Subscribe(Event.Id.BoughtUnit,
                e => mData.AttackInvestments += e.Get<int>(Event.Key.Price));
            eventCenter.Subscribe(new[] {Event.Id.BuildingPlaced, Event.Id.BuildingImproved},
                e => mData.DefenceInvestments += e.Get<int>(Event.Key.Price));
            eventCenter.Subscribe(Event.Id.UpgradeBought,
                e => mData.UpgradeInvestments += e.Get<int>(Event.Key.Price));
        }

        public void Dispose()
        {
            StorageManager.SaveStatistics(mData);
        }

        internal void Update(GameTime gameTime)
        {
            mData.OverallPlayTime += gameTime.ElapsedGameTime;
        }

        internal void Reset()
        {
            mData = new Data();
        }
    }
}