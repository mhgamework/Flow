using Assets.MHGameWork.FlowGame.Domain;
using UnityEngine;

namespace Assets.MHGameWork.FlowGame.GnomeTransportation
{
    public class RawResourceScript : MonoBehaviour, ITransportationSlot
    {
        public string ResourceTypeIdentifier;
        public int Amount;


        public void Start()
        {
            ResourceTypeFactory.FindById(ResourceTypeIdentifier);
        }

        public Vector3 Position
        {
            get { return transform.position; }
        }

        public int RequestChangeResourceAmount(ResourceType type, int change)
        {
            if (change >= 0) return 0;
            if (Amount > 0) Amount--;
            return -1;
        }

        public ResourceType GetAvailableResourceType()
        {
            return ResourceTypeFactory.FindById(ResourceTypeIdentifier);
        }
    }
}