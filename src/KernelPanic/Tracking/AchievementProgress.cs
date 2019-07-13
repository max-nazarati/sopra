using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Autofac.Util;
using KernelPanic.Events;
using Newtonsoft.Json;

namespace KernelPanic.Tracking
{
    internal sealed class AchievementProgress : Disposable, IProgress<ProgressComponent>
    {
        [JsonProperty] internal Achievement Achievement { get; }
        [JsonProperty] internal Achievements.Status Status { get; private set; }

        internal ProgressComponent[] Components
        {
            get => mComponents;
            set
            {
                if (mComponents != null)
                    throw new InvalidOperationException("mComponents is already set.");
                mComponents = value;
            }
        }

        [JsonProperty]
        private ProgressComponent[] mComponents;

        #region Creation

        [JsonConstructor]
        private AchievementProgress(Achievement achievement)
        {
            Achievement = achievement;
        }

        [OnDeserialized]
        private void AfterDeserialization(StreamingContext context)
        {
            Connect();
        }

        internal static AchievementProgress Track(Achievement achievement)
        {
            var progress = new AchievementProgress(achievement);
            progress.Connect();
            return progress;
        }

        private void Connect()
        {
            using (var componentHelper = new ProgressConnector(this))
                Achievement.ConnectComponents(componentHelper);
        }

        internal static AchievementProgress Untracked(Achievement achievement)
        {
            return new AchievementProgress(achievement);
        }

        #endregion

        #region Component Feedback

        void IProgress<ProgressComponent>.SetFailed()
        {
            SetStatus(Achievements.Status.Failed);
        }

        void IProgress<ProgressComponent>.SetSuccess(ProgressComponent component)
        {
            var index = Array.IndexOf(mComponents, component);
            if (index < 0)
                throw new InvalidOperationException("Given component is  not a part of this achievement");
            mComponents[index] = null;
            
            if (mComponents.All(c => c == null || !c.Positive))
                SetStatus(Achievements.Status.Unlocked);
        }
        
        private void SetStatus(Achievements.Status status)
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

        #endregion

        /// <summary>
        /// Disconnects the components from the event center.
        /// </summary>
        private void DisposeComponents()
        {
            if (mComponents == null)
                return;

            foreach (var component in mComponents)
            {
                component?.Dispose();
            }

            mComponents = null;
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