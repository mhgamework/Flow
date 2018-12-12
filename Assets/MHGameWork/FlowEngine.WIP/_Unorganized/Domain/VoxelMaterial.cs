using UnityEngine;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    public class VoxelMaterial
    {
        public Color color;

        public VoxelMaterial()
        {
        }

        public VoxelMaterial(Color color)
        {
            this.color = color;
        }
    }
}