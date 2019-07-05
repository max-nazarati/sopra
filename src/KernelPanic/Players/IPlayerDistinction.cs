namespace KernelPanic.Players
{
    /// <summary>
    /// Used to fight the battle against boolean-blindness.
    /// </summary>
    internal interface IPlayerDistinction
    {
        /// <summary>
        /// This function can be used to act differently based on whether this <see cref="Player"/> object refers to the
        /// player actively playing the game or the AI.
        /// </summary>
        /// <param name="ifActive">Value to be returned if this object is the active player.</param>
        /// <param name="ifPassive">Value to be returned if this object is the non-active (AI) player.</param>
        /// <typeparam name="T">This function is generic in the type of values accepted.</typeparam>
        /// <returns>Either <paramref name="ifActive"/> or <paramref name="ifPassive"/>.</returns>
        T Select<T>(T ifActive, T ifPassive);
    }
}
