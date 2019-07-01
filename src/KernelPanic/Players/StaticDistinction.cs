namespace KernelPanic.Players
{
    internal struct StaticDistinction : IPlayerDistinction
    {
        private readonly bool mIsActive;

        internal StaticDistinction(bool isActive)
        {
            mIsActive = isActive;
        }

        internal StaticDistinction(IPlayerDistinction distinction)
            : this(distinction.Select(true, false))
        {
        }

        public T Select<T>(T ifActive, T ifPassive)
        {
            return mIsActive ? ifActive : ifPassive;
        }
    }
}