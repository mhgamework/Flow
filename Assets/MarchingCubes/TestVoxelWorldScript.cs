using System;
using System.Collections.Generic;
using System.Linq;
using Assets.MarchingCubes.SdfModeling;
using Assets.MarchingCubes.VoxelWorldMVP;
using UnityEngine;

namespace Assets.MarchingCubes
{
    /// <summary>
    /// Temporary unity script to control the world config
    /// </summary>
    public class TestVoxelWorldScript : MonoBehaviour
    {
        public int ChunkSize = 8;
        public int WorldDepth = 5;

        public GeneratorType Generator;
        public enum GeneratorType
        {
            Flat,
            LowerLeftSphere,
            JasperMouse,
            JasperApple,
        }

        public List<Color> MaterialColors;
        public List<VoxelMaterial> VoxelMaterials { get; private set; }


        private Dictionary<GeneratorType, Func<IWorldGenerator>> persistenceDict =
            new Dictionary<GeneratorType, Func<IWorldGenerator>>();

        public void Start()
        {
            VoxelMaterials = MaterialColors.Select(c => new VoxelMaterial { color = c }).ToList();

            persistenceDict.Add(GeneratorType.Flat, () => fromDelegate(FlatWorldFunction));
            persistenceDict.Add(GeneratorType.LowerLeftSphere, () => fromDelegate(LowerleftSphereWorldFunction));

            var jSkull = JasperTest.createSkull();
            var jApple = JasperTest.createApple();
            persistenceDict.Add(GeneratorType.JasperApple, () => fromDelegate(p => SDFJasper((p) / 100f - Vector3.one * 0.4f, jApple)));
            persistenceDict.Add(GeneratorType.JasperMouse, () => fromDelegate(p => SDFJasper((p) / 100f - Vector3.one * 0.4f, jSkull)));



        }

        public OctreeVoxelWorld GetWorld()
        {
            IWorldGenerator persistence = persistenceDict[Generator]();
            int chunkSize = 8;
            int worldDepth = 5;
            var world = new OctreeVoxelWorld(persistence, chunkSize, worldDepth);
            return world;
        }

        private IWorldGenerator fromDelegate(Func<Vector3,VoxelData> f)
        {
            return new DelegateVoxelWorldGenerator(f);
        }

      

        private VoxelData FlatWorldFunction(Vector3 arg1)
        {
            return new VoxelData()
            {
                Density = arg1.y - 10,
                Material = VoxelMaterials[0]
            };
        }

        private VoxelData LowerleftSphereWorldFunction(Vector3 arg1)
        {
            return new VoxelData()
            {
                Density = SdfFunctions.Sphere(arg1, new Vector3(0, 0, 0), 8 * 3),
                Material = VoxelMaterials[0]
            };
        }

        private VoxelData SDFJasper(Vector3 arg1, DistObject distObj)
        {
            var density = distObj.Sdf(arg1);
            return new VoxelData()
            {
                Density = density,
                Material = VoxelMaterials[0]
            };
        }

    }
}