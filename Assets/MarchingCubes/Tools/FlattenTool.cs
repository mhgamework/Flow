using System;
using Assets.MarchingCubes.Domain;
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
            get { return "Flatten tool (Right click to select plane)"; }
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

            if (Input.GetMouseButton(0))
            {
                FlattenTerrain(localPoint, new Plane(flatNormal, flatPoint), script.ActiveSize, script.ActiveMaterial);
            }

        }

        protected virtual  void FlattenTerrain(Vector3 localPoint, Plane targetPlane, float range, VoxelMaterial material)
        {
            WorldEditOperations.FlattenTerrain(localPoint, range, material, world, Time.frameCount,targetPlane);
        }


        public void OnDrawGizmos()
        {
        }
    }
}