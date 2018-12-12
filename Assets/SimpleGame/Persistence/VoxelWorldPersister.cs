using System.IO;
using Assets.MarchingCubes.VoxelWorldMVP;
using Assets.MarchingCubes.VoxelWorldMVP.Persistence;
using Assets.MHGameWork.FlowEngine.OctreeWorld;

namespace Assets.MarchingCubes.Scenes.Persistence
{
    /// <summary>
    /// Save and load voxel worlds, in a folder format, for the simplegame project
    /// SimpleGame/_SavedWorlds holds some worlds
    /// </summary>
    public class VoxelWorldPersister
    {
        public VoxelWorldPersister()
        {

        }

        public void SaveToFolder(string folder, OctreeVoxelWorld world)
        {
            Directory.CreateDirectory(folder);

            var serializer = new RuntimeWorldSerializer(folder + "/voxel");

            var asset = serializer.CreateAsset(world.ChunkSize.X);
            serializer.Save(-1, asset, world);
        }

        public OctreeVoxelWorld LoadFromFolder(string folder)
        {
            var serializer = new RuntimeWorldSerializer(folder + "/voxel");

            int depth;
            int chunkSize;
            serializer.ReconstructDepthAndChunkSizeFromSave( out chunkSize, out depth);

            var asset = serializer.CreateAsset(chunkSize + OctreeVoxelWorld.ChunkOversize);

            serializer.LoadFromDisk(asset);

            var world = new OctreeVoxelWorld(new PersistenceWorldGenerator(new ConstantVoxelWorldGenerator(0f, null), asset), chunkSize, depth);
            return world;
        }


        public bool HasWorldData(string folder)
        {
            var serializer = new RuntimeWorldSerializer(folder + "/voxel");

            return serializer.HasExistingData();
        }
    }
}