using Assets.MarchingCubes.VoxelWorldMVP;
using Assets.MHGameWork.FlowEngine.Models;
using Assets.MHGameWork.FlowEngine.OctreeWorld;
using UnityEngine;

namespace Assets.MarchingCubes.Procedural
{
    public class TestPerlinWorldGenerator : DelegateVoxelWorldGenerator
    {

        public TestPerlinWorldGenerator(ProceduralGenerationTest test, VoxelMaterial material)
            : base((p, k) => worldFunction(p, k, test, material))
        {
        }

        private static VoxelData worldFunction(Vector3 p, int resolution, ProceduralGenerationTest test, VoxelMaterial material)
        {
            var density = test.getTerrainVal(p.x, p.y, p.z) - test.clip;

            return new VoxelData(density, material);

        }

    }
}