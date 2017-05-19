#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Assets.MarchingCubes.VoxelWorldMVP.Persistence;
using Assets.UnityAdditions;
using DirectX11;
using MHGameWork.TheWizards.DualContouring.Terrain;
using UnityEditor;
using UnityEngine;

namespace Assets.MarchingCubes.VoxelWorldMVP.Octrees
{
    public class OctreeWorldSerializer
    {
        private readonly OctreeVoxelWorld world;
        private readonly VoxelWorldAsset asset;

        public OctreeWorldSerializer(OctreeVoxelWorld world, VoxelWorldAsset asset)
        {
            this.world = world;
            this.asset = asset;
        }
        public void Save(int changesSinceFrame)
        {
            var version = new SerializedVersion();
            version.SavedDate = new DateTime();
            version.Chunks = new List<SerializedChunk>();

            var first = true;
            new ClipMapsOctree<OctreeNode>().VisitTopDown(world.Root, n =>
            {
                //if (!first) return;
                if (n.VoxelData == null) return; // Non initialized chunk, not loaded yet
                if (n.VoxelData.LastChangeFrame < changesSinceFrame) return;

                Debug.Log("Serializing " + n.LowerLeft + " " + n.Size + " Frame: " + n.VoxelData.LastChangeFrame);
                //first = false;
                var size = asset.ChunkSize + asset.ChunkOversize;

                var chunk = new SerializedChunk();
                chunk.RelativeResolution = world.getNodeResolution(n.Depth);
                chunk.Densities = new float[size * size * size];
                chunk.Colors = new Color[size * size * size];
                chunk.LowerLeft = n.LowerLeft;


                if (n.VoxelData.Data.Size.X != size)
                {
                    throw new InvalidOperationException("Serialized asset is not compatible with this voxel world, chunk size mismatch");
                }
                n.VoxelData.Data.ForEach((v, p) =>
                {
                    var index = SerializedChunk.toIndex(p, size);
                    chunk.Densities[index] = v.Density;
                    chunk.Colors[index] = v.Material == null ? new Color() : v.Material.color;
                });

                version.Chunks.Add(chunk);


            });

            asset.Versions.Add(version);
            EditorUtility.SetDirty(asset);

            AssetDatabase.SaveAssets();

        }

        public static VoxelWorldAsset CreateAsset(int minNodeSize)
        {
            if (!AssetDatabase.IsValidFolder("Assets/_Generated")) AssetDatabase.CreateFolder("Assets", "_Generated");
            var ret = ScriptableObjectUtility.CreateAsset<VoxelWorldAsset>("Assets/_Generated/New Octree Voxel World");

            ret.ChunkSize = minNodeSize + 1; // Because the corner has an extra voxel !!
            ret.ChunkOversize = OctreeVoxelWorld.ChunkOversize;
            ret.Versions = new List<SerializedVersion>();
            ret.Versions.Add(new SerializedVersion()
            {
                Chunks = new List<SerializedChunk>(),
                SavedDate = new DateTime()
            });
            return ret;
        }
    }
}
#endif