using System;
using Microsoft.Xna.Framework;

namespace KernelPanic.Data
{
    internal static class Geometry
    {
        private static bool DistancePointToLine(Vector2 center, float radius, Vector2 start, Vector2 end)
        {
            // distance between center and line start to end
            var (x, y) = center;
            var (f, f1) = start;
            var (x1, y1) = end;
            var distance = Math.Abs((x1 - f)*(f1 - y) - (f - x)*(y1 - f1))/
                   Math.Sqrt(Math.Pow(x1 - f, 2) + Math.Pow(y1 - f1, 2));
            return distance <= radius;
        }

        internal static bool CircleIntersect(Vector2 center, float radius, Rectangle rectangle)
        {
            if (rectangle.Contains(center))
                return true;

            var p1 = rectangle.Location.ToVector2();
            var p2 = p1 + new Vector2(rectangle.Width, 0);
            var p3 = p1 + new Vector2(0, rectangle.Height);
            var p4 = p1 + new Vector2(rectangle.Width, rectangle.Height);
            return DistancePointToLine(center, radius, p1, p2) ||
                   DistancePointToLine(center, radius, p2, p4) ||
                   DistancePointToLine(center, radius, p3, p4) ||
                   DistancePointToLine(center, radius, p3, p1);
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
    }
}