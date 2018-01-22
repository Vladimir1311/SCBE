using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPResolver.Extensions
{
    public static class DictionaryExt
    {
        public static KeyValuePair<K, V> AddPair<K, V>(this Dictionary<K, V> dictionary, K key, V value)
        {
            var pair = KeyValuePair.Create(key, value);
            dictionary.Add(pair.Key, pair.Value);
            return pair;
        }
    }
}
