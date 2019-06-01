using System;

namespace KernelPanic
{
    internal class Building :Entity
    {

        public Building(int price) : base(price)
        {

        }
        public Building(TimeSpan timeSpan) : base(timeSpan)
        {

        }

        public int BitcoinWorth { get; set; }

        public State StateProperty { get; set; }
        
        internal enum State
        {
            /// <summary>
            /// The building is able to act, that means it is able to attack enemies.
            /// </summary>
            Active,
            
            /// <summary>
            /// The building has been bought and is waiting to become active, that is when no enemies are at its position.
            /// </summary>
            Inactive,
            
            /// <summary>
            /// Used during selection of a new place for a building when the current position is not allowed.
            /// </summary>
            Invalid,
            
            /// <summary>
            /// Used during selection of a new place for a building when the current position is allowed.
            /// </summary>
            Valid
        };
    }
}
