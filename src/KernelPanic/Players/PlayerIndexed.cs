using System;
using System.Diagnostics.Contracts;

namespace KernelPanic.Players
{
    /// <summary>
    /// Stores two values of the same type <see cref="T"/> which can be accessed
    /// using values implementing <see cref="IPlayerDistinction"/>.
    /// </summary>
    /// <typeparam name="T">The type of value stored.</typeparam>
    internal struct PlayerIndexed<T>
    {
        internal T A { get; set; }
        internal T B { get; set; }

        internal PlayerIndexed(T a, T b)
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

        /// <summary>
        /// Applies a function to both <see cref="A"/> and <see cref="B"/> and constructs a new
        /// <see cref="PlayerIndexed{T}"/> from the result.
        /// </summary>
        /// <param name="func">The function to apply.</param>
        /// <typeparam name="TMapped">The result type.</typeparam>
        /// <returns>A new <see cref="PlayerIndexed{T}"/> object with the resulting values.</returns>
        [Pure]
        internal PlayerIndexed<TMapped> Map<TMapped>(Func<T, TMapped> func)
        {
            return new PlayerIndexed<TMapped>(func(A), func(B));
        }
    }
}
