using System;
using System.Collections.Generic;
using System.Linq;
using Assets.MarchingCubes.VoxelWorldMVP.Octrees;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    /// <summary>
    /// LRU cache that supports lazy filling
    /// </summary>
    public class AsyncLRUCache<T, R> where R : class 
    {
        private int cacheSize = 100;


        private LinkedList<T> recentlyUsedList = new LinkedList<T>();


        private Dictionary<T, R> cache = new Dictionary<T, R>();

        private int numDataItems = 0;

        public AsyncLRUCache(int cacheSize)
        {
            this.cacheSize = cacheSize;
        }

        public IEnumerable<T> GetMissingData()
        {
            return recentlyUsedList.Where(f => cache[f] == null);
        }
        /// <summary>
        /// Returns false when the node is already dropped from the cache. This may indicate the cache is too small or the generation too slow.
        /// </summary>
        public bool AddData(T node, R data)
        {
            if (!cache.ContainsKey(node)) return false; // Wasted node!! immediately thrown away

            // First add the node
            // Add is done first, because the added node can also be the one dropped if it is least used of the existing nodes
            cache[node] = data;
            numDataItems++;

            if (numDataItems > cacheSize+1) throw new InvalidOperationException("Should not be possible!");
            if (numDataItems == cacheSize+1) // if the cache was overflowed
            {
                // Cache is too full, drop the extra item 
                dropSingleDataFromCache(); 
            }



           

            return true;
        }

        private void dropSingleDataFromCache()
        {
            for (;;)
            {
                var node = recentlyUsedList.Last();
                var data = cache[node];

                recentlyUsedList.RemoveLast();
                cache.Remove(node);
                if (data != null)
                {
                    // Removed some actual data from the cache
                    numDataItems--;
                    return;
                }
                // The oldest item was never added to the cache, so remove another one
            }
        }

        public object Get(T node)
        {
            R val;
            if (cache.TryGetValue(node, out val))
            {
                // Cache hit !

                // Used, move to front of queue
                recentlyUsedList.Remove(node);
                recentlyUsedList.AddFirst(node);
                return val;
            }

            // Cache miss, add to list
            recentlyUsedList.AddFirst(node);
            cache.Add(node, null);

            return null;
        }
    }
}