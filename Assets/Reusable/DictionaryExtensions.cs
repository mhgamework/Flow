using System.Collections.Generic;

namespace Assets.Reusable
{
    public static class DictionaryExtensions
    {
        public static T GetOrDefault<T, K>(this IDictionary<K,T> dict, K key, T defaultValue)
        {
            T ret;
            if (dict.TryGetValue(key, out ret)) return ret;

            return defaultValue;
        }
    }
}