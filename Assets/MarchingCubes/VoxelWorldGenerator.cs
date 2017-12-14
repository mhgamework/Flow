using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Assets.MarchingCubes.ProceduralTutorial;
using Assets.MarchingCubes.Rendering;
using Assets.MarchingCubes.SdfModeling;
using Assets.MarchingCubes.VoxelWorldMVP;
using DirectX11;
using MHGameWork.TheWizards;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using UnityEngine;

namespace Assets.MarchingCubes
{
    /// <summary>
    /// Temporary unity script to control the world config
    /// </summary>
    public class VoxelWorldGenerator : MonoBehaviour
    {
        public enum DrawMode { Noise, Color }
        public bool DrawMesh = false;
        public float meshStartY;

        public DrawMode Mode;
        [Range(1, 6)]
        public int LodLevel;
        public bool AutoGenerate = false;
        public int WorldSize = 10;
        public float WorldScale = 1;
        public float EditorScale = 1;
        [Range(1, 20)]
        public int Octaves;
        [Range(0, 1)]
        public float Persistence;
        public float Lacunarity;
        public int Seed;
        public Vector2 Offset;
        public TerrainType[] Regions;


        public bool GenerateCalibrationNoise = false;

        public float HeightMultiplier = 1;

        public AnimationCurve MeshHightCurve;






        public int ChunkSize = 8;
        public int WorldDepth = 5;

        public GeneratorType Generator;
        public enum GeneratorType
        {
            Flat,
            LowerLeftSphere,
            JasperMouse,
            JasperApple,
            Terrain3DValueFbm,
            Terrain2DValueFbm,
            TerrainFromDemo
        }

        public List<Color> MaterialColors;
        public List<VoxelMaterial> VoxelMaterials { get; private set; }


        private Dictionary<GeneratorType, Func<IWorldGenerator>> persistenceDict =
            new Dictionary<GeneratorType, Func<IWorldGenerator>>();

        private Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();

        public void Start()
        {
            VoxelMaterials = MaterialColors.Select(c => new VoxelMaterial { color = c }).ToList();

            persistenceDict.Add(GeneratorType.Flat, () => fromDelegate(FlatWorldFunction));
            persistenceDict.Add(GeneratorType.LowerLeftSphere, () => fromDelegate(LowerleftSphereWorldFunction));

            var jSkull = JasperTest.createSkull();
            var jApple = JasperTest.createApple();
            persistenceDict.Add(GeneratorType.JasperApple, () => fromDelegate(p => SDFJasper((p) / 100f - Vector3.one * 0.4f, jApple)));
            persistenceDict.Add(GeneratorType.JasperMouse, () => fromDelegate(p => SDFJasper((p) / 100f - Vector3.one * 0.4f, jSkull)));

            persistenceDict.Add(GeneratorType.Terrain3DValueFbm, () => fromDelegate(theTerrain3D));
            persistenceDict.Add(GeneratorType.Terrain2DValueFbm, () => fromDelegate(theTerrain2D));
            persistenceDict.Add(GeneratorType.TerrainFromDemo, () => new DemoWorldGenerator(this));

        }


        public OctreeVoxelWorld GetWorld()
        {
            IWorldGenerator persistence = persistenceDict[Generator]();
            var world = new OctreeVoxelWorld(persistence, ChunkSize, WorldDepth);
            return world;
        }

        private IWorldGenerator fromDelegate(Func<Vector3, VoxelData> f)
        {
            return new DelegateVoxelWorldGenerator(f);
        }
        private IWorldGenerator fromDelegate(Func<Vector3, int, VoxelData> f)
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
        TerrainTest test = new TerrainTest();
        private VoxelData theTerrain3D(Vector3 arg1, int resolution)
        {
            var res = test.fbmd(arg1 * 0.01f, resolution * 0.01f);
            var sdf = res.x;

            sdf -= 0.25f;
            //sdf *= 0.7f;
            var mat = VoxelMaterials[2];
            var dot = Vector3.Dot(-TerrainTest.yzw(res), Vector3.up);
            if (dot > 0.8f)
                mat = VoxelMaterials[1];
            if (dot < -0.8f)
                mat = VoxelMaterials[0];

            return new VoxelData()
            {
                Density = sdf,
                Material = mat
            };
        }
        private VoxelData theTerrain2D(Vector3 arg1, int resolution)
        {
            var res = test.fbmd(arg1.TakeXZ() * (1f / 1000));//, resolution * 0.01f);
            var sdf = res.x * 300f + 500 - arg1.y;

            //sdf -= 0.25f;
            //sdf *= 0.7f;
            var mat = VoxelMaterials[2];
            //var dot = Vector3.Dot(-TerrainTest.yzw(res), Vector3.up);
            //if (dot > 0.8f)
            //    mat = VoxelMaterials[1];
            //if (dot < -0.8f)
            //    mat = VoxelMaterials[0];

            return new VoxelData()
            {
                Density = sdf,
                Material = mat
            };
        }

