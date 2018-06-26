using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace utils
{
    public static class LinqExtensions
    {
        public static int IndexOf<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
        {
            return collection.Select((value, index) => new { value, index })
                        .Where(pair => predicate(pair.value))
                        .Select(pair => pair.index + 1)
                        .FirstOrDefault() - 1;
        }
    }
}
