using System.Linq;
using DirectX11;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using UnityEngine;

namespace Assets.VoxelEngine
{
    public class VoxelAsset : ScriptableObject
    {
        public int SizeX;
        public int SizeY;
        public int SizeZ;
        public bool[] GridPoints;

        public VoxelData ToVoxelData()
        {
            var mat = new VoxelMaterial();
            var d = new VoxelData(new Point3(SizeX,SizeY,SizeZ));
            d.GridPoints = Array3D<VoxelMaterial>.FromFlattenedArray(GridPoints.Select(p => p ? mat : null).ToArray(), new Point3(SizeX, SizeY, SizeZ));

            return d;
        }

        public void FromVoxelData(VoxelData getVoxelData)
        {
            if (getVoxelData == null)
            {
                SizeX = -1;
                SizeY = -1;
                SizeZ = -1;
                return;
            }
            SizeX = getVoxelData.GridPoints.Size.X;
            SizeY = getVoxelData.GridPoints.Size.Y;
            SizeZ = getVoxelData.GridPoints.Size.Z;

            GridPoints = getVoxelData.GridPoints.ToArray().Select(p => p != null).ToArray();
        }
    }
}