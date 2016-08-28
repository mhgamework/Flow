using System.Linq;
using MHGameWork.TheWizards.DualContouring;
using UnityEngine;

namespace Assets.Flow
{
    public class Player : MonoBehaviour
    {
        public float FireMagic;
        public float WaterMagic;
        public float Clay;

        public float MaxMagic = 10;
        public float MaxClay = 3;

        private IInteractable currentTarget = null;
        private Collider currentTargetCollider = null;
        public float MagicExtractionSpeed = 1;
        public float ClayExtractionSpeed = 1f/3;

        public void Update()
        {
            if (Camera.current == null) return;
            var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10f))
            {
                IInteractable interactable = hit.collider.GetComponent(typeof(IInteractable)) as IInteractable;
                if (interactable == null)
                    clearRaycast(hit.collider);
                else
                    updateRaycast(hit.collider,interactable);

                if (currentTarget != null)
                    currentTarget.TryInteract(this,hit.point);
            }
        }


        private void updateRaycast(Collider collider, IInteractable interactable)
        {
            if (currentTarget == interactable) return;
            clearRaycast(collider);
            interactable.OnFocus(this);
            collider.GetComponents(typeof(ISecondaryInteractable)).Cast<ISecondaryInteractable>().ForEach(i => i.OnFocus(this));
            
            currentTarget = interactable;
            currentTargetCollider = collider;
            Debug.Log("Interacting with " + interactable);
        }

        private void clearRaycast(Collider collider)
        {
            if (currentTarget == null) return;
            Debug.Log("Stopped interacting with: " + currentTarget);
            currentTarget.OnUnfocus(this);
            currentTargetCollider.GetComponents(typeof(ISecondaryInteractable)).Cast<ISecondaryInteractable>().ForEach(i => i.OnUnfocus(this));
            currentTarget = null;
            currentTargetCollider = null;


        }
    }
}