using System;
using System.Collections.Generic;
using Assets.MarchingCubes.Domain;
using Assets.MarchingCubes.VoxelWorldMVP;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.Flow.FireSpirit
{
    public class DynamicRenderingService : Singleton<DynamicRenderingService>, IDynamicRenderingService
    {
        private List<DynamicVoxelEntity> entities = new List<DynamicVoxelEntity>();
        public VoxelChunkRendererScript TemplateRenderer;

        private Dictionary<Color, Material> dict = new Dictionary<Color, Material>();

        SdfSampler sampler = new SdfSampler();
        IVoxelMeshGenerator gen = new VoxelChunkMeshGenerator(new MarchingCubes.MarchingCubesService());

        public void AddMaterial(Color col, Material mat)
        {
            dict.Add(col, mat);
        }
        public DynamicVoxelEntity CreateEntity(SdfFunction sdf, float size, Vector3 center, float sampleInterval)
        {
            var ret = new DynamicVoxelEntity(sdf, size);
            ret.Center = center;
            ret.SampleInterval = sampleInterval;
            ret.Renderer = Instantiate(TemplateRenderer, transform);
            ret.Renderer.AutomaticallyGenerateMesh = false;
            ret.Renderer.MaterialsDictionary = new Dictionary<Color, Material>();
            foreach (var el in dict)
                ret.Renderer.MaterialsDictionary.Add(el.Key, el.Value);

            entities.Add(ret);
            return ret;
        }
        public void Update()
        {
            foreach (var ent in entities)
            {
                updateEntityRenderer(ent);
            }
        }

        private void updateEntityRenderer(DynamicVoxelEntity entity)
        {
            var voxelRenderer = entity.Renderer;

       
            var min = entity.Center + (new Vector3(1, 1, 1) * -entity.Size);
            var max = min + new Vector3(1, 1, 1) * entity.Size * 2;
            var maxFloored = (max / entity.SampleInterval).ToCeiled().ToVector3() * entity.SampleInterval;

            var thePos = (min / entity.SampleInterval).ToFloored().ToVector3() * entity.SampleInterval;

            var data = sampler.CreateEmptyData(Mathf.CeilToInt((maxFloored - min).MaxComponent()));
            sampler.SampleSdf(entity.Sdf, thePos, entity.SampleInterval, data);
            voxelRenderer.transform.position = thePos;
            voxelRenderer.transform.localScale = new Vector3(1, 1, 1) * entity.SampleInterval;

            var mesh = VoxelMeshData.CreatePreallocated();
            gen.GenerateMeshFromVoxelData(data, mesh);

            voxelRenderer.setMeshToUnity(mesh);
        }
    }
}