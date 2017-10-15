using System;
using System.Linq;
using Assets.MarchingCubes.Domain;
using DirectX11;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    public class DepositTool : IState
    {
        private VoxelWorldEditorScript script;
        private PlaceSphereState tool;
        private IEditableVoxelWorld world;
        private readonly GameObject sphereGizmo;
        private Vector3 point;

        public string Name
        {
            get { return "Deposit"; }
        }

        public DepositTool(VoxelWorldEditorScript script, IEditableVoxelWorld world, GameObject sphereGizmo)
        {
            this.script = script;
            this.world = world;
            this.sphereGizmo = sphereGizmo;
            //this.tool = new PlaceSphereState(script, world);
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public void Update(RaycastHit? raycast)
        {
            if (!raycast.HasValue) return;

            point = raycast.Value.point;

            sphereGizmo.transform.position = point;
            sphereGizmo.transform.localScale = Vector3.one * script.ActiveSize;
            var range = script.ActiveSize;
            var material = script.ActiveMaterial;

            if (Input.GetMouseButton(0)) DepositTerrainMaterial(range, material);
            if (Input.GetMouseButton(1)) WithdrawTerrainMaterial(range, material);
        }

        protected virtual void DepositTerrainMaterial(float radius, VoxelMaterial material)
        {
            WorldEditOperations.DepositOrWithdrawTerrainMaterial(new Point3(1, 1, 1) * (int)Math.Ceiling(radius), 0, material, point, world, Time.frameCount);
        }

        protected virtual void WithdrawTerrainMaterial(float radius, VoxelMaterial material)
        {
            WorldEditOperations.DepositOrWithdrawTerrainMaterial(new Point3(1, 1, 1) * (int)Math.Ceiling(radius), 1, material, point, world, Time.frameCount);
        }




        public void OnDrawGizmos()
        {
        }
    }
}