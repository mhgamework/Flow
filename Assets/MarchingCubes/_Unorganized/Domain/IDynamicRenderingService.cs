using Assets.MarchingCubes.Domain;
using Assets.MarchingCubes.VoxelWorldMVP;
using UnityEngine;

namespace Assets.Flow.FireSpirit
{
    public interface IDynamicRenderingService
    {
        void AddMaterial(Color col, Material mat);
        DynamicVoxelEntity CreateEntity(SdfFunction sdf, float size, Vector3 center, float sampleInterval);
    }
}