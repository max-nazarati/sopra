using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Autofac.Util;
using KernelPanic.Entities;
using KernelPanic.Entities.Buildings;
using KernelPanic.Entities.Units;
using KernelPanic.Events;
using KernelPanic.Players;
using Newtonsoft.Json;

namespace KernelPanic.Tracking
{
    internal sealed class AchievementProgress : Disposable
    {
        [JsonProperty] internal Achievement Achievement { get; }
        [JsonProperty] internal Achievements.Status Status { get; private set; }

        [JsonProperty]
        internal ProgressComponent[] mComponents;

        #region Creation

        [JsonConstructor]
        private AchievementProgress(Achievement achievement)
        {
            Achievement = achievement;
        }

        [OnDeserialized]
        private void AfterDeserialization(StreamingContext context)
        {
            using (var iterator = mComponents.AsEnumerable().GetEnumerator())
            {
                AddComponents(new ComponentHelper(iterator));
            }
        }

        internal static AchievementProgress Track(Achievement achievement)
        {
            var progress = new AchievementProgress(achievement);
            var helper = new ComponentHelper(null);
            progress.AddComponents(helper);
            progress.mComponents = helper.ResultingComponents;
            return progress;
        }

        internal static AchievementProgress Untracked(Achievement achievement)
        {
            return new AchievementProgress(achievement);
        }

        #endregion

        #region Component Management

        private struct ComponentHelper
        {
            private readonly IEnumerator<ProgressComponent> mComponentIterator;
            private readonly List<ProgressComponent> mNewComponents;

            internal ProgressComponent[] ResultingComponents => mNewComponents.ToArray();

            internal ComponentHelper(IEnumerator<ProgressComponent> componentIterator)
            {
                mComponentIterator = componentIterator;
                mNewComponents = mComponentIterator == null ? new List<ProgressComponent>(4) : null;
            }

            internal ProgressComponent Add(ProgressComponent component)
            {
                if (mNewComponents != null)
                {
                    mNewComponents.Add(component);
                    return component;
                }

                mComponentIterator.MoveNext();
                
                // NOTE: If you get an exception here, you might have to delete your achievements save file.
                if (!(mComponentIterator.Current is ProgressComponent restored))
                    throw new InvalidOperationException("Adding more components than were restored.");
                if (!component.IsSimilar(restored))
                    throw new InvalidOperationException("Components mismatch.");

                return restored;
            }
        }

        private void AddComponents(ComponentHelper componentHelper)
        {
            switch (Achievement)
            {
                case Achievement.Win1:
                    componentHelper.Add(new CounterProgressComponent(this, Event.Id.GameWon)).Connect();
                    break;
                case Achievement.Win10:
                    componentHelper.Add(new CounterProgressComponent(this, Event.Id.GameWon, 10)).Connect();
                    break;
                case Achievement.Win100:
                    componentHelper.Add(new CounterProgressComponent(this, Event.Id.GameWon, 100)).Connect();
                    break;

                case Achievement.Lose1:
                    componentHelper.Add(new CounterProgressComponent(this, Event.Id.GameLost)).Connect();
                    break;
                case Achievement.Lose10:
                    componentHelper.Add(new CounterProgressComponent(this, Event.Id.GameLost, 10)).Connect();
                    break;
                case Achievement.Lose100:
                    componentHelper.Add(new CounterProgressComponent(this, Event.Id.GameLost, 100)).Connect();
                    break;

                case Achievement.AptGetUpgrade:
                    componentHelper
                        .Add(new CounterProgressComponent(this, Event.Id.UpgradeBought, 10) {ExtractKey = Event.Key.Price})
                        .Connect(IsActiveBuyer);
                    break;

                case Achievement.BitcoinAddict:
                    componentHelper
                        .Add(new ComparisonProgressComponent(this, 1100, Event.Id.BitcoinChanged, Event.Key.Price))
                        .Connect(IsActiveBuyer);
                    break;

                case Achievement.IronFortress:
                    componentHelper
                        .Add(new CounterProgressComponent(this, Event.Id.GameWon))
                        .Connect(Is100Percent);
                    break;

                case Achievement.HighInference:
                    componentHelper
                        .Add(new CounterProgressComponent(this, Event.Id.GameWon))
                        .Connect();
                    componentHelper
                        .Add(new CounterProgressComponent(this, Event.Id.BuildingPlaced) {ResultingStatus = Achievements.Status.Failed})
                        .Connect(PlacedNonWifi);
                    break;

                case Achievement.BugsFixed:
                    componentHelper
                        .Add(new CounterProgressComponent(this, Event.Id.KilledUnit, 3))
                        .Connect(IsEnemyBug);
                    break;

                case Achievement.EmptySlot:
                    componentHelper
                        .Add(new CounterProgressComponent(this, Event.Id.LoadEmptySlot))
                        .Connect();
                    break;

                case Achievement.NumberOfAchievements:
                    goto default;
                default:
                    throw new InvalidEnumArgumentException(nameof(Achievement), (int) Achievement, typeof(Achievement));
            }
        }

        /// <summary>
        /// Disconnects the components from the event center.
        /// </summary>
        private void DisposeComponents()
        {
            if (mComponents == null)
                return;

            foreach (var component in mComponents)
            {
                component.Dispose();
            }

            mComponents = null;
        }

        #endregion

        #region Conditions

        private static bool Is100Percent(Event @event)
        {
            return @event.Get<Player>(Event.Key.Winner).Base.Power == 100;
        }

        private static bool PlacedNonWifi(Event @event)
        {
            var building = @event.Get<Building>(Event.Key.Building);
            return IsActiveBuyer(@event) && !(building is WifiRouter);
        }

        private static bool IsActiveBuyer(Event @event)
        {
            return @event.IsActivePlayer(Event.Key.Buyer);
        }

        private static bool IsEnemyBug(Event @event)
        {
            return @event.IsActivePlayer(Event.Key.Defender) && @event.Get<Unit>(Event.Key.Unit) is Bug;
        }

        #endregion

        internal void SetStatus(Achievements.Status status)
        {
            if (Status == status)
                return;

            if (Status != Achievements.Status.Locked)
                throw new InvalidOperationException($"Can't go from {Status} to {status}");

            Status = status;
            DisposeComponents();

            EventCenter.Default.Send(status == Achievements.Status.Unlocked
                ? Event.AchievementUnlocked(Achievement)
                : Event.AchievementImpossible(Achievement));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                DisposeComponents();

            base.Dispose(disposing);
        }

        internal struct Comparer : IComparer<AchievementProgress>
        {
            public int Compare(AchievementProgress x, AchievementProgress y)
            {
                if (x?.Achievement is Achievement a && y?.Achievement is Achievement b)
                    return a.CompareTo(b);
                return (x == null).CompareTo(y == null);
            }
        }
    }
}