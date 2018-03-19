using System;
using System.Collections.Generic;
using Assets.Reusable.Threading;
using UnityEngine.Profiling;

namespace Assets.Reusable
{
    /// <summary>
    /// Pool of objects that can is automatically extended and can reuse objects without having them go to the GC
    /// If the pool gets empty, it replenishes the pool by 'growSize' number of objects on the main thread.
    /// 
    /// </summary>
    public class ConcurrentObjectPool<T>
    {
        private readonly Func<T> createNew;
        private readonly int growSize;
        private readonly IDispatcher dispatcher;
        private readonly string name;
        private Stack<T> pool;

        public ConcurrentObjectPool(Func<T> createNew, int initialCapacity, int growSize,IDispatcher dispatcher, string name = null)
        {
            this.createNew = createNew;
            this.growSize = growSize;
            this.dispatcher = dispatcher;
            this.name = name ?? "ConcurrentObjectPool<" + typeof(T).Name + ">";
            pool = new Stack<T>(initialCapacity);
            dispatcher.Dispatch(() => growPool(initialCapacity));
        }

        public T Take()
        {
            lock (this)
            {
                if (pool.Count == 0) dispatcher.Dispatch(() => growPool(growSize));
                return pool.Pop();
            }
        }

        /// <summary>
        /// Make sure not to hold the item anymore!!
        /// </summary>
        /// <param name="item"></param>
        public void Release(T item)
        {
            lock (this)
            {
                pool.Push(item);
            }
        }

        private void growPool(int amount)
        {
            Profiler.BeginSample("Grow " + name);
            for (int i = 0; i < amount; i++)
            {
                pool.Push(createNew());
            }
            Profiler.EndSample();
        }
    }
}