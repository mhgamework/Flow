using DirectX11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    /// <summary>
    /// Holds state for renderers of the voxel world
    /// Responsible for updating the renderers of the voxel world
    /// </summary>
    public class VoxelWorldRenderer
    {
        VoxelWorld world;

        private Dictionary<Point3, VoxelChunkRenderer> chunks = new Dictionary<Point3, VoxelChunkRenderer>();
        Transform container;

        public VoxelWorldRenderer(VoxelWorld world, Transform container)
        {
            this.world = world;
            this.container = container;
        }

        public void createRenderers(Point3 chunksToRender)
        {
            Point3.ForEach(chunksToRender, p =>
             {
                 var obj = new GameObject();
                 obj.transform.SetParent(container);
                 var subRenderer = obj.AddComponent<VoxelChunkRenderer>();
                 subRenderer.SetChunk(world.GetChunk(p));
                 subRenderer.SetWorldcoords(p, world.ChunkSize);
                 chunks.Add(p, subRenderer);

                 //TODO: make a unity scene and  test
                 //TODO: implement a world generator
             });
        }


    }
}
