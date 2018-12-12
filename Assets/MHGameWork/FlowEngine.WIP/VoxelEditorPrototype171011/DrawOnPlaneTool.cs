using System;
using Assets.MHGameWork.FlowEngine.OctreeWorld;
using DirectX11;
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
        private Vector3 flatPointStart;
        private Vector3 flatNormalStart;

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

            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (raycast.HasValue)
                {
                    sphereGizmo.transform.position = raycast.Value.point;
                    sphereGizmo.transform.localScale = Vector3.one * script.ActiveSize;
                    if (Input.GetMouseButtonDown(1))
                    {
                        flatPoint = raycast.Value.point;
                        flatNormal = raycast.Value.normal;
                        flatPointStart = flatPoint;
                        flatNormalStart = flatNormal;
                    }
                    else if (Input.GetMouseButton(1))
                    {
                        flatPoint = (raycast.Value.point + flatPointStart) * 0.5f;
                        var up = Vector3.Slerp(raycast.Value.normal, flatNormalStart, 0.5f);

                        var diff = flatPoint - flatPointStart;
                        var realUp = Vector3.Cross(Vector3.Cross(diff, up), diff);
                        flatNormal = realUp.normalized;

                        planeGizmo.transform.localScale = new Vector3(1, 1, 1) *
                                                          (raycast.Value.point - flatPointStart).magnitude;
                    }

                }
                return;
            }
         
            if (!Input.GetMouseButton(1))
                planeGizmo.transform.localScale = new Vector3(1, 1, 1);

            var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            float dist;
            if (!new Plane(flatNormal, flatPoint).Raycast(ray, out dist)) return;

            var localPoint = ray.GetPoint(dist);

            planeGizmo.transform.position = localPoint;



            sphereGizmo.transform.position = localPoint;
            sphereGizmo.transform.localScale = Vector3.one * script.ActiveSize;


            var range = script.ActiveSize;
            var material = script.ActiveMaterial;
            var radius = new Point3(1, 1, 1) * (int)Math.Ceiling(script.ActiveSize);

            tool.tryPlaceSphere(range, material, localPoint - flatNormal * range * 0.5f, true);


        }

        public void OnDrawGizmos()
        {
        }
    }
}