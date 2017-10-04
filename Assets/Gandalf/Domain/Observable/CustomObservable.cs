using System.Collections.Generic;

namespace Assets.Gandalf.Domain.Observable
{
    public class CustomObservable<T>
    {
        private List<CustomSubscriber<T>> subscribers = new List<CustomSubscriber<T>>();
        public void OnNext(T el)
        {
            foreach (var s in subscribers) s.OnNext(el);
        }

        public CustomSubscriber<T> Subscribe()
        {
            var ret = new CustomSubscriber<T>();
            subscribers.Add(ret);
            return ret;
        }
    }
}