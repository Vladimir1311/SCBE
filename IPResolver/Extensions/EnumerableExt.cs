using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPResolver.Extensions
{
    public static class EnumerableExt
    {
        public static bool TryGet<T>(this IEnumerable<T> collection, Func<T, bool> func, out T item)
        {
            item = collection.FirstOrDefault(func);
            return item?.Equals(default(T)) == false;
        }
    }
}
