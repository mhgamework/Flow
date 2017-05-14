using System;
using DirectX11;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    class PlaceSphereState : IState
    {
        private readonly IEditableVoxelWorld world;
        private readonly GameObject sphereGizmo;

        private WorldEditTool tool = new WorldEditTool();

        private VoxelWorldEditorScript voxelWorldScript;
        private Vector3 point;

        public string Name
        {
            get { return "Add/Remove sphere"; }
        }

        public PlaceSphereState(VoxelWorldEditorScript voxelWorldScript, IEditableVoxelWorld world, GameObject sphereGizmo)
        {
            this.voxelWorldScript = voxelWorldScript;
            this.world = world;
            this.sphereGizmo = sphereGizmo;
        }

        public void Stop()
        {
        }

        public void Update(RaycastHit? raycast)
        {
            if (!raycast.HasValue) return;

            var range = voxelWorldScript.ActiveSize;
            var material = voxelWorldScript.ActiveMaterial;

            point = raycast.Value.point;

            sphereGizmo.transform.position = point;
            sphereGizmo.transform.localScale = Vector3.one * range;

            tryPlaceSphere(range, material, point);

        }

        public void tryPlaceSphere(float range, VoxelMaterial material, Vector3 point)
        {
            var min = (point - new Vector3(1, 1, 1) * range).ToFloored();
            var max = (point + new Vector3(1, 1, 1) * range).ToCeiled();
            var radius = new Point3(1, 1, 1) * (int)Math.Ceiling(range);

            if (Input.GetMouseButtonDown(0))
            {
                world.RunKernel1by1(point.ToFloored() - radius, point.ToCeiled() + radius, WorldEditTool.createAddSphereKernel(point, range, material), Time.frameCount);

                //world.ForChunksInRange(min, max, (p, c) =>
                //{
                //    var offset = p.Multiply(world.ChunkSize);
                //    var localHit = point - offset;

                //    tool.addSphere(c, localHit, range, material);
                //    c.LastChangeFrame = Time.frameCount;

                //});


            }
            else if (Input.GetMouseButtonDown(1))
            {
                world.RunKernel1by1(point.ToFloored() - radius, point.ToCeiled() + radius, WorldEditTool.createRemoveSphereKernel(point, range), Time.frameCount);

                //world.ForChunksInRange(min, max, (p, c) =>
                //{
                //    var offset = p.Multiply(world.ChunkSize);
                //    var localHit = point - offset;

                //    tool.removeSphere(c, localHit, range, material);
                //    c.LastChangeFrame = Time.frameCount;

                //});
            }
        }

        public void Start()
        {
        }

        public void OnDrawGizmos()
        {
        }
    }
}