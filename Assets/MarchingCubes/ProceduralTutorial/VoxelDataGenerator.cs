using System.Collections.Generic;
using Assets.MarchingCubes.VoxelWorldMVP;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using UnityEngine;

namespace Assets.MarchingCubes.ProceduralTutorial
{
    public class VoxelDataGenerator
    {


        public static void VoxelDataFromMapData(MapData mapData, int sampleDistance, float chunkHeight, Dictionary<Color, VoxelMaterial> voxelMaterialDict, float heightMultiplier, AnimationCurve oriCurve, Array3D<VoxelData> outData)
        {
            var curve = new AnimationCurve(oriCurve.keys);

            var mapSize = mapData.HeightMap.GetLength(0);
            var chunkSize = mapSize;

            //var data = new Array3D<VoxelData>(new DirectX11.Point3(chunkSize, chunkSize, chunkSize));
            for (int x = 0; x < chunkSize; x++)
                for (int y = 0; y < chunkSize; y++)
                    for (int z = 0; z < chunkSize; z++)
                    {
                        var height = curve.Evaluate(mapData.HeightMap[x, z]) * heightMultiplier;

                        var worldY = chunkHeight + y * sampleDistance;
                        //height = 100;
                        //data[new DirectX11.Point3(x, y, z)] = new VoxelData((height - (y*resolution+yOffset)), voxelMaterial);
                        outData[new DirectX11.Point3(x, y, z)] = new VoxelData((height - worldY), voxelMaterialDict[mapData.ColorMap[x + z * mapSize]]);
                    }
        }
    }
}