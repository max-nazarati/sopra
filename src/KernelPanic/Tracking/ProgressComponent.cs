using System;
using Autofac.Util;
using KernelPanic.Events;
using Newtonsoft.Json;

namespace KernelPanic.Tracking
{
    internal abstract class ProgressComponent : Disposable
    {
        [JsonIgnore] protected AchievementProgress Progress { get; private set; }
        [JsonProperty] internal Event.Id EventId { get; set; }

        private IDisposable mDisposable;

        protected ProgressComponent(Event.Id eventId)
        {
            EventId = eventId;
        }

        protected override void Dispose(bool disposing)
        {
            mDisposable?.Dispose();
            mDisposable = null;

            base.Dispose(disposing);
        }

        /// <summary>
        /// Connects this <see cref="ProgressComponent"/> with the <see cref="EventCenter"/>. If an <see cref="Event"/>
        /// with <see cref="Event.Kind"/> <see cref="EventId"/> occurs <see cref="Handle"/> will be called for this
        /// <see cref="ProgressComponent"/>.
        /// 
        /// <para>
        /// The <see cref="EventCenter"/> subscription can be disposed of with a call to <see cref="Dispose"/>.
        /// </para>
        /// </summary>
        /// <param name="progress">The progress where this is a component.</param>
        /// <param name="condition">If not <c>null</c> this function must return <c>true</c> for events to be passed to <see cref="Handle"/>.</param>
        /// <exception cref="InvalidOperationException">If this <see cref="ProgressComponent"/> is already connected.</exception>
        internal void Connect(AchievementProgress progress, Func<Event, bool> condition = null)
        {
            if (mDisposable != null || IsDisposed)
                throw new InvalidOperationException("Component is/was already connected.");

            Progress = progress;
            mDisposable = EventCenter.Default.Subscribe(EventId, Handle, condition);
        }

        protected abstract void Handle(Event @event);

        /// <summary>
        /// Compares the initial parameters of two <see cref="ProgressComponent"/>s.
        /// </summary>
        /// <param name="other">The component to compare <c>this</c> with.</param>
        /// <returns><c>true</c> if they are similar.</returns>
        internal abstract bool IsSimilar(ProgressComponent other);
    }

    internal sealed class CounterProgressComponent : ProgressComponent
    {
        [JsonProperty] private int Target { get; set; }
        [JsonProperty] private int Current { get; set; }
        
        /// <summary>
        /// If not <c>null</c> <see cref="Current"/> will be increased by the <see cref="int"/> value extracted from
        /// the <see cref="Event"/>. If <see cref="ExtractKey"/> is <c>null</c> it will be increased by one.
        /// </summary>
        [JsonProperty] internal Event.Key? ExtractKey { get; set; }
        
        /// <summary>
        /// The status to which <see cref="ProgressComponent.Progress"/> is set when <see cref="Current"/> reaches
        /// <see cref="Target"/>.
        /// </summary>
        [JsonProperty] internal Achievements.Status ResultingStatus { get; set; } = Achievements.Status.Unlocked;

        [JsonConstructor]
        internal CounterProgressComponent(Event.Id eventId, int target = 1) : base(eventId)
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
        [JsonProperty] private int Best { get; set; }
        [JsonProperty] private Event.Key ExtractKey { get; set; }

        [JsonConstructor]
        internal ComparisonProgressComponent(Event.Id eventId, Event.Key extractKey, int target)
            : base(eventId)
        {
            Target = target;
            EventId = eventId;
            ExtractKey = extractKey;
        }

        protected override void Handle(Event @event)
        {
            Best = Math.Max(Best, @event.Get<int>(ExtractKey));
            if (Best >= Target)
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