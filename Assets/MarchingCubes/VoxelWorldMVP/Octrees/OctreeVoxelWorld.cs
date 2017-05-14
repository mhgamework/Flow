using Assets.MarchingCubes.VoxelWorldMVP.Octrees;
using Assets.UnityAdditions;
using DirectX11;
using MHGameWork.TheWizards.DualContouring.Terrain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using UnityEngine;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    /// <summary>
    /// Holds data for entire voxel world
    /// </summary>
    public class OctreeVoxelWorld : IEditableVoxelWorld
    {
        IWorldGenerator generator;
        private int depth;
        private ClipMapsOctree<OctreeNode> helper;

        /// <summary>
        /// Chunks data is bigger than actual chunk size, so they overlap for LOD
        /// This value is here so that code depending on it breaks when we remove this behaviour
        /// </summary>
        public const int ChunkOversize = 1;
        private int chunkSize;
        public Point3 ChunkSize { get { return new Point3(chunkSize, chunkSize, chunkSize); } }

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
            this.chunkSize = chunkSize;
            this.depth = depth;

            Root = new OctreeNode();
            Root.Depth = 0;
            var helper = new ClipMapsOctree<OctreeNode>();
            var maxSize = (int)Math.Pow(2, depth) * chunkSize;

            Root = helper.Create(maxSize, maxSize);

            this.helper = new ClipMapsOctree<OctreeNode>();
        }

        public void RunKernel1by1Single(Point3 minInclusive, Point3 maxInclusive, Func<VoxelData, Point3, VoxelData> act, int frame, OctreeNode data)
        {
            var resolution = getNodeResolution(data.Depth);
            var max = maxInclusive - data.LowerLeft;
            var start = minInclusive - data.LowerLeft;
            for (int i = 0; i < 3; i++) max[i] = Math.Min(max[i] / resolution, ChunkSize[i] + 1/* One extra for lod stitching */);//floor
            for (int i = 0; i < 3; i++) start[i] = Math.Max((start[i] + resolution - 1) / resolution, 0); // ceil
            var grid = data.VoxelData.Data;
            for (int x = start.X; x <= max.X; x++)
                for (int y = start.Y; y <= max.Y; y++)
                    for (int z = start.Z; z <= max.Z; z++)
                    {
                        var p = new Point3(x, y, z);
                        var val = grid.GetFast(x, y, z);
                        grid[p] = act(val, new Point3(data.LowerLeft.X + resolution * x, data.LowerLeft.Y + resolution * y, data.LowerLeft.Z + resolution * z));

                    }
            data.VoxelData.LastChangeFrame = frame;
        }

        public void RunKernel1by1(Point3 minInclusive, Point3 maxInclusive, Func<VoxelData, Point3, VoxelData> act, int frame)
        {
            helper.VisitTopDown(Root, n =>
            {
                for (int i = 0; i < 3; i++)
                    if (n.LowerLeft[i] > maxInclusive[i]) // is outside
                        return VisitOptions.SkipChildren;
                for (int i = 0; i < 3; i++)
                    if (n.LowerLeft[i] + n.Size + 1 * getNodeResolution(n.Depth)/* one extra for lod stitching */ < minInclusive[i]) // is outside
                        return VisitOptions.SkipChildren;
                // inside
                allocChunk(n);
                RunKernel1by1Single(minInclusive, maxInclusive, act, frame, n);

                return VisitOptions.Continue;
            });
        }

        public void RunKernelXbyXUnrolled(Point3 minInclusive, Point3 maxInclusive, Func<VoxelData[], Point3, VoxelData> act, int kernelSize, int frame)
        {
            throw new NotImplementedException();
        }


        private void allocChunk(OctreeNode octreeNode)
        {
            if (octreeNode.VoxelData != null) return;
            octreeNode.VoxelData = generator.Generate(octreeNode.LowerLeft, ChunkSize + new Point3(1, 1, 1) + new Point3(1, 1, 1), 1 << (this.depth - octreeNode.Depth));
            if (octreeNode.Depth == depth) return; // leaf node

            helper.Split(octreeNode);

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
        public OctreeNode GetNode(Point3 nodeLowerLeft, int nodeDepth, OctreeNode node)
        {
            for (int i = 0; i < 3; i++)
                if (nodeLowerLeft[i] > node.LowerLeft[i] + node.Size) return null;
            for (int i = 0; i < 3; i++)
                if (nodeLowerLeft[i] < node.LowerLeft[i]) return null;

            allocChunk(node);

            if (nodeLowerLeft == node.LowerLeft && nodeDepth == node.Depth) return node;
            if (nodeDepth == node.Depth) return null; // dont go deeper

            for (int i = 0; i < 8; i++)
            {
                var ret = GetNode(nodeLowerLeft, nodeDepth, node.Children[i]);
                if (ret != null) return ret;
            }

            return null;

        }
        public OctreeNode GetNode(Point3 nodeLowerLeft, int nodeDepth)
        {
            return GetNode(nodeLowerLeft, nodeDepth, Root);
            //var data = generator.Generate(nodeLowerLeft, ChunkSize + new Point3(1, 1, 1), getNodeResolution(nodeDepth));

            //return new OctreeNode()
            //{
            //    Depth = nodeDepth,
            //    LowerLeft = nodeLowerLeft,
            //    VoxelData = data
            //};
        }

        public int getNodeResolution(int nodeDepth)
        {
            return 1 << (this.depth - nodeDepth);
        }

        public OctreeNode GetChild(OctreeNode worldRoot, int childIndex)
        {
            if (worldRoot.Children == null) throw new InvalidOperationException("Has no children!");
            var ret = worldRoot.Children[childIndex];
            allocChunk(ret);

            return ret;

        }

     
    }
}
