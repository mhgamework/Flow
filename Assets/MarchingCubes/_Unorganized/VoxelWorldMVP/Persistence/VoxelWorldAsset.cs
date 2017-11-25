using System;
using System.Collections.Generic;
using DirectX11;
using UnityEngine;

namespace Assets.MarchingCubes.VoxelWorldMVP.Persistence
{
    public class VoxelWorldAsset : ScriptableObject
    {
        public List<SerializedVersion> Versions;
        public int ChunkSize;
        public int ChunkOversize; // Extra data for lod
    }

    [Serializable]
    public class SerializedVersion
    {
        public DateTime SavedDate;
        public List<SerializedChunk> Chunks = new List<SerializedChunk>();

    }
    [Serializable]
    public class SerializedChunk
    {
        public float[] Densities;
        public Color[] Colors;
        public Point3 LowerLeft;
        public int RelativeResolution;
        public string LayoutDescription = "x+size*(y+size*z)";

        public static int toIndex(Point3 p,int size)
        {
            return p.X + size * (p.Y + size * p.Z);
        }
    }
}