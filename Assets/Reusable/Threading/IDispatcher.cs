using System;

namespace Assets.Reusable.Threading
{
    public interface IDispatcher
    {
        void Dispatch(Action act);
    }

    public class MockDispatcher :IDispatcher{
        public void Dispatch(Action act)
        {
            act(); // Always main thread
        }
    }
}