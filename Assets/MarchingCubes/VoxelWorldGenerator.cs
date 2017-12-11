using System;
using System.Collections.Generic;
using System.Linq;
using Assets.MarchingCubes.Rendering;
using Assets.MarchingCubes.SdfModeling;
using Assets.MarchingCubes.VoxelWorldMVP;
using Assets.UnityAdditions;
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

        public DrawMode Mode;
        [Range(1, 6)]
        public int LodLevel;
        public bool AutoGenerate = false;
        public int WorldSize = 10;
        public float WorldScale = 1;
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
            Terrain2DValueFbm
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

            persistenceDict.Add(GeneratorType.Terrain3DValueFbm, () => fromDelegate(theTerrain3D));
            persistenceDict.Add(GeneratorType.Terrain2DValueFbm, () => fromDelegate(theTerrain2D));


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

        public void Generate()
        {
            var map = noise(WorldSize, Octaves, WorldScale, Persistence, Lacunarity, Seed, Offset);

            if (GenerateCalibrationNoise)
                for (int x = 0; x < WorldSize; x++)
                    for (int z = 0; z < WorldSize; z++)
                    {
                        map[x, z] = new Vector2((x + Offset.x) / WorldScale, (z + Offset.y) / WorldScale).magnitude;
                    }
            var colorMap = new Color[WorldSize * WorldSize];
            for (int x = 0; x < WorldSize; x++)
                for (int z = 0; z < WorldSize; z++)
                {
                    var height = map[x, z];
                    for (int i = 0; i < Regions.Length; i++)
                    {
                        if (height <= Regions[i].Height)
                        {
                            colorMap[x + z * WorldSize] = Regions[i].Color;
                            break;
                        }


                    }
                }
            if (Mode == DrawMode.Noise)
                WorldDisplay.Instance.DrawTexture(WorldDisplay.TextureFromHeightMap(map));
            else if (Mode == DrawMode.Color)
                WorldDisplay.Instance.DrawTexture(WorldDisplay.TextureFromColorMap(colorMap, WorldSize, WorldSize));
            if (DrawMesh)
            {
                var resolution = (int)Math.Round(Math.Pow(2, LodLevel-1));
                WorldDisplay.Instance.DrawMesh(GenerateMesh(map, colorMap, resolution), new Vector3(10 * resolution, HeightMultiplier, 10 * resolution), (WorldSize / resolution)-1);
            }
        }

        private VoxelMeshData GenerateMesh(float[,] map, Color[] colorMap, int resolution)
        {
            var gen = new VoxelChunkMeshGenerator(new MarchingCubesService());

            var chunkSize = WorldSize / resolution;

            var data = new Array3D<VoxelData>(new DirectX11.Point3(chunkSize, chunkSize, chunkSize));
            for (int x = 0; x < chunkSize; x++)
                for (int y = 0; y < chunkSize; y++)
                    for (int z = 0; z < chunkSize; z++)
                    {
                        var height = MeshHightCurve.Evaluate(map[WorldSize - 1 - x * resolution, WorldSize - 1 - z * resolution]);
                        data[new DirectX11.Point3(x, y, z)] = new VoxelData((height - y), null);
                    }

            var mesh = gen.GenerateMeshFromVoxelData(data);
            return mesh;
        }


        public static float[,] noise(int size, int octaves, float scale, float persistence, float lacunarity, int seed, Vector2 offset)
        {
            var test = new TerrainTest();
            var rand = new System.Random(seed);

            Vector2[] octaveOffsets = new Vector2[octaves];
            for (int i = 0; i < octaves; i++)
            {
                float offsetX = rand.Next(-100000, 100000);
                float offsetY = rand.Next(-100000, 100000);
                octaveOffsets[i] = new Vector2(offsetX, offsetY) + offset;
            }

            var min = float.MaxValue;
            var max = float.MinValue;

            float halfSize = size / 2f;

            var map = new float[size, size];
            for (int x = 0; x < size; x++)
                for (int z = 0; z < size; z++)
                {

                    float noiseHeight = 0.0f;
                    float amplitude = 0.5f;
                    float frequency = 1.0f;
                    for (int i = 0; i < octaves; i++)
                    {
                        var sampleX = (x - halfSize) / scale * frequency + octaveOffsets[i].x;
                        var sampleZ = (z - halfSize) / scale * frequency + octaveOffsets[i].y;

                        float n = Mathf.PerlinNoise(sampleX, sampleZ);//* 2 - 1;
                        //float n = test.noised(new Vector3(sampleX, 0, sampleZ)).x * 2 - 1;

                        noiseHeight += amplitude * n; // accumulate values		
                        amplitude *= persistence; // amplitude decrease

                        frequency *= lacunarity; // frequency increase
                    }

                    map[x, z] = noiseHeight;
                    if (noiseHeight < min)
                        min = noiseHeight;
                    if (noiseHeight > max)
                        max = noiseHeight;
                }

            var diff = max - min;
            for (int x = 0; x < size; x++)
                for (int z = 0; z < size; z++)
                {
                    //map[x, z] = (map[x, z] - min) / diff;
                }

            return map;
        }


    }
    [System.Serializable]
    public struct TerrainType
    {
        public float Height;
        public Color Color;
    }
    public class TerrainTest
    {

        float hash(float n) { return fract(sin(n) * 753.5453123f); }
        float hash(Vector3 p)  // replace this by something better
        {
            p = 50.0f * fract(p * 0.3183099f + new Vector3(0.71f, 0.113f, 0.419f));
            return -1.0f + 2.0f * fract(p.x * p.y * p.z * (p.x + p.y + p.z));
        }

        //---------------------------------------------------------------
        // value noise, and its analytical derivatives
        //---------------------------------------------------------------

        public Vector4 noised(Vector3 x)
        {

            var p = floor(x);
            var w = fract(x);
            var u = w.Multiply(w).Multiply(Vector3.one * 3.0f - 2.0f * w);
            var du = 6.0f * w.Multiply(Vector3.one * 1.0f - w);

            float n = p.x + p.y * 157.0f + 113.0f * p.z;

            float a = hash(p + new Vector3(0.0f, 0.0f, 0.0f));
            float b = hash(p + new Vector3(1.0f, 0.0f, 0.0f));
            float c = hash(p + new Vector3(0.0f, 1.0f, 0.0f));
            float d = hash(p + new Vector3(1.0f, 1.0f, 0.0f));
            float e = hash(p + new Vector3(0.0f, 0.0f, 1.0f));
            float f = hash(p + new Vector3(1.0f, 0.0f, 1.0f));
            float g = hash(p + new Vector3(0.0f, 1.0f, 1.0f));
            float h = hash(p + new Vector3(1.0f, 1.0f, 1.0f));

            float k0 = a;
            float k1 = b - a;
            float k2 = c - a;
            float k3 = e - a;
            float k4 = a - b - c + d;
            float k5 = a - c - e + g;
            float k6 = a - b - e + f;
            float k7 = -a + b + c - d + e - f - g + h;

            var first = k0 + k1 * u.x + k2 * u.y + k3 * u.z + k4 * u.x * u.y + k5 * u.y * u.z + k6 * u.z * u.x + k7 * u.x * u.y * u.z;
            var second = du.Multiply(new Vector3(k1, k2, k3) + yzx(u).Multiply(new Vector3(k4, k5, k6)) +
                               zxy(u).Multiply(new Vector3(k6, k4, k5)) + k7 * yzx(u).Multiply(zxy(u)));

            return new Vector4(first, second.x, second.y, second.z);
        }

        public Vector4 fbmd(Vector3 x, float f1)
        {
            const float scale = 1.5f;

            float a = 0.0f;
            float b = 0.5f;
            float f = 1.0f;
            var d = new Vector3();
            for (int i = 0; i < 1; i++) // was 8
            {
                if (f * 1.1f > (1f / f1)) break;// To high frequency

                Vector4 n = noised(f * x * scale);
                a += b * n.x;           // accumulate values		
                d += b * yzw(n) * f * scale; // accumulate derivatives
                b *= 0.5f;             // amplitude decrease
                f *= 1.8f;             // frequency increase
            }

            return new Vector4(a, d.x, d.y, d.z);
        }

        private Vector3 yzx(Vector3 v)
        {
            return new Vector3(v.y, v.z, v.x);
        }
        private Vector3 zxy(Vector3 v)
        {
            return new Vector3(v.z, v.x, v.y);
        }
        public static Vector3 yzw(Vector4 v)
        {
            return new Vector3(v.y, v.z, v.w);
        }
        private Vector3 floor(Vector3 vector3)
        {
            return vector3.ToFloored();
        }

        float fract(float n)
        {
            return n - (float)Math.Truncate(n);
        }

        Vector3 fract(Vector3 n)
        {
            return new Vector3(fract(n.x), fract(n.y), fract(n.z));
        }
        Vector2 fract(Vector2 n)
        {
            return new Vector2(fract(n.x), fract(n.y));
        }
        Vector2 floor(Vector2 n)
        {
            return new Vector2(Mathf.Floor(n.x), Mathf.Floor(n.y));
        }

        float sin(float f)
        {
            return Mathf.Sin(f);
        }


        //const mat2 m = mat2(0.8, -0.6, 0.6, 0.8);
        float hash1(float n)
        {
            return fract(n * 17.0f * fract(n * 0.3183099f));
        }
        float hash1(Vector2 p)
        {
            p = 50.0f * fract(p * 0.3183099f);
            return fract(p.x * p.y * (p.x + p.y));
        }
        Vector3 noised(Vector2 x)
        {
            var p = floor(x);
            var w = fract(x);

            var u = w.Multiply(w).Multiply(w).Multiply(w.Multiply(w * 6.0f - 15.0f * Vector2.one) + 10.0f * Vector2.one);
            var du = 30.0f * w.Multiply(w).Multiply(w.Multiply(w - 2.0f * Vector2.one) + 1.0f * Vector2.one);

            float a = hash1(p + new Vector2(0, 0));
            float b = hash1(p + new Vector2(1, 0));
            float c = hash1(p + new Vector2(0, 1));
            float d = hash1(p + new Vector2(1, 1));

            float k0 = a;
            float k1 = b - a;
            float k2 = c - a;
            float k4 = a - b - c + d;

            var gradient = 2.0f * du.Multiply(new Vector2(k1 + k4 * u.y,
                              k2 + k4 * u.x));
            return new Vector3(-1.0f + 2.0f * (k0 + k1 * u.x + k2 * u.y + k4 * u.x * u.y),
                gradient.x, gradient.y);
        }

        public Vector3 fbmd(Vector2 x)
        {
            const float scale = 1.5f;

            float a = 0.0f;
            float b = 0.5f;
            float f = 1.0f;
            var d = new Vector2();
            for (int i = 0; i < 8; i++)
            {
                //if (f * 1.1f > (1f / f1)) break;// To high frequency

                Vector3 n = noised(x);//f * x * scale);
                a += b * n.x;           // accumulate values		
                d += b * new Vector2(n.y, n.z) * f * scale; // accumulate derivatives
                b *= 0.55f;             // amplitude decrease

                f *= 1.9f;             // frequency increase

                //const mat2 m2 = mat2(0.80, 0.60,
                //    -0.60, 0.80);
                // 0.8 -0.6
                // 0.6 0.8

                // m*x
                x = 1.9f * x;//(x.x * new Vector2(0.8f, 0.6f) + x.y * new Vector2(-0.6f, 0.8f));
            }

            return new Vector4(a, d.x, d.y);
        }


        public float terrainWierd(Vector2 p)
        {


            float a = 0.0f;
            float b = 1.0f;
            Vector2 d = new Vector2();
            for (int i = 0; i < 2; i++)//was 15
            {
                var n = noised(p);
                d += new Vector2(n.y, n.z);
                a += b * n.x / (1.0f + Vector2.Dot(d, d));
                b *= 0.5f;
                // Was m * p
                p = new Vector2(0.8f, -0.6f) * p.x + new Vector2(0.6f, 0.8f) * 2.0f;
            }
            return a;
        }
    }
}