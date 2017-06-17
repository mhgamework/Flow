using System;
using Assets.MarchingCubes.VoxelWorldMVP;
using DirectX11;
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