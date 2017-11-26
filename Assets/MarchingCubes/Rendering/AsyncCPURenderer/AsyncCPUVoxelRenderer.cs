using System.Collections.Generic;
using System.Linq;
using Assets.MarchingCubes.VoxelWorldMVP;
using Assets.MarchingCubes.VoxelWorldMVP.Octrees;
using UnityEngine;
using UnityEngine.Profiling;

namespace Assets.MarchingCubes.Rendering
{
    /// <summary>
    /// TODO: Hide/Show is not idempotent. Probably now called exactly once correctly by other classes
    /// but is dangerous
    /// TODO: concurrentvoxelgenerator is holding chunks in cache until shown, it could be that chunks
    /// are going into the generator but never being shown, leaking memory
    /// TODO: concurrentvoxelgenerator currently seems to be a single parallel thread, 
    /// which could be improved
    /// </summary>
    public class AsyncCPUVoxelRenderer
    {
        private Material TemplateMaterial;
        private Dictionary<Color, Material> materialsDictionary;

        private ConcurrentVoxelGenerator concurrentVoxelGenerator;
        private readonly VoxelChunkRendererPoolScript chunkPool;
        private readonly OctreeVoxelWorld octreeVoxelWorld;
        private readonly Transform transform;

        public AsyncCPUVoxelRenderer(ConcurrentVoxelGenerator concurrentVoxelGenerator,
            VoxelChunkRendererPoolScript chunkPool,
            List<VoxelMaterial> voxelMaterials,
            OctreeVoxelWorld octreeVoxelWorld,
            Transform transform, Material templateMaterial)
        {
            this.concurrentVoxelGenerator = concurrentVoxelGenerator;
            this.chunkPool = chunkPool;
            this.octreeVoxelWorld = octreeVoxelWorld;
            this.transform = transform;
            TemplateMaterial = templateMaterial;

            this.materialsDictionary = voxelMaterials.ToDictionary(v => v.color, c =>
            {
                var mat = new Material(TemplateMaterial);
                mat.color = c.color;
                return mat;
            });
        }

        /// <summary>
        /// Prepare a chunk to be rendered. Should generate all render data offscreen.
        /// Results in CanShowChunk becoming true
        /// </summary>
        /// <param name="tempTaskList"></param>
        public void PrepareShowChunk(List<ConcurrentVoxelGenerator.Task> tempTaskList)
        {
            concurrentVoxelGenerator.SetRequestedChunks(tempTaskList);

        }

        public bool CanShowChunk(OctreeNode node)
        {
            return concurrentVoxelGenerator.HasNodeData(node);
        }

        /// <summary>
        /// Warning: Allthough this is called show/hide, these methods are currently
        /// probably NOT idempotent.
        /// </summary>
        public VoxelChunkRendererScript ShowChunk(OctreeNode node, out int frame)
        {
            var result = concurrentVoxelGenerator.GetNodeData(node);
            concurrentVoxelGenerator.RemoveNodeData(node);

            var ret = createRenderData(result); ;

            activateRenderdata(node, ret);

            frame = result.Frame;

            return ret;
        }
        /// <summary>
        /// Warn: Not idempotent!!
        /// </summary>
        /// <param name="renderObject"></param>
        public void HideChunk(VoxelChunkRendererScript renderObject)
        {
            chunkPool.ReleaseChunk(renderObject);
        }

        private VoxelChunkRendererScript createRenderData(ConcurrentVoxelGenerator.Result result)
        {
            Profiler.BeginSample("RequestChunk");

            var comp = chunkPool.RequestChunk();

            Profiler.EndSample();

            Profiler.BeginSample("SetToUnity");


            comp.AutomaticallyGenerateMesh = false;
            comp.MaterialsDictionary = materialsDictionary;
            comp.setMeshToUnity(result.data);
            comp.transform.SetParent(transform);
            comp.gameObject.SetActive(true);

            Profiler.EndSample();

            return comp;
        }

        private void activateRenderdata(OctreeNode node, VoxelChunkRendererScript renderDataOrNull)
        {
            var comp = renderDataOrNull;
            comp.MaterialsDictionary = materialsDictionary;
            comp.SetWorldcoords(node.LowerLeft, node.Size / (float)(octreeVoxelWorld.ChunkSize.X)); // TOOD: DANGEROES

            comp.transform.SetParent(transform);
            //comp.gameObject.SetActive(true);
        }


  
    }
}