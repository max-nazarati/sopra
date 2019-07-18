using System;
using Microsoft.Xna.Framework;

namespace KernelPanic.Data
{
    internal static class Geometry
    {
        internal static bool CircleIntersect(Vector2 center, float radius, Rectangle rectangle)
        {
            var deltaX = center.X - Math.Max(rectangle.X, Math.Min(center.X, rectangle.X + rectangle.Width));
            var deltaY = center.Y - Math.Max(rectangle.Y, Math.Min(center.Y, rectangle.Y + rectangle.Height));
            return (deltaX * deltaX + deltaY * deltaY) < (radius * radius);
        }

        /// <summary>
        /// Calculates the angle of <paramref name="vector"/> in radians. <paramref name="piMul"/> times π is added.
        /// </summary>
        /// <param name="vector">The vector to calculate the angle of.</param>
        /// <param name="piMul">The number of times to add π to the result.</param>
        /// <returns>The angle in radians.</returns>
        internal static float Angle(this Vector2 vector, double piMul = 0)
        {
            return (float) (Math.Atan2(vector.Y, vector.X) + piMul * Math.PI);
        }

        /// <summary>
        /// Calculates the angle between <paramref name="vector1"/> and <paramref name="vector2"/> in radians.
        /// </summary>
        /// <param name="vector1">The first vector for the calculation.</param>
        /// <param name="vector2">The second vector for the calculation.</param>
        /// <returns>The angle in radians.</returns>
        internal static float Angle(Vector2 vector1, Vector2 vector2)
        {
            return (float) Math.Acos(Vector2.Dot(vector1, vector2) / (vector1.Length() * vector2.Length()));
        }
    }
}