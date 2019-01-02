using Assets.MHGameWork.FlowGame.DI;
using Assets.MHGameWork.FlowGame.Domain;
using Assets.MHGameWork.FlowGame.PlayerStating;
using UnityEngine;

namespace Assets.MHGameWork.FlowGame.GnomeTransportation
{
    public class NexusScript : MonoBehaviour, ITransportationSlot
    {
        public Vector3 Position
        {
            get { return transform.position; }
        }

        public int RequestChangeResourceAmount(ResourceType type, int change)
        {
            return FlowGameServiceProvider.Instance.GetService<PlayerGlobalResourcesRepository>()
                .RequestChangeResourceAmount(type, change);
        }

        public ResourceType GetAvailableResourceType()
        {
            return null;
        }
    }
}