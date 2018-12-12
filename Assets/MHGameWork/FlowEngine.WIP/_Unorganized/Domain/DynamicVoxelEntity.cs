using Assets.MarchingCubes.VoxelWorldMVP;
using UnityEngine;

namespace Assets.MarchingCubes.Domain
{
    public class DynamicVoxelEntity
    {
        public SdfFunction Sdf;
        public float Size;
        public VoxelChunkRendererScript Renderer;
        public float SampleInterval;
        public Vector3 Center;

        public DynamicVoxelEntity(SdfFunction sdf, float size)
        {
            Sdf = sdf;
            Size = size;
        }
    }
}