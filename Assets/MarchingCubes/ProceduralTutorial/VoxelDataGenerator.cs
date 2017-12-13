using Assets.MarchingCubes.VoxelWorldMVP;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using UnityEngine;

namespace Assets.MarchingCubes.ProceduralTutorial
{
    public class VoxelDataGenerator
    {
       

        public static Array3D<VoxelData> VoxelDataFromHeightMap(float[,] map, int sampleDistance, float chunkHeight, VoxelMaterial voxelMaterial, float heightMultiplier, AnimationCurve oriCurve)
        {
            var curve = new AnimationCurve(oriCurve.keys);

            var mapSize = map.GetLength(0);
            var chunkSize = mapSize;

            var data = new Array3D<VoxelData>(new DirectX11.Point3(chunkSize, chunkSize, chunkSize));
            for (int x = 0; x < chunkSize; x++)
                for (int y = 0; y < chunkSize; y++)
                    for (int z = 0; z < chunkSize; z++)
                    {
                        var height = curve.Evaluate(map[x, z]) * heightMultiplier;

                        var worldY = chunkHeight + y * sampleDistance;
                        //height = 100;
                        //data[new DirectX11.Point3(x, y, z)] = new VoxelData((height - (y*resolution+yOffset)), voxelMaterial);
                        data[new DirectX11.Point3(x, y, z)] = new VoxelData((height - worldY), voxelMaterial);
                    }
            return data;
        }
    }
}