using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Assets.Reusable.Threading;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Assets.Reusable.Editor
{
    public class ConcurrentObjectPoolTest
    {

        [Test]
        public void TestTakeUsesInitializedObjects()
        {
            var pool = new ConcurrentObjectPool<int[]>(() => new int[3] { 1, 2, 3 }, 5, 3, new MockDispatcher());

            var el = pool.Take();
            Assert.AreEqual(2, el[1]);

        }

        [Test]
        public void TestReuseObjects()
        {
            var pool = new ConcurrentObjectPool<int[]>(() => new int[10], 5, 3, new MockDispatcher());

            var el = pool.Take();

            pool.Release(el);

            var el2 = pool.Take();

            Assert.AreEqual(el2, el);
        }

        [Test]
        public void TestGrowPool()
        {
            var pool = new ConcurrentObjectPool<int[]>(() => new int[10], 2, 3, new MockDispatcher());

            for (int i = 0; i < 10; i++)
            {
                pool.Take();
            }
        }

        // A UnityTest behaves like a coroutine in PlayMode
        // and allows you to yield null to skip a frame in EditMode
        [UnityTest]
        public IEnumerator TestWithMainThreadDispatcher()
        {
            // Initialize dispatcher
            var t = MainThreadDispatcher.Instance;
            yield return null;

            var pool = new ConcurrentObjectPool<int[]>(() => new int[10], 2, 3, MainThreadDispatcher.Instance);
            yield return null;

            var count = 0;
            new Thread(() =>
            {
                for (int i = 0; i < 10; i++)
                {
                    pool.Take();
                    count++;
                }
            }).Start();

            // Should block when empty
            while (count < 2)
                Thread.Sleep(10);
            Thread.Sleep(10);
            Assert.AreEqual(2, count);

            yield return null; // Runs dispatcher

            while (count < 5)
                Thread.Sleep(10);

            Thread.Sleep(10);
            Assert.AreEqual(5, count);

            yield return null; // Runs dispatcher

            while (count < 8)
                Thread.Sleep(10);

            Thread.Sleep(10);
            Assert.AreEqual(8, count);


        }
    }
}
