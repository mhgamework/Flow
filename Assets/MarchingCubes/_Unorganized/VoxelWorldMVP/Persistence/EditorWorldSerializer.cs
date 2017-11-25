#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using Assets.UnityAdditions;

namespace Assets.MarchingCubes.VoxelWorldMVP.Persistence
{
    /// <summary>
    /// For use in unity editor
    /// </summary>
    public class EditorWorldSerializer : IWorldSerializer
    {
        private readonly RuntimeWorldSerializer runtimeSerializer;

        public EditorWorldSerializer(RuntimeWorldSerializer runtimeSerializer)
        {
            this.runtimeSerializer = runtimeSerializer;
        }

        public VoxelWorldAsset CreateAsset(int minNodeSize)
        {
            if (!AssetDatabase.IsValidFolder("Assets/_Generated")) AssetDatabase.CreateFolder("Assets", "_Generated");
            var ret = ScriptableObjectUtility.CreateAsset<VoxelWorldAsset>("Assets/_Generated/New Octree Voxel World");

            runtimeSerializer.CreateAsset( minNodeSize, ret);
            return ret;
        }

        public void Save(int changesSinceFrame, VoxelWorldAsset asset, OctreeVoxelWorld world)
        {
            var version = runtimeSerializer.createVersion(changesSinceFrame, asset, world);
            asset.Versions.Add(version);
            EditorUtility.SetDirty(asset);

            AssetDatabase.SaveAssets();
        }

        public VoxelWorldAsset LoadAsset(VoxelWorldAsset asset)
        {
            return asset;// Just use editor version
        }

    }
}
#endif