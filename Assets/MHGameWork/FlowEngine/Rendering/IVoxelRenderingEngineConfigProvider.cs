using UnityEngine;

namespace Assets.MarchingCubes.Rendering
{
    public interface IVoxelRenderingEngineConfigProvider
    {
        Material GetTemplateMaterial();
        Material GetVertexColorMaterial();
        float GetRenderScale();
    }
}