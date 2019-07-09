using System;
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
        [JsonProperty] internal AchievementStatus Status { get; private set; }

        [JsonProperty]
        internal AchievementProgressComponent[] mComponents;

        private AchievementProgress(Achievement achievement)
        {
            Achievement = achievement;
        }

        [OnDeserialized]
        private void AfterDeserialization(StreamingContext context)
        {
            // TODO: Re-track the events.
        }

        internal static AchievementProgress Track(Achievement achievement)
        {
            var progress = new AchievementProgress(achievement);

            switch (achievement)
            {
                case Achievement.Win1:
                    progress.mComponents = new AchievementProgressComponent[]
                    {
                        new EventCounter(progress, Event.Id.GameWon) 
                    };
                    break;

                case Achievement.Win10:
                    progress.mComponents = new AchievementProgressComponent[]
                    {
                        new EventCounter(progress, Event.Id.GameWon, count: 10) 
                    };
                    break;

                case Achievement.Win100:
                    progress.mComponents = new AchievementProgressComponent[]
                    {
                        new EventCounter(progress, Event.Id.GameWon, count: 100) 
                    };
                    break;

                case Achievement.Lose1:
                    progress.mComponents = new AchievementProgressComponent[]
                    {
                        new EventCounter(progress, Event.Id.GameLost) 
                    };
                    break;

                case Achievement.Lose10:
                    progress.mComponents = new AchievementProgressComponent[]
                    {
                        new EventCounter(progress, Event.Id.GameLost, count: 10) 
                    };
                    break;

                case Achievement.Lose100:
                    progress.mComponents = new AchievementProgressComponent[]
                    {
                        new EventCounter(progress, Event.Id.GameLost, count: 100) 
                    };
                    break;

                case Achievement.AptGetUpgrade:
                    progress.mComponents = new AchievementProgressComponent[]
                    {
                        new EventCounter(progress, Event.Id.UpgradeBought, count: 10)
                        {
                            ExtractKey = Event.Key.Price
                        }
                    };
                    break;

                case Achievement.BitcoinAddict:
                    progress.mComponents = new AchievementProgressComponent[]
                    {
                        new ValueComparison(progress, 1100, Event.Id.BitcoinChanged, Event.Key.Price, IsActiveBuyer)
                    };
                    break;

                case Achievement.IronFortress:
                    progress.mComponents = new AchievementProgressComponent[]
                    {
                        new EventCounter(progress, Event.Id.GameWon, Is100Percent) 
                    };
                    break;

                case Achievement.HighInference:
                    progress.mComponents = new AchievementProgressComponent[]
                    {
                        new EventCounter(progress, Event.Id.GameWon),
                        new EventCounter(progress, Event.Id.BuildingPlaced, PlacedNonWifi)
                        {
                            ResultingStatus = AchievementStatus.Failed
                        }
                    };
                    break;

                case Achievement.BugsFixed:
                    progress.mComponents = new AchievementProgressComponent[]
                    {
                        new EventCounter(progress, Event.Id.KilledUnit, IsEnemyBug, 3)
                    };
                    break;

                case Achievement.EmptySlot:
                    progress.mComponents = new AchievementProgressComponent[]
                    {
                        new EventCounter(progress, Event.Id.LoadEmptySlot)
                    };
                    break;

                case Achievement.NumberOfAchievements:
                    goto default;
                default:
                    throw new ArgumentOutOfRangeException(nameof(achievement), achievement, null);
            }

            return progress;
        }

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
            return IsActivePlayer(@event, Event.Key.Buyer);
        }

        private static bool IsActivePlayer(Event @event, Event.Key key)
        {
            return @event.Get<Player>(key).Select(true, false);
        }

        private static bool IsEnemyBug(Event @event)
        {
            var unit = @event.Get<Unit>(Event.Key.Unit);
            return IsActivePlayer(@event, Event.Key.Defender) && unit is Bug;
        }

        #endregion

        internal void SetStatus(AchievementStatus status)
        {
            if (Status == status)
                return;
                
            if (Status != AchievementStatus.Locked)
                throw new InvalidOperationException($"Can't go from {Status} to {status}");
            
            Status = status;
            DisposeComponents();

            EventCenter.Default.Send(status == AchievementStatus.Unlocked
                ? Event.AchievementUnlocked(Achievement)
                : Event.AchievementImpossible(Achievement));
        }

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
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                DisposeComponents();

            base.Dispose(disposing);
        }
    }

    internal abstract class AchievementProgressComponent : Disposable
    {
        [JsonProperty] protected AchievementProgress Progress { get; }

        [JsonProperty] internal Event.Id EventId { get; set; }

        private IDisposable mDisposable;

        protected AchievementProgressComponent(AchievementProgress progress, Event.Id eventId, Func<Event, bool> condition)
        {
            Progress = progress;
            EventId = eventId;
            mDisposable = EventCenter.Default.Subscribe(eventId, Handle, condition);
        }

        protected override void Dispose(bool disposing)
        {
            mDisposable?.Dispose();
            mDisposable = null;

            base.Dispose(disposing);
        }

        protected abstract void Handle(Event @event);
    }

    internal sealed class EventCounter : AchievementProgressComponent
    {
        [JsonProperty]
        private int mTarget;

        [JsonProperty]
        private int mCurrent;

        [JsonProperty] internal Event.Key? ExtractKey { get; set; }

        [JsonProperty] internal AchievementStatus ResultingStatus { get; set; } = AchievementStatus.Unlocked;

        internal EventCounter(AchievementProgress progress, Event.Id eventId, Func<Event, bool> condition = null, int count = 1)
            : base(progress, eventId, condition)
        {
            mTarget = count;
        }

        protected override void Handle(Event @event)
        {
            mCurrent += ExtractKey is Event.Key key ? @event.Get<int>(key) : 1;
            if (mCurrent >= mTarget)
                Progress.SetStatus(ResultingStatus);
        }
    }

    internal sealed class ValueComparison : AchievementProgressComponent
    {
        [JsonProperty]
        private int mTarget;

        [JsonProperty]
        private Event.Key mExtractKey;

        internal ValueComparison(AchievementProgress progress, int target, Event.Id eventId, Event.Key extractKey, Func<Event, bool> condition = null)
            : base(progress, eventId, condition)
        {
            mTarget = target;
            EventId = eventId;
            mExtractKey = extractKey;
        }

        protected override void Handle(Event @event)
        {
            if (@event.Get<int>(mExtractKey) >= mTarget)
                Progress.SetStatus(AchievementStatus.Unlocked);
        }
    }

    internal enum AchievementStatus
    {
        Locked, Unlocked, Failed
    }
}