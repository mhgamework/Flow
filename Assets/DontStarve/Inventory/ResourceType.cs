using UnityEngine;

namespace Assets
{
    [System.Serializable]
    public class ResourceType 
    {
        public string Identifier;
        public GameObject WorldPrefab;
        public GameObject UIPrefab;

        public ResourceType()
        {
            
        }
        public ResourceType(string identifier, GameObject worldPrefab, GameObject uiPrefab)
        {
            Identifier = identifier;
            WorldPrefab = worldPrefab;
            UIPrefab = uiPrefab;
        }
    }
}