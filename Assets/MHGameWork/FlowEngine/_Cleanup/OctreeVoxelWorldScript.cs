using System;
using Assets.MHGameWork.FlowEngine.Models;
using Assets.MHGameWork.FlowEngine.OctreeWorld;
using Assets.MHGameWork.FlowEngine._Cleanup.EditorVoxelWorldGen;
using DirectX11;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.MHGameWork.FlowEngine._Cleanup
{
    /// <summary>
    /// Temporary constructor class for providing OctreeVoxelWorldScript with world
    /// </summary>
    public interface IVoxelWorldFactory
    {
        OctreeVoxelWorld CreateNewWorld();
    }
    /// <summary>
    /// Can currently be either a VoxelWorldGenerator or SDFWorld, or a OctreeVoxelWorld set from code
    /// </summary>
    public class OctreeVoxelWorldScript : MonoBehaviour
    {
        public IVoxelWorldFactory GeneratorWorld;
        public SDFWorldGeneratorScript SDFWorld;
        public int SDFChunkSize = 8;
        public int SDFChunkDepth = 2;
        private OctreeVoxelWorld worldSetFromCodeMode;

        private bool hasInitializationStrategySet()
        {
            return GeneratorWorld != null
                   || SDFWorld != null
                   || worldSetFromCodeMode != null;
        }
        public OctreeVoxelWorld CreateNewWorld()
        {
            if (!hasInitializationStrategySet())
                throw new InvalidOperationException("Not world initialization strategy set!");
            if (GeneratorWorld != null && SDFWorld != null) // SHould also check for worldsetfrom code but wathever
                throw new System.Exception("Can only set one of the world types1");

            if (GeneratorWorld != null) return GeneratorWorld.CreateNewWorld();
            if (SDFWorld != null) return createWorldFromSdf(SDFWorld);
            if (worldSetFromCodeMode != null) return worldSetFromCodeMode;

            throw new System.Exception("No world set");
        }

        private OctreeVoxelWorld createWorldFromSdf(SDFWorldGeneratorScript sdfWorld)
        {
            var ret = new OctreeVoxelWorld(new SDFAdapterGenerator(sdfWorld), SDFChunkSize, SDFChunkDepth);
            //Pregen
            //var bounds = new Bounds();
            foreach (var c in sdfWorld.ChildrenList)
            {
                //pregen(c, ret, 2);
                //pregen(c, ret, 1);
                //pregen(c, ret, 0);

                //if (c == sdfWorld.ChildrenList[0])
                //    bounds.SetMinMax(c.Min, c.Max);
                //else
                //{
                //    bounds.Encapsulate(c.Min);
                //    bounds.Encapsulate(c.Max);
                //}
            }
            return ret;
        }

        /// <summary>
        /// Configures this class to use a world created in code, instead of passed through editor
        /// </summary>
        /// <param name="world"></param>
        public void SetWorldDirectlyFromCodeMode(OctreeVoxelWorld world)
        {
            if (hasInitializationStrategySet())
                throw new InvalidOperationException("Already using another kind of world initialization strategy");
            this.worldSetFromCodeMode = world;
        }

       

        private void pregen(IVoxelObject c, OctreeVoxelWorld ret, int depth)
        {
            var res = 1 << depth;
            var lower = (c.Min / SDFChunkSize / res).ToFloored();
            var upper = (c.Max / SDFChunkSize / res).ToCeiled();
            for (int x = lower.X; x < upper.X; x++)
                for (int y = lower.Y; y < upper.Y; y++)
                    for (int z = lower.Z; z < upper.Z; z++)
                    {
                        var f = ret.GetNode(new Point3(x, y, z) * SDFChunkSize * res, SDFChunkDepth - depth);
                        if (f == null) throw new System.Exception();
                    }
        }

        private class SDFAdapterGenerator : IWorldGenerator
        {
            private SDFWorldGeneratorScript sdfWorld;

            public SDFAdapterGenerator(SDFWorldGeneratorScript sdfWorld)
            {
                this.sdfWorld = sdfWorld;
                sdfWorld.UpdateChildrenList();

            }

            public void Generate(Point3 start, Point3 chunkSize, int sampleResolution, UniformVoxelData outData)
            {
                lock (this)
                {
                    sdfWorld.GenerateChunk(start, chunkSize.X, sampleResolution, outData);
                }
            }
        }
    }
}