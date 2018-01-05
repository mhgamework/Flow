using Assets.MarchingCubes.VoxelWorldMVP;
using DirectX11;
using UnityEngine;

namespace Assets.SimpleGame.Scripts
{
    public interface IVoxelObject
    {
        Vector3 Min { get;  }
        Vector3 Max { get;  }
        void Sdf(Point3 point3, VoxelData voxelData, out float density, out Color color);
    }
}