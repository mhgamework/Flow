using System.Collections.Generic;
using Assets.MHGameWork.FlowEngine.Models;
using Assets.Reusable;
using UnityEngine;

namespace Assets.MHGameWork.FlowEngine.SdfWorldGeneration
{
    public class VoxelMaterialFactory
    {
        private Dictionary<Color, VoxelMaterial> materials;

        public VoxelMaterialFactory()
        {
            materials = new Dictionary<Color, VoxelMaterial>(new ColorEqualityComparer());
        }

        public VoxelMaterial GetOrCreate(Color color)
        {
            lock (this)
            {
                return materials.GetOrCreate(color, createMaterial);

            }
        }

        private VoxelMaterial createMaterial(Color c)
        {
            return new VoxelMaterial() { color = c };
        }
    }
}