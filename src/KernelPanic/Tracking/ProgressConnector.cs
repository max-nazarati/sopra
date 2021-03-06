using System;
using System.Collections.Generic;
using System.Linq;
using KernelPanic.Events;

namespace KernelPanic.Tracking
{
    internal struct ProgressConnector : IDisposable
    {
        private readonly AchievementProgress mProgress;
        private readonly IEnumerator<ProgressComponent> mComponentIterator;
        private readonly List<ProgressComponent> mNewComponents;

        internal ProgressConnector(AchievementProgress progress)
        {
            mProgress = progress;
            mComponentIterator = progress.Components?.AsEnumerable().GetEnumerator();
            mNewComponents = progress.Components == null ? new List<ProgressComponent>(4) : null;
        }

        void IDisposable.Dispose()
        {
            mComponentIterator?.Dispose();

            if (mNewComponents != null)
                mProgress.Components = mNewComponents.ToArray();
        }

        private ProgressComponent Add(ProgressComponent component)
        {
            if (mNewComponents != null)
            {
                mNewComponents.Add(component);
                return component;
            }

            // NOTE: If you get an exception here, you might have to delete your achievements save file.
            if (!mComponentIterator.MoveNext())
                throw new InvalidOperationException("Adding more components than were restored.");

            var restored = mComponentIterator.Current;
            if (restored != null && !restored.IsSimilar(component))
                throw new InvalidOperationException("Components mismatch.");

            return restored;
        }

        private void Connect(ProgressComponent component, Func<Event, bool> condition)
        {
            Add(component)?.Connect(mProgress, condition);
        }

        internal void ConnectCounter(Event.Id eventId,
            Event.Key? extractKey = null,
            int target = 1,
            Func<Event, bool> condition = null,
            bool success = true)
        {
            var component = new CounterProgressComponent(eventId, extractKey, target, success);
            Connect(component, condition);
        }

        internal void ConnectComparison(Event.Id eventId,
            Event.Key extractKey,
            int target,
            Func<Event, bool> condition = null)
        {
            Connect(new ComparisonProgressComponent(eventId, extractKey, target), condition);
        }
    }
}