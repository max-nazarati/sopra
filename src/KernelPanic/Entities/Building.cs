﻿namespace KernelPanic
{
    internal abstract class Building :Entity
    {
        protected Building(int price) : base(price)
        {
            BitcoinWorth = price;
        }

        internal int BitcoinWorth { get; set; }

        internal State StateProperty { get; set; }
        
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
