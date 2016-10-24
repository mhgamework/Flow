using UnityEngine;

namespace Assets.RTSActions
{
    public abstract class BaseRTSAction<T> : IRTSAction where T:Component
    {

        public abstract void Execute(T target);

        public abstract string Name { get; }

        void IRTSAction.Execute(Component target)
        {
            Execute((T) target);
        }
    }
}