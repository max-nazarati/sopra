using System;
using Autofac.Util;
using KernelPanic.Events;
using Newtonsoft.Json;

namespace KernelPanic.Tracking
{
    internal abstract class ProgressComponent : Disposable
    {
        [JsonProperty] protected AchievementProgress Progress { get; }

        [JsonProperty] internal Event.Id EventId { get; set; }

        private IDisposable mDisposable;

        protected ProgressComponent(AchievementProgress progress, Event.Id eventId)
        {
            Progress = progress;
            EventId = eventId;
        }

        protected override void Dispose(bool disposing)
        {
            mDisposable?.Dispose();
            mDisposable = null;

            base.Dispose(disposing);
        }

        internal void Connect(Func<Event, bool> condition = null)
        {
            mDisposable?.Dispose();
            mDisposable = EventCenter.Default.Subscribe(EventId, Handle, condition);
        }

        protected abstract void Handle(Event @event);

        internal abstract bool IsSimilar(ProgressComponent other);
    }

    internal sealed class CounterProgressComponent : ProgressComponent
    {
        [JsonProperty] private int Target { get; set; }
        [JsonProperty] private int Current { get; set; }
        [JsonProperty] internal Event.Key? ExtractKey { get; set; }
        [JsonProperty] internal Achievements.Status ResultingStatus { get; set; } = Achievements.Status.Unlocked;

        [JsonConstructor]
        internal CounterProgressComponent(AchievementProgress progress, Event.Id eventId, int target = 1) : base(progress, eventId)
        {
            Target = target;
        }

        protected override void Handle(Event @event)
        {
            Current += ExtractKey is Event.Key key ? @event.Get<int>(key) : 1;
            if (Current >= Target)
                Progress.SetStatus(ResultingStatus);
        }

        internal override bool IsSimilar(ProgressComponent other)
        {
            return other is CounterProgressComponent counter
                   && Target == counter.Target
                   && ExtractKey == counter.ExtractKey
                   && ResultingStatus == counter.ResultingStatus;
        }
    }

    internal sealed class ComparisonProgressComponent : ProgressComponent
    {
        [JsonProperty] private int Target { get; set; }
        [JsonProperty] private Event.Key ExtractKey { get; set; }

        internal ComparisonProgressComponent(AchievementProgress progress, int target, Event.Id eventId, Event.Key extractKey)
            : base(progress, eventId)
        {
            Target = target;
            EventId = eventId;
            ExtractKey = extractKey;
        }

        protected override void Handle(Event @event)
        {
            if (@event.Get<int>(ExtractKey) >= Target)
                Progress.SetStatus(Achievements.Status.Unlocked);
        }

        internal override bool IsSimilar(ProgressComponent other)
        {
            return other is ComparisonProgressComponent comparison
                   && Target == comparison.Target
                   && ExtractKey == comparison.ExtractKey;
        }
    }
}