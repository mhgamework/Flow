using Assets.MHGameWork.FlowEngine.Models;
using DirectX11;
using UnityEngine;

namespace Assets.MHGameWork.FlowEngine.Samples._NeedsCleanupFirst.SdfObjectRenderingSample.SystemToMove
{
    public interface ISdfObject2
    {
        void Sdf(Point3 p, VoxelData v, out float density, out Color color);
    }
}