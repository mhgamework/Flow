using System.Collections;
using System.Collections.Generic;

namespace Assets.Gandalf.Domain.Observable
{
    public class CustomSubscriber<T>
    {
        private List<T> buffer = new List<T>();
        public void OnNext(T el)
        {
            buffer.Add(el);
        }

        public IEnumerable<T> ConsumeBuffer()
        {
            foreach (var f in buffer) yield return f;
            buffer.Clear();
        }

    }
}