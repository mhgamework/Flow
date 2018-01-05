using Assets.MarchingCubes.Rendering;
using UnityEngine;

namespace Assets.SimpleGame.Scripts
{
    public class SimpleGameStartupScript : MonoBehaviour
    {
        public VoxelRenderingEngineScript RenderingEngine;
        public Transform RootScene;
        private bool init;

        public void Update()
        {
            //var objects= RootScene.GetComponentsInChildren<IVoxelObject>();

            //EditorVoxelGeneratorScript.RenderObjectsToWorld(objects, RenderingEngine.GetWorld());

            //init = true;
        }
    }
}