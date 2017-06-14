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

        public static IEnumerable<T> WithOut<T>(this IEnumerable<T> source, T item)
        {
            return source.Where(I => !I.Equals(item));
        }

        public static string SumStrings<T>(this IEnumerable<T> source)
        {
            return source.Select(Item => Item.ToString())
                .Aggregate((P, N) => $"{P}, {N}");
        }

        public static IEnumerable<T> Concat<T>(this T value, IEnumerable<T> collecion)
        {
            yield return value;
            foreach (var item in collecion)
                yield return item;
        }

        public static IEnumerable<T> Add<T>(this IEnumerable<T> collection, int count, Func<T> creator)
        {
            foreach (T item in collection)
                yield return item;
            for (int i = 0; i < count; i++)
                yield return creator();
        }

        public static void AnySet<T>(this List<T> list, int index, T item)
        {

            while (list.Count - 1 < index)
                list.Add(default(T));
            list[index] = item;
        }
    }
}
