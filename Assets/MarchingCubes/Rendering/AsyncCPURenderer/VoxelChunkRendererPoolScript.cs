using System.Collections.Generic;
using UnityEngine;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    public class VoxelChunkRendererPoolScript : MonoBehaviour
    {

        private List<VoxelChunkRendererScript> freeChunks = new List<VoxelChunkRendererScript>();

        public VoxelChunkRendererScript RequestChunk()
        {
            if (freeChunks.Count == 0)
                allocateNewChunk();
            var ret = freeChunks[freeChunks.Count - 1];
            freeChunks.RemoveAt(freeChunks.Count-1);


            //renderObject.name = "Node " + result.Frame + " " + node.LowerLeft + " " + node.Size + " V: " +
            //                result.data.vertices.Count;

            return ret;
        }

        public void ReleaseChunk(VoxelChunkRendererScript chunk)
        {
            freeChunks.Add(chunk);
            chunk.gameObject.SetActive(false);
            chunk.transform.SetParent(transform);
            
        }

        private void allocateNewChunk()
        {
            var renderObject = new GameObject();
            renderObject.SetActive(false);


            var comp = renderObject.AddComponent<VoxelChunkRendererScript>();
            comp.AutomaticallyGenerateMesh = false;
            comp.transform.SetParent(transform);
            comp.initialize();

            freeChunks.Add(comp);
        }
    }
}