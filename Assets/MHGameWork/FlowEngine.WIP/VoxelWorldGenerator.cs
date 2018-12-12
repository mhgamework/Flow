using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Assets.MarchingCubes.ProceduralTutorial;
using Assets.MarchingCubes.Rendering;
using Assets.MarchingCubes.SdfModeling;
using Assets.MarchingCubes.VoxelWorldMVP;
using Assets.Reusable;
using DirectX11;
using MHGameWork.TheWizards;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using UnityEngine;

namespace Assets.MarchingCubes
{
   /// <summary>
    /// Temporary unity script to control the world config
    /// </summary>
    public class VoxelWorldGenerator : MonoBehaviour, IGenericVoxelWorldGenerator
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
        [Range(0.1f, 2)]

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


        public OctreeVoxelWorld CreateNewWorld()
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

        public void GenerateMapData(MapData outMapData, Vector2 lowerLeft, int sampleDist, int mapSize, float scale)
        {

            Noise.noise(outMapData.HeightMap, mapSize, Octaves, WorldScale * scale, Persistence, Lacunarity, Seed, /*new Vector2(Offset.x, Offset.y) +*/ lowerLeft, sampleDist, out outMapData.Min, out outMapData.Max);

            if (GenerateCalibrationNoise)
                Noise.calibrationNoise(outMapData.HeightMap, mapSize, WorldScale * scale / sampleDist, Offset);

  

            for (int x = 0; x < mapSize; x++)
                for (int z = 0; z < mapSize; z++)
                {
                    var height = outMapData.HeightMap[x, z];
                    for (int i = 0; i < Regions.Length; i++)
                    {
                        if (height <= Regions[i].Height)
                        {
                            //var world = (new Vector2(x, z) * sampleDist + lowerLeft) / (WorldScale * scale);
                            //var noise = Mathf.PerlinNoise(world.x, world.y);

                            var color = Regions[i].Color;
                            //color.r += noise * 0.3f;

                            outMapData.ColorMap[x + z * mapSize] = color;
                            break;
                        }


                    }
                }

        }

        public void GenerateVoxelData(MapData mapData, int sampleDistance, float chunkHeight, Dictionary<Color, VoxelMaterial> material, float editorScale, Array3D<VoxelData> outData, out bool isEmpty)
        {
            VoxelDataGenerator.VoxelDataFromMapData(mapData, sampleDistance, chunkHeight, material, HeightMultiplier * WorldScale * editorScale, MeshHightCurve, outData,out isEmpty);
        }

        public void DrawMapInEditor()
        {
            var mapSize = WorldSize / getLodSampleDistance();

            var mapData = new MapData(mapSize);

            // Say center as 0,0, then lowerleft is at:
            var lowerLeft = Offset * WorldScale * EditorScale - new Vector2(1, 1) * WorldSize / 2 * getLodSampleDistance();

            GenerateMapData(mapData, lowerLeft, getLodSampleDistance(), mapSize, EditorScale);



            if (Mode == DrawMode.Noise)
                WorldDisplay.Instance.DrawTexture(WorldDisplay.TextureFromHeightMap(mapData.HeightMap), getLodSampleDistance());
            else if (Mode == DrawMode.Color)
                WorldDisplay.Instance.DrawTexture(WorldDisplay.TextureFromColorMap(mapData.ColorMap, mapSize, mapSize), getLodSampleDistance());
            if (DrawMesh)
            {
                var resolution = getLodSampleDistance();
                WorldDisplay.Instance.DrawMesh(GenerateMeshForEditor(mapData, resolution, meshStartY, 1, MeshHightCurve),
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

            var data = new Array3D<VoxelData>(new Point3(1, 1, 1) * mapData.HeightMap.GetLength(0));

            bool isEmpty;
            GenerateVoxelData(mapData, sampleDistance, chunkHeight, GetMaterialsDictionary(), EditorScale, data,out isEmpty);

            var mesh = VoxelMeshData.CreatePreallocated();
            gen.GenerateMeshFromVoxelData(data, mesh);
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

        private Dictionary<Color, VoxelMaterial> materialsDictionaryCache;
        public Dictionary<Color, VoxelMaterial> GetMaterialsDictionary()
        {
            if (materialsDictionaryCache == null)
            {
                lock (this)
                {
                    if (materialsDictionaryCache == null) // only first thread
                    {
                        materialsDictionaryCache = new Dictionary<Color, VoxelMaterial>(new ColorEqualityComparer());
                        foreach (var r in Regions)
                        {
                            if (materialsDictionaryCache.ContainsKey(r.Color)) continue;
                            materialsDictionaryCache.Add(r.Color, new VoxelMaterial() { color = r.Color });
                        }
                    }
                }

            }

            return materialsDictionaryCache;
        }
    }
    class DemoWorldGenerator : IWorldGenerator
    {
        private VoxelWorldGenerator voxelWorldGenerator;

        public DemoWorldGenerator(VoxelWorldGenerator voxelWorldGenerator)
        {
            this.voxelWorldGenerator = voxelWorldGenerator;
        }

        private bool init = false;
        //MapData cacheData;//Not thread safe!

        private Dictionary<Thread, MapData> cacheData = new Dictionary<Thread, MapData>(100); // Fixed capacity thread local storage hack!!


        public void Generate(Point3 start, Point3 chunkSize, int sampleResolution, UniformVoxelData outData)
        {
            MapData mData;

            // Maybe not thread safe :s
            // Lock for now
            lock (cacheData)
            {
                if (!cacheData.ContainsKey(Thread.CurrentThread))
                {
                    // Magic
                    cacheData[Thread.CurrentThread] = new MapData(chunkSize.X);
                    if (cacheData.Count > 90) throw new Exception("WARNING might run into threading errors because resize");
                }
                mData = cacheData[Thread.CurrentThread];
            }
            voxelWorldGenerator.GenerateMapData(mData, start.ToVector3().TakeXZ(), sampleResolution, chunkSize.X, 1);


            voxelWorldGenerator.GenerateVoxelData(mData, sampleResolution, start.Y,
                voxelWorldGenerator.GetMaterialsDictionary(), 1, outData.Data,out outData.isEmpty);

        }
    }

    public class MapData
    {
        public readonly float[,] HeightMap;
        public readonly Color[] ColorMap;
        public float Min;
        public float Max;


        public MapData(float[,] heightMap, Color[] colorMap, float min, float max)
        {
            HeightMap = heightMap;
            ColorMap = colorMap;
            Min = min;
            Max = max;
        }

        public MapData(int size)
        {
            HeightMap = new float[size, size];
            ColorMap = new Color[size * size];
            Min = 0;
            Max = 0;
        }

    }
    [System.Serializable]
    public struct TerrainType
    {
        public float Height;
        public Color Color;
    }
}