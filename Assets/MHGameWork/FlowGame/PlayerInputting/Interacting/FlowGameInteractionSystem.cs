using System.Linq;
using UnityEngine;

namespace Assets.MHGameWork.FlowGame.PlayerInputting.Interacting
{
    public class FlowGameInteractionSystem
    {
        public  float MaxInteractionDistance { get; set; }

        public FlowGameInteractableScript CurrentTargetedInteractable { get; set; }

        public FlowGameInteractionSystem()
        {
            MaxInteractionDistance = 100;
        }

        public void Update()
        {
            var screenPointToRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            Debug.DrawRay(screenPointToRay.GetPoint(0),screenPointToRay.direction);
            var hits= Physics.RaycastAll(screenPointToRay,MaxInteractionDistance);
            var objects = hits.OrderBy(f => f.distance).Select(f => f.collider.GetComponentInParent<FlowGameInteractableScript>()).Where(f => f != null);

            setCurrentTargetedInteractable(objects.FirstOrDefault());

        }

        private void setCurrentTargetedInteractable(FlowGameInteractableScript newI)
        {
            if (CurrentTargetedInteractable == newI) return;
            if (CurrentTargetedInteractable != null) CurrentTargetedInteractable.HideHighlight();
            CurrentTargetedInteractable = newI;
            if (CurrentTargetedInteractable != null)
            {
                CurrentTargetedInteractable.ShowHighlight();
            }
        }
    }
}