using System;
using System.Collections.Generic;
using System.Linq;
using Assets.MarchingCubes.Persistence;
using Assets.MarchingCubes.VoxelWorldMVP.Octrees;
using Assets.MarchingCubes.VoxelWorldMVP.Persistence;
using DirectX11;
using UnityEngine;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    /// <summary>
    /// Unity component to bootstrap the sky islands voxel renderer
    /// </summary>
    public class LodVoxelWorldScript : MonoBehaviour
    {
        private UniformVoxelWorldRenderer worldRenderer;
        private VoxelWorldRaycaster raycaster;

        public List<Material> Materials = new List<Material>();
        public int Size = 3;


        public VoxelMaterial ActiveMaterial;
        public float ActiveSize = 3;

        public bool CreateDefaultObjects = true;
        public GenerationAlgorithm BaseGeneration;

        public List<Color> MaterialColors;
        private List<VoxelMaterial> VoxelMaterials;

        private IWorldSerializer worldSerializer;
        public VoxelWorldAsset SavedWorld;

        public int SavedRevertVersions = 0;


        public enum GenerationAlgorithm
        {
            Empty,
            Flat,
            SinTerrain,
            Spheres,
            SpheresWithFiltering
        }

        public void Start()
        {
            // TODO: create Materials here, set on editor and set dictionary on renderer!
            VoxelMaterials = MaterialColors.Select(c => new VoxelMaterial { color = c }).ToList();

            worldSerializer = new RuntimeWorldSerializer();

#if UNITY_EDITOR
            //worldSerializer = new EditorWorldSerializer();
#endif
            SavedWorld = worldSerializer.LoadAsset(SavedWorld);
            if (SavedWorld == null)
            {
                SavedWorld = worldSerializer.CreateAsset(minNodeSize);
            }

            var generationAlgorithm = getGenerationAlgorithm(BaseGeneration);
            var persistence = new PersistenceWorldGenerator(generationAlgorithm, SavedWorld, SavedRevertVersions);

            world = new OctreeVoxelWorld(persistence, minNodeSize, WorldDepth);

            GetComponent<OctreeVoxelWorldRenderer>().Init(world, VoxelMaterials);
            GetComponent<VoxelWorldEditorScript>().Init(world, VoxelMaterials);
            GetComponent<VoxelAutoSaver>().Init(world, VoxelMaterials);

            //var world = new UniformVoxelWorld(new DelegateVoxelWorldGenerator(worldFunction), new Point3(16, 16, 16));
            //worldRenderer = new UniformVoxelWorldRenderer(world, transform);

            //worldRenderer.createRenderers(new Point3(1, 1, 1) * Size, Materials.ToArray());

            //ActiveMaterial = MaterialGreen;


            //raycaster = new VoxelWorldRaycaster();
            if (CreateDefaultObjects)
            {
                world.RunKernel1by1(new Point3(0, 0, 0), new Point3(16, 16, 16),
                    WorldEditTool.createAddSphereKernel(new Vector3(8, 8, 8), 3f, VoxelMaterials[0]), 1);
                world.RunKernel1by1(new Point3(0, 0, 0), new Point3(64, 64, 64),
                    WorldEditTool.createAddSphereKernel(new Vector3(40, 40, 40), 30f, VoxelMaterials[1]), 1);
            }
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                save();
            }
        }

        private int lastSaveFrame = 1;

        private void save()
        {
#if UNITY_EDITOR
            worldSerializer.Save(lastSaveFrame, SavedWorld, world);
#endif
            lastSaveFrame = Time.frameCount;
        }


        private IWorldGenerator getGenerationAlgorithm(GenerationAlgorithm generationAlgorithm)
        {
            switch (generationAlgorithm)
            {
                case GenerationAlgorithm.Empty:
                    return new ConstantVoxelWorldGenerator(int.MaxValue, null);
                case GenerationAlgorithm.Flat:
                    return new DelegateVoxelWorldGenerator(FlatWorldFunction);
                case GenerationAlgorithm.SinTerrain:
                    return new DelegateVoxelWorldGenerator(sinWorld);
                case GenerationAlgorithm.Spheres:
                    return new DelegateVoxelWorldGenerator((a, b) => worldFunction(a, b, false));
                case GenerationAlgorithm.SpheresWithFiltering:
                    return new DelegateVoxelWorldGenerator((a, b) => worldFunction(a, b, true));
                default:
                    throw new InvalidOperationException();
            }
        }

        public int minNodeSize = 16;
        public int WorldDepth = 3;
        private OctreeVoxelWorld world;


        private VoxelData LowerleftSphereWorldFunction(Vector3 arg1)
        {
            return new VoxelData()
            {
                Density = SdfFunctions.Sphere(arg1, new Vector3(0, 0, 0), minNodeSize * 3),
                Material = VoxelMaterials[0]
            };
        }

        private VoxelData FlatWorldFunction(Vector3 arg1)
        {
            return new VoxelData()
            {
                Density = arg1.y - 10,
                Material = VoxelMaterials[0]
            };
        }

        private VoxelData sinWorld(Vector3 arg1, int samplingInterval)
        {
            arg1 += new Vector3(300, 0, 300);
            double dens = (Math.Sin(arg1.x * 0.01) + Math.Cos(arg1.z * 0.01)) * 50 - arg1.y + 200;
            dens += (Math.Sin(arg1.x * 0.11) + Math.Cos(arg1.z * 0.09)) * 4.3;

            //dens += (Math.Sin(arg1.x * 0.002) + Math.Cos(arg1.z * 0.0019)) * 651;
            //dens += (Math.Sin(arg1.x * 0.00021) + Math.Cos(arg1.z * 0.0002)) * 3000;
            return new VoxelData() { Density = (float)dens, Material = VoxelMaterials[0] };
        }

        private VoxelData worldFunction(Vector3 arg1, int samplingInterval, bool enableSampling)
        {
            if (samplingInterval > 5 && enableSampling)
            {
                return new VoxelData() { Density = 1, Material = null };
            }
            var repeat = 20;
            var radius = 7;

            var c = new Vector3(1, 1, 1) * repeat;
            var q = mod(arg1, c) - 0.5f * c;
            var s = q.magnitude - radius;
            return new VoxelData() { Density = s, Material = VoxelMaterials[0] };
        }

        private Vector3 mod(Vector3 p, Vector3 c)
        {
            return new Vector3(p.x % c.x, p.y % c.y, p.z % c.z);
        }
    }
}