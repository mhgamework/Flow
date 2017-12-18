using System.Collections.Generic;
using Assets.Reusable;
using Assets.Reusable.Threading;
using UnityEngine;
using UnityEngine.Profiling;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    public class VoxelChunkRendererPoolScript : MonoBehaviour
    {
        private ConcurrentObjectPool<VoxelChunkRendererScript> freeChunksPool;
        public void Start()
        {
            freeChunksPool =
                new ConcurrentObjectPool<VoxelChunkRendererScript>(allocateNewChunk, 100, 100,
                    MainThreadDispatcher.Instance);
        }

        public VoxelChunkRendererScript RequestChunk()
        {
            return freeChunksPool.Take();
        }

     

        public void ReleaseChunk(VoxelChunkRendererScript chunk)
        {
            chunk.gameObject.SetActive(false);
            chunk.transform.SetParent(transform);
            freeChunksPool.Release(chunk);

        }

        private VoxelChunkRendererScript allocateNewChunk()
        {
            var renderObject = new GameObject();
            renderObject.SetActive(false);


            var comp = renderObject.AddComponent<VoxelChunkRendererScript>();
            comp.AutomaticallyGenerateMesh = false;
            comp.transform.SetParent(transform);
            comp.initialize();
            return comp;
        }
    }
}