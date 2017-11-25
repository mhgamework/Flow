using System.Collections.Generic;
using UnityEngine;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    public class VoxelMeshData
    {
        public List<Vector3> doubledVertices;
        public int numMeshes;
        public List<int[]> indicesList;
        public List<Color> colors;
    }
}