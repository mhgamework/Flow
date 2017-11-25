using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using UnityEngine;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    public class SdfSampler
    {
        public Array3D<VoxelData> CreateEmptyData(int size)
        {
            return new Array3D<VoxelData>(new DirectX11.Point3(size + 1, size + 1, size + 1));
        }
        public void SampleSdf(SdfFunction sdf, Vector3 startCoord, float sampleInterval, Array3D<VoxelData> outData)
        {
            outData.ForEach((v, p) =>
            {
                outData[p] = sdf(p.ToVector3() * sampleInterval + startCoord, sampleInterval);
            });
        }
    }

    public delegate VoxelData SdfFunction(Vector3 point, float sampleInterval);
}