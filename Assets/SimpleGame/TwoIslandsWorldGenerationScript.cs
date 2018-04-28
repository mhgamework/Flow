using System;
using System.Linq;
using Assets.MarchingCubes.Rendering;
using Assets.MarchingCubes.SdfModeling;
using Assets.MarchingCubes.VoxelWorldMVP;
using Assets.MarchingCubes.VoxelWorldMVP.Octrees;
using Assets.MarchingCubes.World;
using MHGameWork.TheWizards;
using MHGameWork.TheWizards.DualContouring.Terrain;
using TreeEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.MarchingCubes.Scenes
{
    public class TwoIslandsWorldGenerationScript : MonoBehaviour
    {
        public VoxelRenderingEngineScript VoxelRenderingEngine;


        private IEditableVoxelWorld world;
        private SDFWorldEditingService editingService;

        public VoxelWorldGenerator tempVoxelWorldGenerator;

        [NonSerialized]
        private bool initialized = false;


        void OnEnable()
        {
            Debug.Log("Enabling");
            world = VoxelRenderingEngine.GetWorld();
            editingService = new SDFWorldEditingService();
        }

        public void Update()
        {
            if (Time.frameCount < 4) return;
            if (initialized) return;
            initialized = true;
            CreateScene();

        }

        private void CreateScene()
        {

            //var scale = 1f/25;
            //var scale = 1f/10;
            var scale = 1f/1;

            var vector3 = new Vector3(100,100,100)*scale;

            float island1Size;
            Vector3 island1Pos;

            island1Size = 100f * scale;
            island1Pos = vector3 + new Vector3(island1Size, island1Size, island1Size)*2;

            addIsland(island1Pos, island1Size, vector3, scale);

            //island1Size = 100f * scale*0.5f;
            //island1Pos = vector3 + new Vector3(island1Size, island1Size, island1Size)*2;

            //addIsland(island1Pos, island1Size, vector3, scale*0.5f);

            //island1Size = 100f * scale * 0.5f*0.5f;
            //island1Pos = vector3 + new Vector3(island1Size, island1Size, island1Size)*2;

            //addIsland(island1Pos, island1Size, vector3, scale*0.5f*0.5f);



            //var island2Size = 100f * scale;
            //var island2Pos = vector3 + new Vector3(island2Size *6f, island2Size, island2Size) ;


            //var island2 = new Ball(island2Pos, island2Size);

            //editingService.AddSDFObject(world, island2, new Bounds(island2Pos, new Vector3(1, 1, 1) * island2Size * 2), new VoxelMaterial(Color.red), 20);

            //createSDFPrimitives(new Vector3(0, 0, 0));

            //createPerlinNoiseTerrain(new Vector3(0, 0, 0));

            //createPerlinNoise(new Vector3(0, 0, 50));

            //createSDFWithNoise(new Vector3(50, 50, 0));

            //createLocalityPrincipleDemo(new Vector3(100, 0, 0));


        }

        private void addIsland(Vector3 island1Pos, float island1Size, Vector3 vector3, float scale)
        {
            var island1 = new Ball(island1Pos, island1Size);

            //editingService.AddSDFObject(world, island1, new Bounds(island1Pos, new Vector3(1, 1, 1) * island1Size * 2), new VoxelMaterial(Color.red), 20);


            var s2 = new Ball(vector3 + new Vector3(1, 1, 1) * 16, 11);
            var perlin = new TreeEditor.Perlin();
            perlin.SetSeed(0);
            var mat = new VoxelMaterial(Color.gray);
            var size = new Vector3(1, 1, 1) * 32;
            world.RunKernel1by1((island1Pos - Vector3.one * island1Size * 4).ToFloored(),
                (island1Pos + Vector3.one * island1Size *4).ToCeiled(), (v, p) =>
                {
                    var realP = p.ToVector3();
                    realP.y = (realP.y - island1Pos.y)/2f + island1Pos.y;

                    var coords = realP * 0.03f / scale;
                    
                    v.Density = island1.Sdf(realP) + Mathf.Max(-(realP.y - (island1Pos.y)), 0);// + Mathf.Max((p.Y - (island1Pos.y + 10)), 0)*0.3f;

                    v.Density = Mathf.Max(v.Density,  (realP.y - (island1Pos.y)));
                    //v.Density += perlin.Noise(coords.x, coords.y, coords.z) * island1Size * 0.3f;
                    v.Material = mat;
                    return v;
                }, 123);
        }

        private void createSDFPrimitives(Vector3 vector3)
        {
            // Imperative

            var s2 = new Ball(vector3 + new Vector3(0, 0, 0), 5);

            editingService.AddSDFObject(world, s2, new Bounds(vector3 + new Vector3(0, 0, 0), new Vector3(1, 1, 1) * 10), new VoxelMaterial(Color.red), 20);


            for (int i = 0; i < 5; i++)
            {
                var sphere = new Ball(vector3 + new Vector3(30, 30, 30)*i, 20);

                editingService.AddSDFObject(world, sphere, new Bounds(vector3 + new Vector3(30, 30, 30)*i, new Vector3(1, 1, 1) * 20 * 2 * 1.5f), new VoxelMaterial(Color.red), 20);

            }

        }

        private void createPerlinNoise(Vector3 vector3)
        {
            var perlin = new TreeEditor.Perlin();
            perlin.SetSeed(0);
            var mat = new VoxelMaterial(Color.gray);
            var size = new Vector3(1, 1, 1) * 32;
            world.RunKernel1by1(vector3.ToFloored(), (vector3 + size).ToCeiled(), (v,p) =>
            {
                var coords = p.ToVector3() * 0.11f;
                v.Density = perlin.Noise(coords.x,coords.y,coords.z);
                v.Material = mat;
                return v;
            }, 123);
        }
        private void createPerlinNoiseTerrain(Vector3 vector3)
        {


            bool isEmpty;

            var world = (OctreeVoxelWorld)this.world;
            var mapData = new MapData(world.ChunkSize.X+2);

            var helper = new ClipMapsOctree<OctreeNode>();
            
            helper.VisitTopDown(world.GetNode(new DirectX11.Point3(world.getNodeResolution(8)*world.ChunkSize.X*3, 0, 0), 8), n =>
            {
                n = world.GetNode(n.LowerLeft, n.Depth);
                world.makeUnsharedChunk(n);
                tempVoxelWorldGenerator.GenerateMapData(mapData, n.LowerLeft.ToVector3().TakeXZ(),
                    world.getNodeResolution(n.Depth), world.ChunkSize.X + 2, 0.1f);

                tempVoxelWorldGenerator.GenerateVoxelData(mapData, world.getNodeResolution(n.Depth),n.LowerLeft.Y,
                    tempVoxelWorldGenerator.GetMaterialsDictionary(), 0.1f, n.VoxelData.Data, out isEmpty);
            });



        }

        private void createSDFWithNoise(Vector3 vector3)
        {
            var s2 = new Ball(vector3 + new Vector3(1, 1, 1)*16, 11);
            var perlin = new TreeEditor.Perlin();
            perlin.SetSeed(0);
            var mat = new VoxelMaterial(Color.gray);
            var size = new Vector3(1, 1, 1) * 32;
            world.RunKernel1by1(vector3.ToFloored(), (vector3 + size).ToCeiled(), (v, p) =>
            {
                var coords = p.ToVector3() * 0.33f;
                v.Density = perlin.Noise(coords.x, coords.y, coords.z)*3;
                v.Density += s2.Sdf(p);
                v.Material = mat;
                return v;
            }, 123);
        }


        private void createLocalityPrincipleDemo(Vector3 vector3)
        {
            var s2 = new Ball(vector3 + new Vector3(1, 1, 1) * 32, 24);
            var perlin = new TreeEditor.Perlin();
            perlin.SetSeed(0);
            var mat = new VoxelMaterial(Color.gray);
            var size = new Vector3(1, 1, 1) * 64;
            world.RunKernel1by1(vector3.ToFloored(), (vector3 + size).ToCeiled(), (v, p) =>
            {
                var coords = p.ToVector3() * 0.33f;
                v.Density = perlin.Noise(coords.x, coords.y, coords.z) * 3;
                v.Density += s2.Sdf(p);
                v.Material = mat;
                return v;
            }, 123);


            Random.InitState(0);
            
            for (int i = 0; i < 10; i++)
            {
                var offset = Random.onUnitSphere * Random.Range(35, 45);
                var range = Random.Range(3, 6);

                s2 = new Ball(vector3 + new Vector3(1, 1, 1) * 32 + offset, range);
                var center = vector3 + new Vector3(1, 1, 1) * 32 + offset;
                world.RunKernel1by1((center-Vector3.one*range*1.3f).ToFloored(), (center + Vector3.one * range * 1.3f).ToCeiled(), (v, p) =>
                {
                    var coords = p.ToVector3() * 0.5f;

                    var n  = perlin.Noise(coords.x, coords.y, coords.z) * range*0.5f;
                    n += s2.Sdf(p);
                    if (n < v.Density) v.Density = n;

                    v.Material = mat;
                    return v;
                }, 123);
            }

        }

       
       

    }
}