using UnityEngine;

namespace Assets.SimpleGame.BuilderSystem
{
    public interface IBuilderInteractable
    {
        void OnUnBuilt();
        void OnBuilt(Vector3 position, Vector3 normal, Vector3 lookDir);
    }
}