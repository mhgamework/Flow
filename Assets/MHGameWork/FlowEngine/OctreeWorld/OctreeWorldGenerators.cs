using Assets.MHGameWork.FlowEngine.Models;
using UnityEngine;

namespace Assets.MHGameWork.FlowEngine.OctreeWorld
{
    /// <summary>
    /// Simple example/test world generators
    /// </summary>
    public class OctreeWorldGenerators
    {
        private static VoxelMaterial defaultMaterial = new VoxelMaterial(Color.red);

        public static IWorldGenerator GetEmptyWorldGenerator()
        {
            return new ConstantVoxelWorldGenerator(float.MaxValue, defaultMaterial);
        }

        public static IWorldGenerator GetPlaneWorldGenerator(float planeY)
        {
            return new DelegateVoxelWorldGenerator((p, v) => new VoxelData(p.y - planeY, defaultMaterial));
        }
    }
}
