using System;
using Autofac.Util;
using KernelPanic.Data;
using KernelPanic.Events;
using Newtonsoft.Json;

namespace KernelPanic.Tracking
{
    internal interface IProgress<in T>
    {
        void SetFailed();
        void SetSuccess(T component);
    }

    internal abstract class ProgressComponent : Disposable
    {
        [JsonIgnore] private IProgress<ProgressComponent> Progress { get; set; }
        [JsonProperty] internal Event.Id EventId { get; }
        [JsonProperty] internal bool Positive { get; }

        private CompositeDisposable mDisposable;
        internal bool Enabled { get; set; } = true;

        protected ProgressComponent(Event.Id eventId, bool positive)
        {
            EventId = eventId;
            Positive = positive;

            var eventCenter = EventCenter.Default;
            mDisposable += eventCenter.Subscribe(Event.Id.TechDemoStarted, e => Enabled = false);
            mDisposable += eventCenter.Subscribe(Event.Id.TechDemoClosed, e => Enabled = true);
        }

        protected override void Dispose(bool disposing)
        {
            mDisposable.Dispose();
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
        internal void Connect(IProgress<ProgressComponent> progress, Func<Event, bool> condition = null)
        {
            Progress = progress;
            mDisposable += EventCenter.Default.Subscribe(EventId, MaybeHandle, condition);
        }

        private void MaybeHandle(Event @event)
        {
            if (Enabled)
                Handle(@event);
        }

        protected abstract void Handle(Event @event);

        protected void Completed()
        {
            Dispose();

            if (Positive)
            {
                Progress.SetSuccess(this);
            }
            else
            {
                Progress.SetFailed();
            }
        }

        /// <summary>
        /// Compares the initial parameters of two <see cref="ProgressComponent"/>s.
        /// </summary>
        /// <param name="other">The component to compare <c>this</c> with.</param>
        /// <returns><c>true</c> if they are similar.</returns>
        internal virtual bool IsSimilar(ProgressComponent other)
        {
            return EventId == other.EventId && Positive == other.Positive;
        }
    }

    internal sealed class CounterProgressComponent : ProgressComponent
    {
        [JsonProperty] private int Target { get; }
        [JsonProperty] private int Current { get; set; }
        
        /// <summary>
        /// If not <c>null</c> <see cref="Current"/> will be increased by the <see cref="int"/> value extracted from
        /// the <see cref="Event"/>. If <see cref="ExtractKey"/> is <c>null</c> it will be increased by one.
        /// </summary>
        [JsonProperty] private Event.Key? ExtractKey { get; }

        [JsonConstructor]
        internal CounterProgressComponent(Event.Id eventId, Event.Key? extractKey, int target, bool positive) : base(eventId, positive)
        {
            Target = target;
            ExtractKey = extractKey;
        }

        protected override void Handle(Event @event)
        {
            Current += ExtractKey is Event.Key key ? @event.Get<int>(key) : 1;
            if (Current >= Target)
                Completed();
        }

        internal override bool IsSimilar(ProgressComponent other)
        {
            return other is CounterProgressComponent counter
                   && Target == counter.Target
                   && ExtractKey == counter.ExtractKey
                   && base.IsSimilar(other);
        }
    }

    internal sealed class ComparisonProgressComponent : ProgressComponent
    {
        [JsonProperty] private int Target { get; set; }
        [JsonProperty] private int Best { get; set; }
        [JsonProperty] private Event.Key ExtractKey { get; set; }

        [JsonConstructor]
        internal ComparisonProgressComponent(Event.Id eventId, Event.Key extractKey, int target)
            : base(eventId, true)
        {
            Target = target;
            ExtractKey = extractKey;
        }

        protected override void Handle(Event @event)
        {
            Best = Math.Max(Best, @event.Get<int>(ExtractKey));
            if (Best >= Target)
                Completed();
        }

        internal override bool IsSimilar(ProgressComponent other)
        {
            return other is ComparisonProgressComponent comparison
                   && Target == comparison.Target
                   && ExtractKey == comparison.ExtractKey
                   && base.IsSimilar(other);
        }
    }
}