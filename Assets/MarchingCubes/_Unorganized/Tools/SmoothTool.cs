using System;
using Assets.MarchingCubes.Domain;
using Assets.VR;
using DirectX11;
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
            sphereGizmo.SetActive(true);
        }

        public void Stop()
        {
            sphereGizmo.SetActive(false);
        }

        public void Update(RaycastHit? raycast)
        {
            if (!raycast.HasValue) return;

            point = raycast.Value.point;

            sphereGizmo.transform.position = point;
            sphereGizmo.transform.localScale = Vector3.one * script.ActiveSize * VRSettings.RenderScale;
            var range = script.ActiveSize;
            var material = script.ActiveMaterial;

            if (Input.GetMouseButton(0))
            {
                SmoothTerrain(range, material);
            }

        }

        protected virtual void SmoothTerrain(float radius, VoxelMaterial material)
        {
            //var weights = new[] {0.5f, 1f, 0.5f};
            WorldEditOperations.SmoothTerrain(new Point3(1, 1, 1) * (int)Math.Ceiling(radius), material, Time.frameCount, world, point / VRSettings.RenderScale, null);
        }



        public void OnDrawGizmos()
        {
        }
    }
}
;