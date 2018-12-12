using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;

namespace Assets.MHGameWork.FlowEngine.Models
{
    /// <summary>
    /// Represents the data of a single voxel
    /// Also contains info on when the data was last changed
    /// </summary>
    public class UniformVoxelData
    {
        public Array3D<VoxelData> Data { get;  set; }
        public int LastChangeFrame { get;  set; }

        public bool isEmpty;
    }
}
