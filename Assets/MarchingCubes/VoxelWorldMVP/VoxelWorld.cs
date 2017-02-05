using Assets.UnityAdditions;
using DirectX11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    /// <summary>
    /// Holds data for entire voxel world
    /// </summary>
    public class VoxelWorld
    {
        private Dictionary<Point3, UniformVoxelData> chunks = new Dictionary<Point3, UniformVoxelData>();
        IWorldGenerator generator;

        public Point3 ChunkSize { get; private set; }

        public VoxelWorld(IWorldGenerator generator, Point3 chunkSize)
        {
            this.generator = generator;
            ChunkSize = chunkSize;
        }

        public UniformVoxelData GetChunk(Point3 chunkCoord)
        {
            UniformVoxelData ret;
            if (chunks.TryGetValue(chunkCoord, out ret)) return ret;

            ret = generator.Generate(chunkCoord.Multiply(ChunkSize), ChunkSize+ new Point3(1,1,1));
            chunks.Add(chunkCoord, ret);

            return ret;

        }
    }
}
