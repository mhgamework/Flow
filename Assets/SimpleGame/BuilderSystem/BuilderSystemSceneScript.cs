using UnityEngine;

namespace Assets.SimpleGame.BuilderSystem
{
    /// <summary>
    /// Script for bootstrapping the builder system testing scene
    /// </summary>
    public class BuilderSystemSceneScript : MonoBehaviour, ILevelCallbacks
    {
     
        public void OnLocalPlayerConnected(LocalPlayerScript player)
        {
            LocalPlayerScript.Instance.GetPlayer().StoreItems("boxBuilding",1);

        }
    }
}