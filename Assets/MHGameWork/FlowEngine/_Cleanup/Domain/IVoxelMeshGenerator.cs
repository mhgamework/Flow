using Assets.MHGameWork.FlowEngine.Models;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;

namespace Assets.MHGameWork.FlowEngine._Cleanup.Domain
{
    public interface IVoxelMeshGenerator
    {
        void GenerateMeshFromVoxelData(Array3D<VoxelData> voxelData, VoxelMeshData outData);
    }
}