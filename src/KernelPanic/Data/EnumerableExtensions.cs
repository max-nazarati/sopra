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
    }
}
