﻿using System;
using System.Linq;
using Assets.MarchingCubes.Rendering;
using Assets.MarchingCubes.SdfModeling;
using Assets.MarchingCubes.VoxelWorldMVP;
using Assets.MarchingCubes.VoxelWorldMVP.Octrees;
using Assets.MarchingCubes.World;
using LibNoise;
using MHGameWork.TheWizards;
using MHGameWork.TheWizards.DualContouring.Terrain;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.MarchingCubes.Scenes
{
    public class SDFRenderingTestScript : MonoBehaviour
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
            createSDFPrimitives(new Vector3(0, 0, 0));

            createPerlinNoiseTerrain(new Vector3(0, 0, 0));

            createPerlinNoise(new Vector3(0, 0, 50));

            createSDFWithNoise(new Vector3(50, 50, 0));

            createLocalityPrincipleDemo(new Vector3(100, 0, 0));


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
            var mat = new VoxelMaterial(Color.gray);
            var size = new Vector3(1, 1, 1) * 32;
            world.RunKernel1by1(vector3.ToFloored(), (vector3 + size).ToCeiled(), (v,p) =>
            {
                var coords = p.ToVector3() * 0.11f;
                v.Density = (float)Utils.GradientCoherentNoise3D( coords.x, coords.y,coords.z,0,QualityMode.Medium); // TODO check if this still works
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
           
            var mat = new VoxelMaterial(Color.gray);
            var size = new Vector3(1, 1, 1) * 32;
            world.RunKernel1by1(vector3.ToFloored(), (vector3 + size).ToCeiled(), (v, p) =>
            {
                var coords = p.ToVector3() * 0.33f;
                v.Density = (float)Utils.GradientCoherentNoise3D(coords.x, coords.y, coords.z, 0, QualityMode.Medium)*3; // TODO check if this still worksperlin.Noise(coords.x, coords.y, coords.z)*3;
                v.Density += s2.Sdf(p);
                v.Material = mat;
                return v;
            }, 123);
        }


        private void createLocalityPrincipleDemo(Vector3 vector3)
        {
            var s2 = new Ball(vector3 + new Vector3(1, 1, 1) * 32, 24);
          
            var mat = new VoxelMaterial(Color.gray);
            var size = new Vector3(1, 1, 1) * 64;
            world.RunKernel1by1(vector3.ToFloored(), (vector3 + size).ToCeiled(), (v, p) =>
            {
                var coords = p.ToVector3() * 0.33f;
                v.Density = (float)Utils.GradientCoherentNoise3D(coords.x, coords.y, coords.z, 0, QualityMode.Medium)*3; // TODO check if this still worksperlin.Noise(coords.x, coords.y, coords.z) * 3;
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

                    var n  = (float)Utils.GradientCoherentNoise3D(coords.x, coords.y, coords.z, 0, QualityMode.Medium)*0.5f; // TODO check if this still worksperlin.Noise(coords.x, coords.y, coords.z) * range*0.5f;
                    n += s2.Sdf(p);
                    if (n < v.Density) v.Density = n;

                    v.Material = mat;
                    return v;
                }, 123);
            }

        }

       
       

    }
}