using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Accord.Statistics.Kernels;
using Autofac.Util;
using KernelPanic.Entities;
using KernelPanic.Entities.Buildings;
using KernelPanic.Entities.Units;
using KernelPanic.Events;
using KernelPanic.Players;
using KernelPanic.Serialization;
using Newtonsoft.Json;

namespace KernelPanic.Tracking
{
    internal enum Achievement
    {
        Win1,
        Win10,
        Win100,

        Lose1,
        Lose10,
        Lose100,

        AptGetUpgrade,
        BitcoinAddict,
        IronFortress,
        HighInference,
        BugsFixed,

        EmptySlot,
        
        NumberOfAchievements
    }

    internal static class Achievements
    {
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        internal static string Title(this Achievement achievement)
        {
            switch (achievement)
            {
                case Achievement.Win1:
                    return "First Victory!";
                case Achievement.Win10:
                    return "GG EASY";
                case Achievement.Win100:
                    return "Is Dis Tetris?";

                case Achievement.Lose1:
                    return "Unlucky Loss";
                case Achievement.Lose10:
                    return "Rekt";
                case Achievement.Lose100:
                    return "Complete Humiliation";

                case Achievement.AptGetUpgrade:
                    return "while true; DO sudo apt-get upgrade; DONE";
                case Achievement.BitcoinAddict:
                    return "Bitcoin Addiction";
                case Achievement.IronFortress:
                    return "Iron Fortress";
                case Achievement.HighInference:
                    return "High Inference";
                case Achievement.BugsFixed:
                    return "Fix your Code!";

                case Achievement.EmptySlot:
                    return "Fool!";

                case Achievement.NumberOfAchievements:
                    goto default;
                default:
                    throw new InvalidEnumArgumentException(nameof(achievement), (int) achievement, typeof(Achievement));
            }
        }

        internal static string Description(this Achievement achievement)
        {
            // TODO: Return correct descriptions.
            return achievement.ToString();
        }
    }

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
                        new ValueComparison(progress, Event.Id.BitcoinChanged, Event.Key.Price, IsActiveBuyer)
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
            mDisposable = EventCenter.Default.Subscribe(eventId, condition, Handle);
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

        internal ValueComparison(AchievementProgress progress, Event.Id eventId, Event.Key extractKey, Func<Event, bool> condition = null)
            : base(progress, eventId, condition)
        {
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

    internal sealed class AchievementTracker
    {
        [JsonProperty]
        private Dictionary<Achievement, AchievementProgress> mProgress;
        
        
    }

    internal sealed class AchievementPool : Disposable
    {
        internal struct Data
        {
            [JsonProperty] internal DateTime?[] UnlockTime { get; set; }

            [JsonProperty] internal Dictionary<Achievement, AchievementProgress> Progress { get; set; }
        }

        private readonly AchievementPool mParent;
        internal Data TheData { get; }

        internal AchievementPool(AchievementPool parent, Data? loadedData)
        {
            mParent = parent;

            if (loadedData is Data data)
            {
                TheData = data;
            }
            else if (parent == null)
            {
                TheData = new Data
                {
                    UnlockTime = new DateTime?[(int) Achievement.NumberOfAchievements],

                    // TODO: Complete this list.
                    Progress = TrackingDictionary(Achievement.Lose1, Achievement.Lose10, Achievement.Win1, Achievement.Win10)
                };
            }
            else
            {
                TheData = new Data
                {
                    Progress = TrackingDictionary(Achievement.HighInference, Achievement.BugsFixed)
                };
            }

            if (TheData.Progress.Count == 0)
                return;

            EventCenter.Default.Subscribe(Event.Id.AchievementUnlocked,
                @event =>
                {
                    var achievement = @event.Get<Achievement>(Event.Key.Achievement);
                    (TheData.UnlockTime ?? mParent.TheData.UnlockTime)[(int) achievement] = DateTime.Now;
                    TheData.Progress.Remove(achievement);
                });

            EventCenter.Default.Subscribe(Event.Id.AchievementImpossible,
                @event =>
                {
                    var achievement = @event.Get<Achievement>(Event.Key.Achievement);
                    TheData.Progress.Remove(achievement);
                });
        }

        internal static AchievementPool LoadGlobal()
        {
            return new AchievementPool(null, StorageManager.LoadAchievements());
        }

        private Dictionary<Achievement, AchievementProgress> TrackingDictionary(params Achievement[] achievements)
        {
            var dict = new Dictionary<Achievement, AchievementProgress>(achievements.Length);
            foreach (var achievement in achievements)
            {
                if (mParent?.TheData.UnlockTime[(int) achievement].HasValue ?? false)
                    continue;

                dict[achievement] = AchievementProgress.Track(achievement);
            }

            return dict;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing)
                return;

            foreach (var progress in TheData.Progress.Values)
            {
                progress.Dispose();
            }

            if (mParent == null)
                StorageManager.SaveAchievements(TheData);
        }

        internal IEnumerable<(string Title, string Description, string Time)> UserRepresentation
        {
            get
            {
                Achievement a = 0;
                foreach (var time in TheData.UnlockTime)
                {
                    var unlock = time is DateTime unlockTime ? unlockTime.ToString("dd.MM.yyyy, hh:mm") : "-";
                    yield return (a.Title(), a.Description(), unlock);
                    ++a;
                }
            }
        }
    }
}