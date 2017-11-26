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
using UnityEngine.Profiling;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    /// <summary>
    /// Holds data for entire voxel world
    /// Could improve octree by just using hasmap instead of sparse tree structure, OR BOTH
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

        /// <summary>
        /// Kernelsize must be an odd number, larger than 1
        /// </summary>
        /// <param name="minInclusive"></param>
        /// <param name="maxInclusive"></param>
        /// <param name="act"></param>
        /// <param name="kernelSize"></param>
        /// <param name="frame"></param>
        public void RunKernelXbyXUnrolled(Point3 minInclusive, Point3 maxInclusive, Func<VoxelData[], Point3, VoxelData> act, int kernelSize, int frame)
        {


            List<OctreeNode> visited = new List<OctreeNode>();

            var pass = new VoxelData[kernelSize];

            kernelSize = (kernelSize - 1) / 2;

            // Extend for edges
            minInclusive -= new Point3(1, 1, 1) * kernelSize;
            maxInclusive += new Point3(1, 1, 1) * kernelSize;
            var diff = maxInclusive - minInclusive + new Point3(1, 1, 1);
            var data = getVoxelDataArray(minInclusive, maxInclusive, visited);

            // Run kernel
            for (int x = minInclusive.X + kernelSize; x <= maxInclusive.X - kernelSize; x++)
                for (int y = minInclusive.Y; y <= maxInclusive.Y; y++)
                    for (int z = minInclusive.Z; z <= maxInclusive.Z; z++)
                    {
                        for (int i = -kernelSize; i <= kernelSize; i++)
                        {
                            pass[i + kernelSize] = data[flatten(new Point3(x + i, y, z) - minInclusive, diff)];
                        }

                        data[flatten(new Point3(x, y, z) - minInclusive, diff)] = act(pass, new Point3(x, y, z));
                    }
            for (int x = minInclusive.X; x <= maxInclusive.X; x++)
                for (int y = minInclusive.Y + kernelSize; y <= maxInclusive.Y - kernelSize; y++)
                    for (int z = minInclusive.Z; z <= maxInclusive.Z; z++)
                    {
                        for (int i = -kernelSize; i <= kernelSize; i++)
                        {
                            pass[i + kernelSize] = data[flatten(new Point3(x, y + i, z) - minInclusive, diff)];
                        }
                        data[flatten(new Point3(x, y, z) - minInclusive, diff)] = act(pass, new Point3(x, y, z));
                    }
            for (int x = minInclusive.X; x <= maxInclusive.X; x++)
                for (int y = minInclusive.Y; y <= maxInclusive.Y; y++)
                    for (int z = minInclusive.Z + kernelSize; z <= maxInclusive.Z - kernelSize; z++)
                    {
                        for (int i = -kernelSize; i <= kernelSize; i++)
                        {
                            pass[i + kernelSize] = data[flatten(new Point3(x, y, z + i) - minInclusive, diff)];
                        }
                        data[flatten(new Point3(x, y, z) - minInclusive, diff)] = act(pass, new Point3(x, y, z));
                    }


            //visited.Sort((a, b) => a.Depth - b.Depth);

            setVoxelDataArray(minInclusive, maxInclusive, frame, visited, data, kernelSize);
        }
        /// <summary>
        /// Mininclusive and maxinclusive describes the voxeldata array
        /// </summary>
        /// <param name="minInclusive"></param>
        /// <param name="maxInclusive"></param>
        /// <param name="frame"></param>
        /// <param name="visited"></param>
        /// <param name="data"></param>
        /// <param name="padding"></param>
        public void setVoxelDataArray(Point3 minInclusive, Point3 maxInclusive, int frame, List<OctreeNode> visited, VoxelData[] data, int padding = 0)
        {
            var originalDiff = maxInclusive - minInclusive + new Point3(1, 1, 1);

            minInclusive += new Point3(1, 1, 1) * padding;
            maxInclusive -= new Point3(1, 1, 1) * padding;

            var offset = new Point3(1, 1, 1) * padding;

            foreach (var l in visited)
            {
                var resolution = getNodeResolution(l.Depth);
                var max = maxInclusive - l.LowerLeft;
                var start = minInclusive - l.LowerLeft;
                for (int i = 0; i < 3; i++)
                    max[i] = Math.Min(max[i] / resolution, ChunkSize[i] + 1 /* One extra for lod stitching */); //floor
                for (int i = 0; i < 3; i++) start[i] = Math.Max((start[i] + resolution - 1) / resolution, 0); // ceil
                var grid = l.VoxelData.Data;

                var changed = false;
                for (int x = start.X; x <= max.X; x++)
                    for (int y = start.Y; y <= max.Y; y++)
                        for (int z = start.Z; z <= max.Z; z++)
                        {
                            changed = true;
                            var p = new Point3(x, y, z);
                            grid[p] =
                                data[
                                    flatten(offset +
                                        new Point3(l.LowerLeft.X + resolution * x, l.LowerLeft.Y + resolution * y,
                                            l.LowerLeft.Z + resolution * z) - minInclusive, originalDiff)];
                        }
                if (changed)
                    l.VoxelData.LastChangeFrame = frame;
            }
        }

        public VoxelData[] getVoxelDataArray(Point3 minInclusive, Point3 maxInclusive, List<OctreeNode> outVisited)
        {
            var diff = maxInclusive - minInclusive + new Point3(1, 1, 1);
            VoxelData[] data = new VoxelData[diff.X * diff.Y * diff.Z];


            helper.VisitTopDown(Root, n =>
            {
                for (int i = 0; i < 3; i++)
                    if (n.LowerLeft[i] > maxInclusive[i]) // is outside
                        return VisitOptions.SkipChildren;
                for (int i = 0; i < 3; i++)
                    if (n.LowerLeft[i] + n.Size + 1 * getNodeResolution(n.Depth) /* one extra for lod stitching */<
                        minInclusive[i]) // is outside
                        return VisitOptions.SkipChildren;
                // inside
                allocChunk(n);
                outVisited.Add(n);
                if (getNodeResolution(n.Depth) == 1)
                {
                    var max = maxInclusive - n.LowerLeft;
                    var start = minInclusive - n.LowerLeft;
                    for (int i = 0; i < 3; i++)
                        max[i] = Math.Min(max[i], ChunkSize[i] + 1 /* One extra for lod stitching */); //floor
                    for (int i = 0; i < 3; i++) start[i] = Math.Max(start[i], 0); // ceil
                    var grid = n.VoxelData.Data;
                    for (int x = start.X; x <= max.X; x++)
                        for (int y = start.Y; y <= max.Y; y++)
                            for (int z = start.Z; z <= max.Z; z++)
                            {
                                var p = new Point3(x, y, z);
                                var val = grid.GetFast(x, y, z);
                                data[flatten(p + n.LowerLeft - minInclusive, diff)] = val;
                                //grid[p] = act(val, new Point3(n.LowerLeft.X + x, n.LowerLeft.Y + y, n.LowerLeft.Z + z));
                            }
                }

                return VisitOptions.Continue;
            });
            return data;
        }

        private int flatten(Point3 v, Point3 size)
        {
            return v.X + size.X * (v.Y + size.Y * v.Z);
        }


        private void allocChunk(OctreeNode octreeNode)
        {
            if (octreeNode.VoxelData != null) return;
            Profiler.BeginSample("AllocChunk");
            octreeNode.VoxelData = generator.Generate(octreeNode.LowerLeft, ChunkSize + new Point3(1, 1, 1) + new Point3(1, 1, 1), 1 << (this.depth - octreeNode.Depth));
            Profiler.EndSample();

            if (octreeNode.Depth == depth) return; // leaf node
            Profiler.BeginSample("AllocChunk-split");
            helper.Split(octreeNode);
            Profiler.EndSample();

        }

        public void ResetChunk(OctreeNode octreeNode,int frame)
        {
            octreeNode.VoxelData = generator.Generate(octreeNode.LowerLeft, ChunkSize + new Point3(1, 1, 1) + new Point3(1, 1, 1), 1 << (this.depth - octreeNode.Depth));
            octreeNode.VoxelData.LastChangeFrame = frame;
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
