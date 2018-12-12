using Assets.MHGameWork.FlowEngine.Models;
using DirectX11;
using UnityEngine;

namespace Assets.MHGameWork.FlowEngine._Cleanup.EditorVoxelWorldGen
{
    public interface IVoxelObject
    {
        bool Subtract { get; }
        Vector3 Min { get;  }
        Vector3 Max { get;  }
        void Sdf(Point3 point3, VoxelData voxelData, out float density, out Color color);
    }
}