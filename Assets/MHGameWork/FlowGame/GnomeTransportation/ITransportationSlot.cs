using Assets.MHGameWork.FlowGame.Domain;
using UnityEngine;

namespace Assets.MHGameWork.FlowGame.GnomeTransportation
{
    public interface ITransportationSlot
    {
        Vector3 Position { get; }

        /// <summary>
        /// Returns the actual change
        /// </summary>
        int RequestChangeResourceAmount(ResourceType type, int change);

        ResourceType GetAvailableResourceType();
    }
}