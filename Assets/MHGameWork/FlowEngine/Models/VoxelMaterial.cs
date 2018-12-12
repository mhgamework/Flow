using UnityEngine;

namespace Assets.MHGameWork.FlowEngine.Models
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