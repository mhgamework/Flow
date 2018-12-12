using UnityEngine;

namespace Assets.MHGameWork.FlowEngine.Rendering
{
    public interface IVoxelRenderingEngineConfigProvider
    {
        Material GetTemplateMaterial();
        Material GetVertexColorMaterial();
        float GetRenderScale();
    }
}