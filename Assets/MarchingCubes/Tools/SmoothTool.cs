using System;
using System.Linq;
using DirectX11;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    public class SmoothTool : IState
    {
        private VoxelWorldEditorScript script;
        private PlaceSphereState tool;
        private IEditableVoxelWorld world;
        private readonly GameObject sphereGizmo;
        private Vector3 point;

        public string Name
        {
            get { return "Smooth"; }
        }

        public SmoothTool(VoxelWorldEditorScript script, IEditableVoxelWorld world, GameObject sphereGizmo)
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
            var radius = new Point3(1, 1, 1) * (int) Math.Ceiling(script.ActiveSize);

            if (Input.GetMouseButtonDown(0))
            {
                world.RunKernelXbyXUnrolled(point.ToFloored() - radius, point.ToCeiled() + radius, (data, p) =>
                {
                    var val = data[1];
                    if ((p - point).magnitude <= range)
                    {
                        val.Material = material;
                        val.Density = data.Sum(f => f.Density) * (1 / 3f);
                        return val;
                    }
                    return val;
                }, 3, Time.frameCount);



            }

        }

        public void OnDrawGizmos()
        {
        }
    }
}