        public void RequestMapData(Action<MapData> callback)
        {
            new Thread(() => MapDataThread(callback)).Start();
        }

        void MapDataThread(Action<MapData> callback)
        {
            //var data = GenerateMapData();
            //lock (mapDataThreadInfoQueue)
            //    mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, data));
        }

        private void Update()
        {
            if (mapDataThreadInfoQueue.Any())
            {
                lock (mapDataThreadInfoQueue)
                    for (int i = 0; i < mapDataThreadInfoQueue.Count; i++)
                    {
                        var info = mapDataThreadInfoQueue.Dequeue();
                        info.callback(info.parameter);
                    }
            }
        }

        public MapData GenerateMapData(Vector2 lowerLeft, int sampleDist, int mapSize, float scale)
        {

            var map = Noise.noise(mapSize, Octaves, WorldScale* scale, Persistence, Lacunarity, Seed, new Vector2(Offset.x, Offset.y) + lowerLeft, sampleDist);

            if (GenerateCalibrationNoise)
                map = Noise.calibrationNoise(mapSize, WorldScale* scale / sampleDist, Offset);


            var colorMap = new Color[mapSize * mapSize];
            for (int x = 0; x < mapSize; x++)
                for (int z = 0; z < mapSize; z++)
                {
                    var height = map[x, z];
                    for (int i = 0; i < Regions.Length; i++)
                    {
                        if (height <= Regions[i].Height)
                        {
                            colorMap[x + z * mapSize] = Regions[i].Color;
                            break;
                        }


                    }
                }

            return new MapData(map, colorMap);
        }

        public Array3D<VoxelData> GenerateVoxelData(MapData mapData, int sampleDistance, float chunkHeight, VoxelMaterial material, float editorScale)
        {
            return VoxelDataGenerator.VoxelDataFromMapData(mapData, sampleDistance, chunkHeight, material, HeightMultiplier * WorldScale* editorScale, MeshHightCurve);
        }

        public void DrawMapInEditor()
        {
            var mapSize = WorldSize / getLodSampleDistance();

            var data = GenerateMapData(new Vector2(1, 1) * WorldSize / 2/ getLodSampleDistance(), getLodSampleDistance(), mapSize,EditorScale);



            if (Mode == DrawMode.Noise)
                WorldDisplay.Instance.DrawTexture(WorldDisplay.TextureFromHeightMap(data.HeightMap), getLodSampleDistance());
            else if (Mode == DrawMode.Color)
                WorldDisplay.Instance.DrawTexture(WorldDisplay.TextureFromColorMap(data.ColorMap, mapSize, mapSize), getLodSampleDistance());
            if (DrawMesh)
            {
                var resolution = getLodSampleDistance();
                WorldDisplay.Instance.DrawMesh(GenerateMeshForEditor(data, resolution, meshStartY, 1, MeshHightCurve),
                    new Vector3(10 * resolution, 10 * resolution, 10 * resolution), (mapSize) - 1);
            }
        }

        private int getLodSampleDistance()
        {
            return (int)Math.Round(Math.Pow(2, LodLevel - 1));
        }

        public VoxelMeshData GenerateMeshForEditor(MapData mapData, int sampleDistance, float chunkHeight, float heightMultiplier, AnimationCurve curve)
        {
            var gen = new VoxelChunkMeshGenerator(new MarchingCubesService());

            var data = GenerateVoxelData(mapData, sampleDistance, chunkHeight, null,EditorScale);

            var mesh = gen.GenerateMeshFromVoxelData(data);
            return mesh;
        }


        struct MapThreadInfo<T>
        {
            public readonly Action<T> callback;
            public readonly T parameter;

            public MapThreadInfo(Action<T> callback, T parameter)
            {
                this.callback = callback;
                this.parameter = parameter;
            }
        }
    }
    class DemoWorldGenerator : IWorldGenerator
    {
        private VoxelWorldGenerator voxelWorldGenerator;

        public DemoWorldGenerator(VoxelWorldGenerator voxelWorldGenerator)
        {
            this.voxelWorldGenerator = voxelWorldGenerator;
        }

        public UniformVoxelData Generate(Point3 start, Point3 chunkSize, int sampleResolution)
        {
            var map = voxelWorldGenerator.GenerateMapData(start.ToVector3().TakeXZ(), sampleResolution,chunkSize.X,1);

            var ret = new UniformVoxelData()
            {
                Data = voxelWorldGenerator.GenerateVoxelData(map, sampleResolution, start.Y, voxelWorldGenerator.VoxelMaterials[0],1),
                LastChangeFrame = 0
            };

            return ret;
        }
    }

    public struct MapData
    {
        public readonly float[,] HeightMap;
        public readonly Color[] ColorMap;

        public MapData(float[,] heightMap, Color[] colorMap)
        {
            HeightMap = heightMap;
            ColorMap = colorMap;
        }
    }
    [System.Serializable]
    public struct TerrainType
    {
        public float Height;
        public Color Color;
    }
}