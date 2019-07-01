using System.Diagnostics.Contracts;

namespace KernelPanic.Players
{
    /// <summary>
    /// Stores two values of the same type <see cref="T"/> which can be accessed
    /// using values implementing <see cref="IPlayerDistinction"/>.
    /// </summary>
    /// <typeparam name="T">The type of value stored.</typeparam>
    internal struct PlayerOwned<T>
    {
        internal T A { get; set; }
        internal T B { get; set; }

        internal PlayerOwned(T a, T b)
        {
            A = a;
            B = b;
        }

        /// <summary>
        /// Returns either <see cref="A"/> or <see cref="B"/> based on <paramref name="distinction"/>.
        /// </summary>
        /// <param name="distinction">Used to choose between <see cref="A"/> and <see cref="B"/>.</param>
        /// <returns>Either <see cref="A"/> or <see cref="B"/>.</returns>
        [Pure]
        internal T Select(IPlayerDistinction distinction)
        {
            return distinction.Select(A, B);
        }
    }
}
