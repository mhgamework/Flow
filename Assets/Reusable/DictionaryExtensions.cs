using System;
using System.Collections.Generic;
using UnityEngine.Profiling;

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
        //I removed this signature, because it creates an instance of the lambda closure on each get!! public static T GetOrCreate<T, K>(this IDictionary<K, T> dict, K key, Func<T> create)
        /// <summary>
        /// Be careful not to pass a closure here, as it will trash the GC!!
        /// </summary>
        public static T GetOrCreate<T, K>(this IDictionary<K, T> dict, K key, Func<K,T> create)
        {
            T ret;

            if (!dict.TryGetValue(key, out ret))
            {
                ret = create(key);
                dict[key] = ret;
            }

            return ret;
        }
    }
}