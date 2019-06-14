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
        internal static Rectangle ContainingRectangle(Vector2 origin, Vector2 size)
        {
            return new Rectangle(
                (int) origin.X,
                (int) origin.Y,
                (int) Math.Ceiling(size.X),
                (int) Math.Ceiling(size.Y));
        }
    }
}
