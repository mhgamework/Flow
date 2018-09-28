using UnityEngine;

namespace Assets.SimpleGame.BuilderSystem
{
    public class BaseBuilderInteractable : MonoBehaviour,IBuilderInteractable
    {
        public virtual void OnUnBuilt()
        {
            Destroy(gameObject);

        }

        public virtual void OnBuilt(Vector3 position, Vector3 normal, Vector3 lookDir)
        {
        }

    }
}