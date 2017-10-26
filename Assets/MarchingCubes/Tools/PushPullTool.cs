using System;
using Assets.VR;
using DirectX11;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    class PushPullTool : IState
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

        public PushPullTool(VoxelWorldEditorScript voxelWorldScript, IEditableVoxelWorld world, GameObject sphereGizmo)
        {
            this.voxelWorldScript = voxelWorldScript;
            this.world = world;
            this.sphereGizmo = sphereGizmo;
        }

        public void Start()
        {
            sphereGizmo.SetActive(false);
        }

        public void Stop()
        {
            sphereGizmo.SetActive(false);
        }

        public void Update(RaycastHit? raycast)
        {
            if (!raycast.HasValue) return;

            var range = voxelWorldScript.ActiveSize;
            var material = voxelWorldScript.ActiveMaterial;

            point = raycast.Value.point;

            var theNormal = raycast.Value.normal;
            //theNormal = (Camera.main.transform.position - point).normalized;

            var dir = Input.GetMouseButton(1) ? 1 : -1;
            point = point + dir * theNormal * range * VRSettings.RenderScale * 0.9f;

            sphereGizmo.transform.position = point;
            sphereGizmo.transform.localScale = Vector3.one * range * VRSettings.RenderScale;

            tryPlaceSphere(range, material, point / VRSettings.RenderScale, true);

        }

        public void tryPlaceSphere(float range, VoxelMaterial material, Vector3 point, bool continuous = false)
        {
            var min = (point - new Vector3(1, 1, 1) * range).ToFloored();
            var max = (point + new Vector3(1, 1, 1) * range).ToCeiled();
            var radius = new Point3(1, 1, 1) * (int)Math.Ceiling(range);

            if (Input.GetMouseButtonDown(0) || (continuous && Input.GetMouseButton(0)))
            {
                world.RunKernel1by1(point.ToFloored() - radius, point.ToCeiled() + radius, createAddSphereKernelLerp(point, range, material), Time.frameCount);

                //world.ForChunksInRange(min, max, (p, c) =>
                //{
                //    var offset = p.Multiply(world.ChunkSize);
                //    var localHit = point - offset;

                //    tool.addSphere(c, localHit, range, material);
                //    c.LastChangeFrame = Time.frameCount;

                //});


            }
            else if (Input.GetMouseButtonDown(1) || (continuous && Input.GetMouseButton(1)))
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

        public static Func<VoxelData, Point3, VoxelData> createAddSphereKernelLerp(Vector3 position, float radius, VoxelMaterial material)
        {
            Func<VoxelData, Point3, VoxelData> func = (v, p) =>
            {
                var sphere = SdfFunctions.Sphere(p.ToVector3(), position, radius);
                // Min
                if (v.Density < sphere) return v;
                return new VoxelData( Mathf.Lerp( sphere,v.Density,1-Time.deltaTime*10), material);
            };
            return func;
        }
        public static Func<VoxelData, Point3, VoxelData> createRemoveSphereKernelLerp(Vector3 position, float radius)
        {
            Func<VoxelData, Point3, VoxelData> func = (v, p) =>
            {
                var sphere = SdfFunctions.Sphere(p.ToVector3(), position, radius);
                // d1-d2 = Max(d1,-d2) ??? reverse?
                if (v.Density > -sphere) return v;
                return new VoxelData(Mathf.Lerp(-sphere, v.Density, 1 - Time.deltaTime * 2), v.Material);
            };
            return func;
        }

        public void OnDrawGizmos()
        {
        }
    }
}