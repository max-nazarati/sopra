using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autofac.Util;
using KernelPanic.Data;
using KernelPanic.Events;
using KernelPanic.Serialization;
using Newtonsoft.Json;

namespace KernelPanic.Tracking
{
    internal sealed class AchievementPool : Disposable
    {
        internal struct Data
        {
            [JsonProperty] internal DateTime?[] UnlockTime { get; set; }
            [JsonProperty] internal List<AchievementProgress> Progress { get; set; }

            internal bool RemoveProgress(Achievement achievement)
            {
                var index = Progress.BinarySearch(
                    AchievementProgress.Untracked(achievement),
                    new AchievementProgress.Comparer());

                if (index < 0)
                    return false;

                Progress.RemoveAt(index);
                return true;
            }
        }

        private readonly AchievementPool mParent;

        internal Data AchievementData { get; }

        private CompositeDisposable mDisposable;

        private DateTime?[] UnlockTimeArray => AchievementData.UnlockTime ?? mParent.AchievementData.UnlockTime;

        #region Creation

        internal static AchievementPool LoadGlobal()
        {
            return new AchievementPool(null, StorageManager.LoadAchievements());
        }

        static AchievementPool()
        {
            EventCenter.Default.Subscribe(Event.Id.CloseAchievementPool,
                @event => @event.Get<AchievementPool>(Event.Key.AchievementPool).Dispose());
        }

        internal AchievementPool(AchievementPool parent, Data? loadedData)
        {
            mParent = parent;
            AchievementData = loadedData ?? InitialData();

            if (AchievementData.Progress.Count == 0)
                return;
            
            mDisposable = new CompositeDisposable();
            mDisposable += EventCenter.Default.Subscribe(Event.Id.AchievementUnlocked,
                @event =>
                {
                    var achievement = @event.Get<Achievement>(Event.Key.Achievement);
                    if (AchievementData.RemoveProgress(achievement))
                        UnlockTimeArray[(int) achievement] = DateTime.Now;
                });

            mDisposable += EventCenter.Default.Subscribe(Event.Id.AchievementImpossible,
                @event =>
                {
                    var achievement = @event.Get<Achievement>(Event.Key.Achievement);
                    AchievementData.RemoveProgress(achievement);
                });
        }

        private Data InitialData()
        {
            return new Data
            {
                UnlockTime = mParent == null ? new DateTime?[Achievements.Count] : null,
                Progress = InitialProgress(mParent == null ? Achievements.GloballyTracked : Achievements.PerGame)
            };
        }

        private List<AchievementProgress> InitialProgress(IReadOnlyCollection<Achievement> achievements)
        {
            // UnlockTimeArray is still null here when mParent is null because this function is called from InitialData.
            bool StillLocked(Achievement achievement) =>
                mParent == null || !UnlockTimeArray[(int) achievement].HasValue;

            var progress = new List<AchievementProgress>(achievements.Count);
            progress.AddRange(achievements.Where(StillLocked).Select(AchievementProgress.Track));

            progress.Sort(new AchievementProgress.Comparer());
            return progress;
        }
        
        #endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            // Disconnect ourselves from the event center.
            mDisposable.Dispose();

            // Save the achievement progress before we dispose of the progress.
            if (mParent == null)
                StorageManager.SaveAchievements(AchievementData);

            foreach (var progress in AchievementData.Progress)
            {
                progress.Dispose();
            }
        }

        #region User Representation

        internal UserRepresentationCollection UserRepresentation =>
            new UserRepresentationCollection(this);
        
        internal struct UserRepresentationValue
        {
            public UserRepresentationValue(string title, string description, string value)
            {
                Title = title;
                Description = description;
                Value = value;
            }

            internal string Title { get; }
            internal string Description { get; }
            internal string Value { get; }
        }

        internal struct UserRepresentationCollection : IReadOnlyList<UserRepresentationValue>
        {
            private readonly AchievementPool mAchievementPool;

            internal UserRepresentationCollection(AchievementPool achievementPool)
            {
                mAchievementPool = achievementPool;
            }

            public IEnumerator<UserRepresentationValue> GetEnumerator()
            {
                var self = this;
                return Enumerable.Range(0, Count).Select(index => self[index]).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public int Count => mAchievementPool.AchievementData.UnlockTime.Length;

            public UserRepresentationValue this[int index]
            {
                get
                {
                    var achievement = (Achievement) index;
                    var unlockDate = mAchievementPool.AchievementData.UnlockTime[index];
                    var value = unlockDate?.ToString("dd.MM.yyyy, HH:mm") ?? "-";
                    return new UserRepresentationValue(achievement.Title(), achievement.Description(), value);
                }
            }
        }

        #endregion
    }
}
