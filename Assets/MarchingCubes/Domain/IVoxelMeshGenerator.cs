using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    public interface IVoxelMeshGenerator
    {
        VoxelMeshData GenerateMeshFromVoxelData(Array3D<VoxelData> voxelData);
    }
}