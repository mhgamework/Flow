using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Assets.Reusable.Threading
{
    public class MainThreadDispatcher : Singleton<MainThreadDispatcher>, IDispatcher
    {
        private int count = 0;
        private Queue<Action> queue = new Queue<Action>();
        private Thread mainThread;
        public void Awake()
        {
            mainThread = Thread.CurrentThread;
        }

        public void Update()
        {
            lock (queue)
            {
                if (queue.Count == 0) return;
                // Danger for deadlock!!
                queue.Dequeue()();
                count++;
                Monitor.PulseAll(queue);
            }
        }

        public void Dispatch(Action act)
        {
            if (mainThread == null) throw new Exception("Should run first!");
            if (Thread.CurrentThread == mainThread)
            {
                act();
                return;
            }

            lock (queue)
            {
                int theCount = count;
                queue.Enqueue(act);
                while (count == theCount) Monitor.Wait(queue);//Block until executed
            }

        }

        public bool IsMainThread()
        {
            return Thread.CurrentThread == mainThread;
        }
    }
}