using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using KernelPanic.Events;
using KernelPanic.Players;
using KernelPanic.Serialization;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace KernelPanic.Tracking
{
    internal sealed class Statistics : IDisposable
    {
        internal struct Data
        {
            [JsonProperty] internal ulong NumberOfWins { get; set; }
            [JsonProperty] internal ulong NumberOfLoses { get; set; }

            [JsonProperty] internal ulong DamageDealt { get; set; }
            [JsonProperty] internal ulong NumberOfKilledUnits { get; set; }

            [JsonProperty] internal ulong AttackInvestments { get; set; }
            [JsonProperty] internal ulong DefenceInvestments { get; set; }
            [JsonProperty] internal ulong UpgradeInvestments { get; set; }

            [JsonProperty] internal TimeSpan OverallPlayTime { get; set; }
        }

        private Data mData;

        internal Statistics()
        {
            mData = StorageManager.LoadStatistics().GetValueOrDefault();

            var eventCenter = EventCenter.Default;
            var isDefender = IsActive(Event.Key.Defender);
            var isBuyer = IsActive(Event.Key.Buyer);

            eventCenter.Subscribe(Event.Id.GameWon, e => mData.NumberOfWins++);
            eventCenter.Subscribe(Event.Id.GameLost, e => mData.NumberOfLoses++);

            eventCenter.Subscribe(Event.Id.DamagedUnit, isDefender,
                e => mData.DamageDealt += (ulong) e.Get<int>(Event.Key.Damage));
            eventCenter.Subscribe(Event.Id.KilledUnit, isDefender,
                e => mData.NumberOfKilledUnits++);

            eventCenter.Subscribe(Event.Id.BoughtUnit, isBuyer,
                e => mData.AttackInvestments += (ulong) e.Get<int>(Event.Key.Price));
            eventCenter.Subscribe(new[] {Event.Id.BuildingPlaced, Event.Id.BuildingImproved}, isBuyer,
                e => mData.DefenceInvestments += (ulong) e.Get<int>(Event.Key.Price));
            eventCenter.Subscribe(Event.Id.UpgradeBought, isBuyer,
                e => mData.UpgradeInvestments += (ulong) e.Get<int>(Event.Key.Price));
        }

        private static Func<Event, bool> IsActive(Event.Key key)
        {
            return @event => @event.Get<Player>(key).Select(true, false);
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

        [SuppressMessage("ReSharper", "StringLiteralTypo", Justification = "Strings are in German.")]
        internal IEnumerable<(string title, string value)> UserRepresentation =>
            new[]
            {
                Get("Siege", d => d.NumberOfWins),
                Get("Niederlagen", d => d.NumberOfLoses),
                Get("Verursachter Schaden", d => d.DamageDealt),
                Get("Besiegte Einheiten", d => d.NumberOfKilledUnits),
                Get("Investitionen in den Angriff", d => d.AttackInvestments + " BTC"),
                Get("Investitionen in die Verteidigung", d => d.DefenceInvestments + " BTC"),
                Get("Investitionen in Upgrades", d => d.UpgradeInvestments + " EP"),
                Get("Spielzeit", d => d.OverallPlayTime, "hh':'mm':'ss")
            };

        private (string, string) Get<T>(string title, Func<Data, T> f, string format = null)
            where T : IFormattable
        {
            return (title, f(mData).ToString(format, null));
        }

        private (string, string) Get(string title, Func<Data, string> f)
        {
            return (title, f(mData));
        }
    }
}