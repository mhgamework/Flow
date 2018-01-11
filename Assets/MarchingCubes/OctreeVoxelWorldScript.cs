using Assets.MarchingCubes.VoxelWorldMVP;
using Assets.SimpleGame.Scripts.EditorVoxelWorldGen;
using DirectX11;
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
            return new OctreeVoxelWorld(new SDFAdapterGenerator(sdfWorld), SDFChunkSize, SDFChunkDepth);
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