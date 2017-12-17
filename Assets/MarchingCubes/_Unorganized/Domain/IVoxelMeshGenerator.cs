using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    public interface IVoxelMeshGenerator
    {
        void GenerateMeshFromVoxelData(Array3D<VoxelData> voxelData, VoxelMeshData outData);
    }
}