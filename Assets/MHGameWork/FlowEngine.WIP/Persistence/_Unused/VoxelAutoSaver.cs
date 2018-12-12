using System.Collections.Generic;
using Assets.MarchingCubes.VoxelWorldMVP;
using Assets.MarchingCubes.VoxelWorldMVP.Persistence;
using UnityEngine;

namespace Assets.MarchingCubes.Persistence
{
    public class VoxelAutoSaver : MonoBehaviour
    {
        public float AutosaveInterval = 5;
        private OctreeVoxelWorld world;

        public void Start()
        {
        }
        public void Init(OctreeVoxelWorld octreeVoxelWorld, List<VoxelMaterial> voxelMaterials)
        {
            this.world = octreeVoxelWorld;
        }
        private bool first = true;
        private int lastFrame = 1;
        private float lastAutoSave = 0;
        private List<VoxelWorldAsset> temp = new List<VoxelWorldAsset>();
        public void Update()
        {
            return;
//#if UNITY_EDITOR
//            if (Time.time - lastAutoSave < AutosaveInterval) return;
//            lastAutoSave = AutosaveInterval;
//            //if (!first) return;
//            //first = false;

//            var ret = new VoxelWorldAsset();

//            ret.ChunkSize = world.ChunkSize.X + 1; // Because the corner has an extra voxel !!
//            ret.ChunkOversize = OctreeVoxelWorld.ChunkOversize;
//            ret.Versions = new List<SerializedVersion>();
//            //ret.Versions.Add(new SerializedVersion()
//            //{
//            //    Chunks = new List<SerializedChunk>(),
//            //    SavedDate = new DateTime()
//            //});
//            //return ret;

//            var p = new OctreeWorldSerializer(world, ret);
//            var version = p.createVersion(lastFrame);
//            lastFrame = Time.frameCount;

//            ret.Versions.Add(version);

//            temp.Add(ret);
//#endif
        }


    }
}