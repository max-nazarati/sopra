using System;
using Microsoft.Xna.Framework;

namespace KernelPanic
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
    }
}
