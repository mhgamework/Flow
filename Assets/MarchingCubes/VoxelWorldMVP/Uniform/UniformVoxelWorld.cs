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
    public class UniformVoxelWorld : IEditableVoxelWorld
    {
        private Dictionary<Point3, UniformVoxelData> chunks = new Dictionary<Point3, UniformVoxelData>();
        IWorldGenerator generator;

        public Point3 ChunkSize { get; private set; }

        public UniformVoxelWorld(IWorldGenerator generator, Point3 chunkSize)
        {
            this.generator = generator;
            ChunkSize = chunkSize;
        }

        public UniformVoxelData GetChunk(Point3 chunkCoord)
        {
            UniformVoxelData ret;
            if (chunks.TryGetValue(chunkCoord, out ret)) return ret;

            ret = generator.Generate(chunkCoord.Multiply(ChunkSize), ChunkSize + new Point3(1, 1, 1), 1);
            chunks.Add(chunkCoord, ret);

            return ret;

        }

        public void ForChunksInRange(Point3 minInclusive, Point3 maxInclusive, Action<Point3, UniformVoxelData> act)
        {
            maxInclusive += new Point3(1, 1, 1); // Make exclusive :p
            var inverseSize = new Vector3(1f / ChunkSize.X, 1f / ChunkSize.Y, 1f / ChunkSize.Z);
            var minInclusiveChunk = Point3.Floor(minInclusive.ToVector3().Multiply(inverseSize));
            var maxExclusiveChunk = Point3.Ceil(maxInclusive.ToVector3().Multiply(inverseSize));
            for (int x = minInclusiveChunk.X; x < maxExclusiveChunk.X; x++)
                for (int y = minInclusiveChunk.Y; y < maxExclusiveChunk.Y; y++)
                    for (int z = minInclusiveChunk.Z; z < maxExclusiveChunk.Z; z++)
                    {
                        act(new Point3(x, y, z), GetChunk(new Point3(x, y, z)));
                    }

        }

        public void RunKernel1by1(Point3 minInclusive, Point3 maxInclusive, Func<VoxelData, Point3, VoxelData> act, int frame)
        {
            ForChunksInRange(minInclusive, maxInclusive, (p, c) =>
            {
                var offset = p.Multiply(ChunkSize);
                //var localHit = point - offset;

                c.Data.ForEach((vx, px) =>
                {
                    c.Data[px] = act(vx, px + offset);
                });
                c.LastChangeFrame = frame;

            });
        }
    }
}
