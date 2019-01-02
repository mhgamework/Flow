using System.Collections.Generic;
using Assets.MHGameWork.FlowGame.DI;
using Assets.MHGameWork.FlowGame.Domain;
using Assets.MHGameWork.FlowGame.PlayerInputting.Interacting;
using Assets.SimpleGame.Scripts.Enemies;
using UnityEngine;

namespace Assets.MHGameWork.FlowGame.GnomeTransportation
{
    public class GnomeScript : MonoBehaviour
    {
        public float GnomeOperatingDistance = 1f;
        public float GnomeMovementSpeed = 3f;
        public bool IsInteracting = true;
        public ITransportationSlot Source;
        public ITransportationSlot Destination;

        private Coroutine brainRoutine;

        private FlowGameInteractionSystem interactionSystem;
        private AiCapsuleMovingBehaviour movingBehaviour;

        private ResourceType heldResourceType;
        private int amount;

        void Start()
        {
            var interactable = GetComponent<FlowGameInteractableScript>();
            interactable.PlayerInteractHandler = onInteract;

            setIsInteracting(IsInteracting);
            movingBehaviour = new AiCapsuleMovingBehaviour(GetComponent<Rigidbody>(),transform);
        }

        private void init()
        {
            interactionSystem = FlowGameServiceProvider.Instance.GetService<FlowGameInteractionSystem>();
        }
        private void onInteract()
        {
            setIsInteracting(!IsInteracting);

            //        var mode = FlowGameServiceProvider.Instance.GetService<ModeBasedInputSystem>();
            //        mode.SwitchActiveMode(new );

        }

        private void setIsInteracting(bool val)
        {
            if (val == IsInteracting) return;
            IsInteracting = val;
            if (!IsInteracting)
            {
                brainRoutine = StartCoroutine(gnomeBrain().GetEnumerator());
            }
            else
            {
                if (brainRoutine != null) StopCoroutine(brainRoutine);
            }
        }

        void Update()
        {
            if (interactionSystem == null) init();
            if (IsInteracting)
            {
                if (Source != null)
                    Debug.DrawLine(Source.Position, transform.position, Color.green);
                if (Destination != null)
                    Debug.DrawLine(Destination.Position, transform.position, Color.blue);

                if (Input.GetMouseButtonDown(0))
                {
                    Source = getTargetedSlot();
                }

                if (Input.GetMouseButtonDown(1))
                {
                    Destination = getTargetedSlot();
                }

            }
            else
            {
            
            }
        }

        ITransportationSlot getTargetedSlot()
        {
            if (interactionSystem.CurrentTargetedInteractable == null) return null;
            return interactionSystem.CurrentTargetedInteractable.GetComponentInParent<ITransportationSlot>();
        }

        IEnumerable<YieldInstruction> gnomeBrain()
        {
            for (;;)
            {
                if (!isAt(Source))
                {
                    if (Source != null)
                    {
                        foreach (var f in goTo(Source)) yield return f;

                        var availableResourceType = Source.GetAvailableResourceType();
                        var change = Source.RequestChangeResourceAmount(availableResourceType,-1);
                        if (change < 0)
                        {
                            heldResourceType = availableResourceType;
                        }
                    }
                }

                if (!isAt(Destination))
                {
                    foreach (var f in goTo(Destination)) yield return f;
                    if (heldResourceType != null && Destination != null)
                    {
                        if (Destination.RequestChangeResourceAmount(heldResourceType, 1) > 0) heldResourceType = null;
                    }

                }
                yield return null;
            }
        }

        private IEnumerable<YieldInstruction> goTo(ITransportationSlot destination)
        {
            if (destination == null) yield break;
            while (!isAt(destination))
            {
                movingBehaviour.MoveSpeed = GnomeMovementSpeed;
                movingBehaviour.MoveTo(destination.Position);
                yield return null;
            }
 
        }

        private bool isAt(ITransportationSlot source)
        {
            if (source == null) return false;
            return Vector3.Distance(source.Position, transform.position) < GnomeOperatingDistance;
        }
    }
}
