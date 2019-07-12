namespace KernelPanic.Players
{
    /// <summary>
    /// A lightweight type implementing <see cref="IPlayerDistinction"/>.
    ///
    /// This is to be used for cases where later on a distinction is needed but not the full player object.
    /// </summary>
    internal struct StaticDistinction : IPlayerDistinction
    {
        private readonly bool mIsActive;

        /// <summary>
        /// Creates a <see cref="StaticDistinction"/> which decides using the given flag.
        /// </summary>
        /// <param name="isActive">Used to select between the two values in <see cref="Select{T}"/>.</param>
        internal StaticDistinction(bool isActive)
        {
            mIsActive = isActive;
        }

        /*
        /// <summary>
        /// Creates a <see cref="StaticDistinction"/> from an other <see cref="IPlayerDistinction"/>.
        /// </summary>
        /// <param name="distinction">The other <see cref="IPlayerDistinction"/>.</param>
        internal StaticDistinction(IPlayerDistinction distinction)
            : this(distinction.Select(true, false))
        {
        } */

        /// <inheritdoc />
        public T Select<T>(T ifActive, T ifPassive)
        {
            return mIsActive ? ifActive : ifPassive;
        }
    }
}
