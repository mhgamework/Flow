using System;
using Assets.MarchingCubes.Rendering;
using UnityEngine;

namespace Assets.SimpleGame.Scripts
{
    [Obsolete]
    public class SimpleGameStartupScript : Singleton<SimpleGameStartupScript>
    {
        public VoxelRenderingEngineScript RenderingEngine
        {
            get { return SimpleGameSystemScript.Instance.VoxelRenderingEngine; }
        }
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