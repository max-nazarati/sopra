using System;
using System.Collections.Generic;
using System.Linq;

namespace KernelPanic.Data
{
    internal static class EnumerableExtensions
    {
        internal static IEnumerable<T> Extend<T>(this IEnumerable<T> enumerable, params T[] values)
        {
            return enumerable.Concat(values);
        }

        internal static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> enumerable) =>
            enumerable.SelectMany(x => x);

        /// <summary>
        /// Turns to enumerations into an enumeration of tuples. If one enumeration is longer than the other, the
        /// elements are discarded.
        /// </summary>
        /// <param name="first">The first enumerable.</param>
        /// <param name="second">The second enumerable.</param>
        /// <typeparam name="TFirst">The type of elements in <paramref name="first"/>.</typeparam>
        /// <typeparam name="TSecond">The type of elements in <paramref name="second"/>.</typeparam>
        /// <returns>An enumerable over pairs.</returns>
        /// <seealso cref="Enumerable.Zip{TResult,TFirst,TSecond}"/>
        internal static IEnumerable<(TFirst, TSecond)> Zip<TFirst, TSecond>(
            this IEnumerable<TFirst> first, IEnumerable<TSecond> second
        ) => first.Zip(second, (a, b) => (a, b));

        /// <summary>
        /// Enumerates trough all elements throwing all out where <paramref name="func"/> returned <c>null</c>.
        /// </summary>
        /// <param name="enumerable">The source enumerable.</param>
        /// <param name="func">Applied to each element.</param>
        /// <typeparam name="TSource">Type of source elements.</typeparam>
        /// <typeparam name="TResult">Type of resulting elements.</typeparam>
        /// <returns>An enumerable over the values from <paramref name="enumerable"/> with <paramref name="func"/> applied.</returns>
        internal static IEnumerable<TResult> SelectMaybe<TSource, TResult>(
            this IEnumerable<TSource> enumerable,
            Func<TSource, TResult?> func) where TResult : struct
        {
            return enumerable.SelectMany(value =>
                func(value) is TResult result ? new[] {result} : Enumerable.Empty<TResult>());
        }
    }
}
