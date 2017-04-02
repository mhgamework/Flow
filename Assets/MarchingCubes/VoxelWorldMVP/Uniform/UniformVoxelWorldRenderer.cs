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
    /// Builds a uniform grid
    /// 
    /// Just a service, not a unity component
    /// </summary>
    public class UniformVoxelWorldRenderer
    {
        UniformVoxelWorld world;

        private Dictionary<Point3, VoxelChunkRenderer> chunks = new Dictionary<Point3, VoxelChunkRenderer>();
        Transform container;

        public UniformVoxelWorldRenderer(UniformVoxelWorld world, Transform container)
        {
            this.world = world;
            this.container = container;
        }

        public void createRenderers(Point3 chunksToRender,Material[] voxelMaterial)
        {
            Point3.ForEach(chunksToRender, p =>
             {
                 var obj = new GameObject();
                 obj.transform.SetParent(container);
                 var subRenderer = obj.AddComponent<VoxelChunkRenderer>();
                 subRenderer.MaterialsDictionary = voxelMaterial.ToDictionary(m => m.color, m =>m);
                 //subRenderer.VoxelMaterial = voxelMaterial;
                 subRenderer.SetChunk(world.GetChunk(p));
                 subRenderer.SetWorldcoords(p, world.ChunkSize);
                 chunks.Add(p, subRenderer);

               
             });
        }


    }
}
