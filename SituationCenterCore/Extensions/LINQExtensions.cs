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

        public static IQueryable<T> If<T>(this IQueryable<T> source, bool condition,
            Func<IQueryable<T>, IQueryable<T>> action)
            => condition ? action(source) : source;

        public static IQueryable<T> IfNotNullOrEmpry<T>(this IQueryable<T> source, string checkString,
            Func<IQueryable<T>, IQueryable<T>> action)
            => source.If(!string.IsNullOrEmpty(checkString), action);
    }
}
