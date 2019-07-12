using System;
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

        internal Data TheData { get; }

        private CompositeDisposable mDisposable;

        private DateTime?[] UnlockTimeArray => TheData.UnlockTime ?? mParent.TheData.UnlockTime;

        internal AchievementPool(AchievementPool parent, Data? loadedData)
        {
            mParent = parent;
            TheData = loadedData ?? InitialData();

            if (TheData.Progress.Count == 0)
                return;
            
            mDisposable = new CompositeDisposable();
            mDisposable += EventCenter.Default.Subscribe(Event.Id.AchievementUnlocked,
                @event =>
                {
                    var achievement = @event.Get<Achievement>(Event.Key.Achievement);
                    if (TheData.RemoveProgress(achievement))
                        UnlockTimeArray[(int) achievement] = DateTime.Now;
                });

            mDisposable += EventCenter.Default.Subscribe(Event.Id.AchievementImpossible,
                @event =>
                {
                    var achievement = @event.Get<Achievement>(Event.Key.Achievement);
                    TheData.RemoveProgress(achievement);
                });
        }

        internal static AchievementPool LoadGlobal()
        {
            return new AchievementPool(null, StorageManager.LoadAchievements());
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
            var progress = new List<AchievementProgress>(achievements.Count);
            progress.AddRange(achievements
                .Where(achievement => !UnlockTimeArray[(int)achievement].HasValue)
                .Select(AchievementProgress.Track));

            progress.Sort(new AchievementProgress.Comparer());
            return progress;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            mDisposable.Dispose();
            foreach (var progress in TheData.Progress)
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
