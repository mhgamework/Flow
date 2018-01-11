using Assets.MarchingCubes.VoxelWorldMVP;
using Assets.SimpleGame.Scripts;
using Assets.SimpleGame.Scripts.EditorVoxelWorldGen;
using DirectX11;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.MarchingCubes
{
    /// <summary>
    /// Can currently be either a VoxelWorldGenerator or SDFWorld
    /// </summary>
    public class OctreeVoxelWorldScript : MonoBehaviour
    {
        public VoxelWorldGenerator GeneratorWorld;
        public SDFWorldGeneratorScript SDFWorld;
        public int SDFChunkSize = 8;
        public int SDFChunkDepth = 2;

        public OctreeVoxelWorld CreateNewWorld()
        {
            if (GeneratorWorld != null && SDFWorld != null)
                throw new System.Exception("Can only set one of the world types1");

            if (GeneratorWorld != null) return GeneratorWorld.CreateNewWorld();
            if (SDFWorld != null) return createWorldFromSdf(SDFWorld);

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