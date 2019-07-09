using System;
using System.Collections.Generic;
using Autofac.Util;
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
                    Progress = TrackingDictionary(Achievement.Lose1,
                        Achievement.Lose10,
                        Achievement.Win1,
                        Achievement.Win10,
                        Achievement.BitcoinAddict)
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
