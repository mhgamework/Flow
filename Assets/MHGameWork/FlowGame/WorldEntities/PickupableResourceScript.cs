using System;
using Assets.MHGameWork.FlowGame.DI;
using Assets.MHGameWork.FlowGame.Domain;
using Assets.MHGameWork.FlowGame.PlayerInputting.Interacting;
using Assets.MHGameWork.FlowGame.PlayerStating;
using UnityEngine;

namespace Assets.MHGameWork.FlowGame.WorldEntities
{
    [RequireComponent(typeof(FlowGameInteractableScript))]
    public class PickupableResourceScript : MonoBehaviour
    {
        public string ResourceId = "magicCrystals";
        public int ResourceAmount = 1;
        public void Start()
        {
            GetComponent<FlowGameInteractableScript>().PlayerInteractHandler = onPlayerInteract;
            var type = ResourceTypeFactory.FindById(ResourceId);
            if (type == null) throw new Exception("Resource type not found: " + ResourceId);
        }

        private void onPlayerInteract()
        {
            var type = ResourceTypeFactory.FindById(ResourceId);
            var diff = FlowGameServiceProvider.Instance.GetService<PlayerGlobalResourcesRepository>()
                .RequestStoreResources(type, ResourceAmount);
            ResourceAmount -= diff;

            if (ResourceAmount == 0) Destroy(gameObject);
        }

    }
}