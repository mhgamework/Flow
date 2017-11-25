using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.MarchingCubes.Persistence;
using Assets.MarchingCubes.Procedural;
using Assets.MarchingCubes.VoxelWorldMVP.Octrees;
using Assets.MarchingCubes.VoxelWorldMVP.Persistence;
using DirectX11;
using MHGameWork.TheWizards.DualContouring.Terrain;
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
        public string SaveFilePath = "savegame";
        public VoxelWorldAsset SavedWorld;

        public int SavedRevertVersions = 0;

        public bool loadFromDisk = true;
        public bool debugRegenerate = false;


        public enum GenerationAlgorithm
        {
            Empty,
            Flat,
            SinTerrain,
            Spheres,
            SpheresWithFiltering,
            Perlin
        }

        public void Start()
        {
            // TODO: create Materials here, set on editor and set dictionary on renderer!
            VoxelMaterials = MaterialColors.Select(c => new VoxelMaterial {color = c}).ToList();
            worldgentemptestthing = FindObjectOfType<ProceduralGenerationTest>();
            new FileInfo(SaveFilePath).Directory.Create();
            worldSerializer = new RuntimeWorldSerializer(SaveFilePath);

#if UNITY_EDITOR
            //worldSerializer = new EditorWorldSerializer();
#endif
                StartCoroutine(loadWorld().GetEnumerator());
        }

        private IEnumerable<YieldInstruction> loadWorld()
        {
            yield return null;
            OverlayPanel.Instance.Show("Loading world...");
            yield return null;
            if (loadFromDisk)
            {
                SavedWorld = worldSerializer.LoadAsset(SavedWorld);
            }
            if (SavedWorld == null)
            {
                SavedWorld = worldSerializer.CreateAsset(minNodeSize);
            }

            var generationAlgorithm = getGenerationAlgorithm(BaseGeneration);
            var persistence = new PersistenceWorldGenerator(generationAlgorithm, SavedWorld, SavedRevertVersions);

            world = new OctreeVoxelWorld(persistence, minNodeSize, WorldDepth);

            //TODO: //GetComponent<OctreeVoxelWorldRenderer>().Init(world, VoxelMaterials);
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

            yield return null;
            OverlayPanel.Instance.Hide("Welcome to Flow.",3f);

        }


        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            { 
                save();
            }
            if (debugRegenerate || worldgentemptestthing.debugRegenerate)
            {
                debugRegenerate = false;
                worldgentemptestthing.debugRegenerate = false;

                new ClipMapsOctree<OctreeNode>().VisitTopDown(world.Root, n =>
                {
                    //if (!first) return;
                    if (n.VoxelData == null) return; // Non initialized chunk, not loaded yet
                    world.ResetChunk(n, Time.frameCount);
                });
            }
        }

        private int lastSaveFrame = 1;

        private void save()
        {
            StartCoroutine(saveCoroutine().GetEnumerator());
        }

        private IEnumerable<YieldInstruction> saveCoroutine()
        {
            ScreenCapture.CaptureScreenshot(SaveFilePath + ".png");
            yield return null;
            OverlayPanel.Instance.Show("Saving World...");
            yield return null;
            worldSerializer.Save(lastSaveFrame, SavedWorld, world);
            lastSaveFrame = Time.frameCount;
            yield return null;

            OverlayPanel.Instance.Hide("Saved!", 2);

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
                case GenerationAlgorithm.Perlin:
                    return new TestPerlinWorldGenerator(FindObjectOfType<ProceduralGenerationTest>(), VoxelMaterials[0]);
                default:
                    throw new InvalidOperationException();
            }
        }


        public int minNodeSize = 16;
        public int WorldDepth = 3;
        private OctreeVoxelWorld world;
        private ProceduralGenerationTest worldgentemptestthing;


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