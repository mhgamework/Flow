﻿using System;
using Assets.MarchingCubes.Rendering.ClipmapsOctree;
using Assets.MarchingCubes.VoxelWorldMVP;
using Assets.MarchingCubes.VoxelWorldMVP.Octrees;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.MarchingCubes.Rendering
{
    public class VoxelRenderingEngineScript : MonoBehaviour, IVoxelRenderingEngineConfigProvider
    {
        public float RenderScale = 1;
        public bool UseGpuRenderer = false;
        public UnityEngine.ComputeShader GPUShader;

        public OctreeVoxelWorldScript World;

        public Camera LODCamera;
        [Range(0.5f,4)]
        public float LODDistanceFactor = 1.2f;

        //TODO add render scale

        public AnimationCurve LodDistanceCurve;
        public float LodDistanceCurveEnd = 1;

        public Text DebugText;

        public Material TemplateMaterial;
        public Material VertexColorMaterial;

        public bool enableMultithreadingWorldGen = true;
        public bool enableMultithreadingMeshGen = true;

        private ClipmapsOctreeService clipmapsOctreeService;


        [NonSerialized]
        private bool init = false;
        private AsyncCPUVoxelRenderer renderingService;

        IConcurrentVoxelGenerator concurrentGenerator;
        private OctreeVoxelWorld octreeVoxelWorld;


        public void Start()
        {


        }
        private void initialize()
        {
            if (init) return;
            init = true;

            var chunkPoolObject = new GameObject("ChunkPool");
            chunkPoolObject.transform.SetParent(transform);
            var chunkPool = chunkPoolObject.AddComponent<VoxelChunkRendererPoolScript>();

            var rendererObject = new GameObject("Renderer");
            rendererObject.transform.SetParent(transform);

            octreeVoxelWorld = World.CreateNewWorld();
            // new OctreeVoxelWorld(new ConstantVoxelWorldGenerator(-1, new VoxelMaterial(Color.black)), 8, 6);//World.CreateNewWorld();


            if (UseGpuRenderer)
                concurrentGenerator = createGpuRenderer(octreeVoxelWorld);
            else
                concurrentGenerator = createCPURenderer();

            renderingService = new AsyncCPUVoxelRenderer(
                concurrentGenerator,
                chunkPool,
                null,
                octreeVoxelWorld,
                rendererObject.transform,
                this,
                !enableMultithreadingWorldGen
            );

            clipmapsOctreeService = new ClipmapsOctreeService(octreeVoxelWorld, renderingService, LodDistanceCurve, LodDistanceCurveEnd);

        }

        public OctreeVoxelWorld GetWorld()
        {
            initialize();
            return octreeVoxelWorld;
        }


        private IConcurrentVoxelGenerator createGpuRenderer(OctreeVoxelWorld world)
        {
            return new GpuVoxelMeshGenerator(GPUShader, world);
        }

        private IConcurrentVoxelGenerator createCPURenderer()
        {

            var marchingCubesService = new MarchingCubesService();
            var meshGenerator = new VoxelChunkMeshGenerator(marchingCubesService);

            var ret = new ConcurrentVoxelGenerator(meshGenerator, !enableMultithreadingMeshGen);
            ret.Start(); // WARN dont forget

            return ret;
        }


        public void Update()
        {
            initialize();
            clipmapsOctreeService.LODDistanceFactor = LODDistanceFactor;
            clipmapsOctreeService.UpdateRendererState(LODCamera.transform.position / RenderScale);

            concurrentGenerator.Update();

            renderingService.Update();

            if (DebugText)
                DebugText.text = renderingService.UnavailableChunks.ToString();
        }

        public Vector3 ToVoxelSpace(Vector3 min)
        {
            return min / RenderScale;
        }

        public Material GetTemplateMaterial()
        {
            return TemplateMaterial;
        }

        public Material GetVertexColorMaterial()
        {
            return VertexColorMaterial;
        }

        public float GetRenderScale()
        {
            return RenderScale;
        }
    }
}