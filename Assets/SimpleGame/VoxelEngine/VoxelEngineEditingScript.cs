using System;
using Assets.MarchingCubes.Rendering;
using Assets.MarchingCubes.VoxelWorldMVP;
using Assets.MarchingCubes.World;
using Assets.MHGameWork.FlowEngine.OctreeWorld;
using Assets.MHGameWork.FlowEngine._Cleanup;
using Assets.SimpleGame.Tools;
using Assets.SimpleGame._UtilsToMove;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.SimpleGame.VoxelEngine
{
    /// <summary>
    /// Responsible for synchronizing voxel engine edits
    /// </summary>
    public class VoxelEngineEditingScript : NetworkBehaviour
    {
        private VoxelWorldEditingHelper helper;

        void Start()
        {
        }

        public void Initialize(VoxelRenderingEngineScript renderer, OctreeVoxelWorld world)
        {
            helper = new VoxelWorldEditingHelper(renderer, world);
        }

        public void AddEdit(Vector3 pos, float digRadius)
        {
            if (!isServer) throw new Exception("Server only!");
            RpcAddEdit(pos, digRadius);

            RpcAddEdit(pos, digRadius);
        }

      

        [ClientRpc]
        private void RpcAddEdit(Vector3 pos, float digRadius)
        {
            DigTool.doDigTemp(digRadius, new SDFWorldEditingService.Counts(), helper, pos);
        }
    }
}