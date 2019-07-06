using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KernelPanic.Events
{
    internal sealed class EventCenter
    {
        private sealed class HandlerBag : IEnumerable<Action<Event>>
        {
            internal struct Token : IDisposable
            {
                private readonly uint mKey;
                private readonly HandlerBag mBag;

                internal Token(uint key, HandlerBag bag)
                {
                    mKey = key;
                    mBag = bag;
                }

                void IDisposable.Dispose()
                {
                    mBag.mHandlers.Remove(mKey);
                }
            }
            
            internal struct MultiToken : IDisposable
            {
                private List<IDisposable> mTokens;

                internal MultiToken(List<IDisposable> tokens)
                {
                    mTokens = tokens;
                }

                void IDisposable.Dispose()
                {
                    if (mTokens == null)
                        return;

                    foreach (var token in mTokens) using (token)
                    {
                        // The tokens are automatically disposed.
                    }

                    mTokens = null;
                }
            }

            private uint mNextKey;
            private readonly Dictionary<uint, Action<Event>> mHandlers = new Dictionary<uint, Action<Event>>();

            internal Token Add(Action<Event> handler)
            {
                var key = mNextKey++;
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

        private readonly List<Event> mEventsBuffer = new List<Event>();

        #region Singleton

        private EventCenter()
        {
        }

        private static readonly EventCenter sDefaultCenter = new EventCenter();
        internal static EventCenter Default => sDefaultCenter;

        #endregion

        #region Subscribing

        /// <summary>
        /// Subscribes to the events with id <paramref name="id"/>. Every time such an event occurs,
        /// <paramref name="handler"/> is invoked.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="handler"></param>
        /// <returns>An <see cref="IDisposable"/> which unsubscribes <paramref name="handler"/> from the event stream.</returns>
        internal IDisposable Subscribe(Event.Id id, Action<Event> handler)
        {
            if (mSubscribers.TryGetValue(id, out var subscribers))
                return subscribers.Add(handler);

            var bag = new HandlerBag();
            mSubscribers[id] = bag;
            return bag.Add(handler);
        }

        internal IDisposable Subscribe(IEnumerable<Event.Id> ids, Action<Event> handler)
        {
            var tokens = ids.Select(id => Subscribe(id, handler)).ToList();
            return new HandlerBag.MultiToken(tokens);
        }

        #endregion

        #region Sending

        /// <summary>
        /// Queues <paramref name="event"/> for delivering during the next call to <see cref="Run"/>.
        /// </summary>
        /// <param name="event">The <see cref="Event"/> to queue.</param>
        internal void Send(Event @event)
        {
            mEventsBuffer.Add(@event);
        }

        /// <summary>
        /// Delivers all <see cref="Event"/>s passed to <see cref="Send"/> since the last call to <see cref="Run"/>.
        /// </summary>
        internal void Run()
        {
            foreach (var @event in mEventsBuffer)
            {
                if (!mSubscribers.TryGetValue(@event.Kind, out var handlers))
                    continue;

                foreach (var handler in handlers)
                {
                    handler(@event);
                }
            }
        }

        #endregion
    }
}