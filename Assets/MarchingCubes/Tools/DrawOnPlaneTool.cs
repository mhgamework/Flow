using System;
using DirectX11;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    public class DrawOnPlaneTool : IState
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
            get { return "Plane Draw"; }
        }
        public DrawOnPlaneTool(VoxelWorldEditorScript script, IEditableVoxelWorld world, GameObject sphereGizmo, GameObject planeGizmo)
        {
            this.script = script;
            this.world = world;
            this.sphereGizmo = sphereGizmo;
            this.planeGizmo = planeGizmo;
            this.tool = new PlaceSphereState(script, world, sphereGizmo);
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
            if (raycast.HasValue)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    flatPoint = raycast.Value.point;
                    flatNormal = raycast.Value.normal;

                }

            }

            var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            float dist;
            if (!new Plane(flatNormal, flatPoint).Raycast(ray, out dist)) return;

            var localPoint = ray.GetPoint(dist);

            planeGizmo.transform.position = localPoint;



            sphereGizmo.transform.position = localPoint;
            sphereGizmo.transform.localScale = Vector3.one * script.ActiveSize;


            if (Input.GetMouseButtonDown(0))
            {
                var range = script.ActiveSize;
                var material = script.ActiveMaterial;
                var radius = new Point3(1, 1, 1) * (int)Math.Ceiling(script.ActiveSize);

                tool.tryPlaceSphere(range, material, localPoint - flatNormal * range * 0.5f);

            }

        }

        public void OnDrawGizmos()
        {
        }
    }
}