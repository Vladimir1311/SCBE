using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System.Linq
{
    public static class LINQ
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            source.Select(E => { action(E); return E; }).Count();
        }
    }
}
