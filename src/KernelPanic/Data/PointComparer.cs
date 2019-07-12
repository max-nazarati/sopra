using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace KernelPanic.Data
{
    /// <summary>
    /// Can be used in generic algorithms (such as <see cref="List{T}.Sort(IComparer{T})"/>) to compare
    /// <see cref="Point"/> values which aren't themselves <see cref="IComparable{T}"/>.
    /// </summary>
    internal struct PointComparer : IComparer<Point>
    {
        public int Compare(Point a, Point b)
        {
            var x = a.X.CompareTo(b.X);
            return x == 0 ? a.Y.CompareTo(b.Y) : x;
        }
    }
}
