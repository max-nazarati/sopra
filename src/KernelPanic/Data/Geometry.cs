using System;
using Microsoft.Xna.Framework;

namespace KernelPanic.Data
{
    internal static class Geometry
    {
        internal static bool CircleIntersect(Vector2 center, float radius, Vector2 start, Vector2 end)
        {
            var d = end - start;
            var f = start - center;
            float a = Vector2.Dot(d, d);
            float b = 2 * Vector2.Dot(f, d);
            float c = Vector2.Dot(f, f) - radius * radius;
            float discriminant = b * b - 4 * a * c;
            if (discriminant < 0)
                return false;
            discriminant = (float)Math.Sqrt(discriminant);
            float t1 = (-b - discriminant) / (2 * a);
            float t2 = (-b + discriminant) / (2 * a);

            return (t1 >= 0 && t1 <= 1) || (t2 > 0 && t2 <= 1);
        }

        internal static bool CircleIntersect(Vector2 center, float radius, Rectangle rectangle)
        {
            if (rectangle.Contains(center))
                return true;

            var p1 = rectangle.Location.ToVector2();
            var p2 = p1 + new Vector2(rectangle.Width, 0);
            var p3 = p1 + new Vector2(0, rectangle.Height);
            var p4 = p1 + new Vector2(rectangle.Width, rectangle.Height);
            return CircleIntersect(center, radius, p1, p2) ||
                   CircleIntersect(center, radius, p2, p3) ||
                   CircleIntersect(center, radius, p3, p4) ||
                   CircleIntersect(center, radius, p4, p1);
        }
    }
}