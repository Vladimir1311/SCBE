using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterCore.Extensions
{
    public static class LINQExtensions
    {
        public static IEnumerable<T> Except<T>(this IEnumerable<T> source, T excepted)
        {
            return source.Where(E => !E.Equals(excepted));
        }
    }
}
