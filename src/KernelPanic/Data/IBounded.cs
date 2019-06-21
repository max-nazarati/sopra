using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace KernelPanic.Data
{
    internal interface IBounded
    {
        /// <summary>
        /// Should return a rectangle which completely contains the object.
        /// </summary>
        Rectangle Bounds { get; }
    }

    internal static class Bounds
    {
        internal static Rectangle ContainingRectangle(Vector2 position, Vector2 size)
        {
            return new Rectangle(
                (int) position.X,
                (int) position.Y,
                (int) Math.Ceiling(size.X),
                (int) Math.Ceiling(size.Y));
        }

        /// <summary>
        /// Returns the bounds of a given <see cref="IBounded"/> offset by a given value.
        /// </summary>
        /// <param name="bounded">The value which provides the bounds.</param>
        /// <param name="offset">Specifies the X and Y offset to be added to the bounds.</param>
        /// <returns>The bounds of <paramref name="bounded"/> offset by <paramref name="offset"/>.</returns>
        private static Rectangle OffsetBounds(this IBounded bounded, Vector2 offset)
        {
            var bounds = bounded.Bounds;
            bounds.Offset(offset);
            return bounds;
        }

        /// <summary>
        /// Unions all bounds of the elements in <paramref name="elements"/>.
        /// </summary>
        /// <param name="elements">The bounded elements.</param>
        /// <returns>A <see cref="Rectangle"/> which contains all bounds of <paramref name="elements"/>.</returns>
        /// <exception cref="InvalidOperationException">If <paramref name="elements"/> contains no elements.</exception>
        internal static Rectangle Union<T>(this IEnumerable<T> elements) where T: IBounded
        {
            return elements.Union(Vector2.Zero);
        }

        /// <summary>
        /// Unions all bounds of the elements in <paramref name="elements"/>.
        /// </summary>
        /// <param name="elements">The bounded elements.</param>
        /// <param name="offset">The offset to be added to each bound.</param>
        /// <returns>A <see cref="Rectangle"/> which contains all bounds of <paramref name="elements"/>.</returns>
        /// <exception cref="InvalidOperationException">If <paramref name="elements"/> contains no elements.</exception>
        internal static Rectangle Union<T>(this IEnumerable<T> elements, Vector2 offset) where T: IBounded
        {
            return elements.Select(bounded => bounded.OffsetBounds(offset)).Aggregate(Rectangle.Union);
        }
    }
}
