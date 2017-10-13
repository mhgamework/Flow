using UnityEngine;

namespace Assets.Homm.UI
{
    public abstract class BaseUIState : MonoBehaviour
    {
        public virtual void ActivateState()
        {
            gameObject.SetActive(true);
        }

        public virtual void DeactivateState()
        {
            gameObject.SetActive(false);
        }

    }
}