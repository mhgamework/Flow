using Assets.MarchingCubes.VoxelWorldMVP.Octrees;
using Assets.UnityAdditions;
using DirectX11;
using MHGameWork.TheWizards.DualContouring.Terrain;
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
    public class OctreeVoxelWorld
    {
        private Dictionary<Point3, UniformVoxelData> chunks = new Dictionary<Point3, UniformVoxelData>();
        IWorldGenerator generator;
        private int depth;

        public Point3 ChunkSize { get; private set; }

        public OctreeNode Root { get; set; }

        /// <summary>
        /// The depth is used to determine the size of the world
        /// a world of size depth has 2^depth leaf chunks
        /// </summary>
        /// <param name="generator"></param>
        /// <param name="chunkSize"></param>
        /// <param name="depth"></param>
        public OctreeVoxelWorld(IWorldGenerator generator, int chunkSize, int depth)
        {
            this.generator = generator;
            ChunkSize = new Point3(chunkSize, chunkSize, chunkSize);
            this.depth = depth;

            Root = new OctreeNode();
            Root.Depth = 0;
            var helper = new ClipMapsOctree<OctreeNode>();
            var maxSize = (int)Math.Pow(2, depth) * chunkSize;

            Root = helper.Create(maxSize, maxSize);
        }


        //public UniformVoxelData GetChunk(Point3 chunkCoord)
        //{
        //    UniformVoxelData ret;
        //    if (chunks.TryGetValue(chunkCoord, out ret)) return ret;

        //    ret = generator.Generate(chunkCoord.Multiply(ChunkSize), ChunkSize + new Point3(1, 1, 1));
        //    chunks.Add(chunkCoord, ret);

        //    return ret;

        //}

        //public void ForChunksInRange(Point3 minInclusive, Point3 maxInclusive, Action<Point3, UniformVoxelData> act)
        //{
        //    maxInclusive += new Point3(1, 1, 1); // Make exclusive :p
        //    var inverseSize = new Vector3(1f / ChunkSize.X, 1f / ChunkSize.Y, 1f / ChunkSize.Z);
        //    var minInclusiveChunk = Point3.Floor(minInclusive.ToVector3().Multiply(inverseSize));
        //    var maxExclusiveChunk = Point3.Ceil(maxInclusive.ToVector3().Multiply(inverseSize));
        //    for (int x = minInclusiveChunk.X; x < maxExclusiveChunk.X; x++)
        //        for (int y = minInclusiveChunk.Y; y < maxExclusiveChunk.Y; y++)
        //            for (int z = minInclusiveChunk.Z; z < maxExclusiveChunk.Z; z++)
        //            {
        //                act(new Point3(x, y, z), GetChunk(new Point3(x, y, z)));
        //            }

        //}
        public OctreeNode GetNode(Point3 nodeLowerLeft, int nodeDepth)
        {
            var data = generator.Generate(nodeLowerLeft, ChunkSize + new Point3(1,1,1), 1 << (this.depth - nodeDepth));

            return new OctreeNode()
            {
                Depth = nodeDepth,
                LowerLeft = nodeLowerLeft,
                VoxelData = data
            };
        }
    }
}
