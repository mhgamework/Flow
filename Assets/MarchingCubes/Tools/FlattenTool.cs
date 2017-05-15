using System;
using DirectX11;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    public class FlattenTool : IState
    {
        private VoxelWorldEditorScript script;
        private PlaceSphereState tool;
        private IEditableVoxelWorld world;
        private readonly GameObject sphereGizmo;
        private readonly GameObject planeGizmo;
        private Vector3 flatPoint;
        private Vector3 flatNormal;

        public string Name
        {
            get { return "Flatten tool"; }
        }
        public FlattenTool(VoxelWorldEditorScript script, IEditableVoxelWorld world, GameObject sphereGizmo, GameObject planeGizmo)
        {
            this.script = script;
            this.world = world;
            this.sphereGizmo = sphereGizmo;
            this.planeGizmo = planeGizmo;
            //this.tool = new PlaceSphereState(script, world);
        }

        public void Start()
        {
            planeGizmo.SetActive(true);
        }

        public void Stop()
        {
            planeGizmo.SetActive(false);
        }

        public void Update(RaycastHit? raycast)
        {
            planeGizmo.transform.position = flatPoint;
            planeGizmo.transform.up = flatNormal;
            if (!raycast.HasValue) return;
            var localPoint = raycast.Value.point;

            sphereGizmo.transform.position = localPoint;
            sphereGizmo.transform.localScale = Vector3.one * script.ActiveSize;


            if (Input.GetMouseButtonDown(1))
            {
                flatPoint = raycast.Value.point;
                flatNormal = raycast.Value.normal;

            }

            if (Input.GetMouseButtonDown(0))
            {

                var range = script.ActiveSize;
                var material = script.ActiveMaterial;
                var radius = new Point3(1, 1, 1) * (int)Math.Ceiling(script.ActiveSize);

                world.RunKernel1by1(localPoint.ToFloored() - radius, localPoint.ToCeiled() + radius, (data, p) =>
                {
                    if ((p - localPoint).magnitude <= range)// && data.Density >  0)
                    {
                        if (data.Material == null || data.Material.color.Equals(new Color()))
                            data.Material = material;
                        data.Density = new Plane(flatNormal, flatPoint).GetDistanceToPoint(p);
                        return data;
                    }
                    return data;
                }, Time.frameCount);

            }

        }

        public void OnDrawGizmos()
        {
        }
    }
}