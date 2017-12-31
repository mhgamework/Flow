using Assets.MarchingCubes.VoxelWorldMVP;
using UnityEngine;

namespace Assets.MarchingCubes._Unorganized.VoxelWorldMVP.Uniform
{
    [ExecuteInEditMode]
    public class EditorVoxelGeneratorScript : MonoBehaviour
    {
        public UniformVoxelRendererScript UniformRenderer;

        public void Start()
        {
            
        }

        public void Update()
        {
            UniformRenderer.UpdateRenderers();
        }
    }
}