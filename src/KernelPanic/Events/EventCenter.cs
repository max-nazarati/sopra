using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KernelPanic.Data;

namespace KernelPanic.Events
{
    internal sealed class EventCenter
    {
        private sealed class HandlerBag : IEnumerable<Action<Event>>
        {
            internal struct Token : IDisposable
            {
                private readonly ulong mKey;
                private readonly HandlerBag mBag;

                internal Token(ulong key, HandlerBag bag)
                {
                    mKey = key;
                    mBag = bag;
                }

                void IDisposable.Dispose()
                {
                    mBag.mHandlers.Remove(mKey);
                }
            }

            private ulong mNextKey;
            private readonly Dictionary<ulong, Action<Event>> mHandlers = new Dictionary<ulong, Action<Event>>();

            internal Token Add(Action<Event> handler)
            {
                var key = mNextKey++;
                if (key > mNextKey)
                {
                    // We had a wrap-around. Although this last key would still be usable it's quite likely that we
                    // would want to add another event handler for this event at some point.
                    throw new InvalidOperationException("Too many event handlers registered.");
                }

                mHandlers.Add(key, handler);
                return new Token(key, this);
            }

            public IEnumerator<Action<Event>> GetEnumerator()
            {
                return mHandlers.Values.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private readonly Dictionary<Event.Id, HandlerBag> mSubscribers = new Dictionary<Event.Id, HandlerBag>();

        private byte mBufferIndex;
        private readonly List<Event>[] mEventBuffers = {
            new List<Event>(),
            new List<Event>()
        };

        #region Singleton

        private EventCenter()
        {
        }

        internal static EventCenter Default { get; } = new EventCenter();

        #endregion

        #region Subscribing

        /// <summary>
        /// Subscribes to the events with id <paramref name="id"/>. Every time such an event occurs,
        /// <paramref name="handler"/> is invoked.
        /// </summary>
        /// <param name="id">The <see cref="Event.Id"/> of the event to be notified of.</param>
        /// <param name="handler">The function to invoke when such an event occurs.</param>
        /// <returns>An <see cref="IDisposable"/> which unsubscribes <paramref name="handler"/> from the event stream.</returns>
        internal IDisposable Subscribe(Event.Id id, Action<Event> handler)
        {
            if (mSubscribers.TryGetValue(id, out var subscribers))
                return subscribers.Add(handler);

            var bag = new HandlerBag();
            mSubscribers[id] = bag;
            return bag.Add(handler);
        }

        internal IDisposable Subscribe(Event.Id id, Func<Event, bool> condition, Action<Event> handler)
        {
            return Subscribe(id, GuardHandler(condition, handler));
        }

        /// <summary>
        /// Subscribes to the events with an id in <paramref name="ids"/>. Every time such an event occurs,
        /// <paramref name="handler"/> is invoked.
        /// </summary>
        /// <param name="ids">A list of <see cref="Event.Id"/> of the events to be notified of.</param>
        /// <param name="handler">The function to invoke when such an event occurs.</param>
        /// <returns>An <see cref="IDisposable"/> which unsubscribes <paramref name="handler"/> from the event stream.</returns>
        internal IDisposable Subscribe(IEnumerable<Event.Id> ids, Action<Event> handler)
        {
            return ids.Aggregate(new CompositeDisposable(), (current, id) => current + Subscribe(id, handler));
        }

        internal IDisposable Subscribe(IEnumerable<Event.Id> ids, Func<Event, bool> condition, Action<Event> handler)
        {
            var realHandler = GuardHandler(condition, handler);
            return Subscribe(ids, realHandler);
        }

        /// <summary>
        /// Creates a new <see cref="Action{T}"/> which only passes the value to <paramref name="handler"/> if
        /// <paramref name="condition"/> returns <c>true</c>. If <paramref name="condition"/> is <c>null</c>
        /// <paramref name="handler"/> will be returned unmodified.
        /// </summary>
        /// <param name="condition">Used to determine if <paramref name="handler"/> should be called.</param>
        /// <param name="handler">Action which is called if <paramref name="condition"/> returned <c>true</c>.</param>
        /// <returns>An action.</returns>
        private static Action<Event> GuardHandler(Func<Event, bool> condition, Action<Event> handler)
        {
            if (condition == null)
                return handler;

            return @event =>
            {
                if (condition(@event))
                    handler(@event);
            };
        }

        #endregion

        #region Sending

        /// <summary>
        /// Queues <paramref name="event"/> for delivering during the next call to <see cref="Run"/>.
        /// </summary>
        /// <param name="event">The <see cref="Event"/> to queue.</param>
        internal void Send(Event @event)
        {
            mEventBuffers[mBufferIndex].Add(@event);
        }

        /// <summary>
        /// Delivers all <see cref="Event"/>s passed to <see cref="Send"/> since the last call to <see cref="Run"/>.
        /// </summary>
        internal void Run()
        {
            // Process events as long as new events are sent in response to us delivering events.
            for (var buffer = mEventBuffers[mBufferIndex]; buffer.Count > 0; buffer = mEventBuffers[mBufferIndex])
            {
                // mEventBuffers contains two elements, XOR with 1 switch mBufferIndex between 0 and 1.
                mBufferIndex ^= 1;

                foreach (var @event in buffer)
                {
                    if (!mSubscribers.TryGetValue(@event.Kind, out var handlers))
                        continue;

                    foreach (var handler in new List<Action<Event>>(handlers))
                    {
                        handler(@event);
                    }
                }

                buffer.Clear();
            }
        }

        #endregion
    }
}