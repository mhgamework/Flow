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
            LocalPlayerScript.Instance.GetPlayer().StoreItems("mirrorBuilding", 1);
            LocalPlayerScript.Instance.GetPlayer().StoreItems("wrench", 1);

            LocalPlayerScript.Instance.GetPlayer().StoreItems("chuteNode", 1);
            LocalPlayerScript.Instance.GetPlayer().StoreItems("elevatorCell", 1);
            LocalPlayerScript.Instance.GetPlayer().StoreItems("elevatorBase", 1);

        }
    }
}