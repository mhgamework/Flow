using System;
using System.Linq;
using Assets.Flow.WardDrawing;
using DirectX11;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    public class DrawWardTool : MonoBehaviour, IState
    {
        private VoxelWorldEditorScript script;
        private PlaceSphereState tool;
        private IEditableVoxelWorld world;
        private GameObject sphereGizmo;
        private Vector3 point;
        private Vector3 normal;
        private WardDrawInput wardDrawInput;


        public string Name
        {
            get { return "Draw Wards"; }
        }

        public void Init(VoxelWorldEditorScript script, IEditableVoxelWorld world, GameObject sphereGizmo)
        {
            this.script = script;
            this.world = world;
            this.sphereGizmo = sphereGizmo;
            //this.tool = new PlaceSphereState(script, world);
        }

        public void Start()
        {
            wardDrawInput = GetComponent<WardDrawInput>();
        }

        void IState.Start()
        {
            wardDrawInput.Show();
        }

        void IState.Stop()
        {
            wardDrawInput.Hide();
        }

        public void Update(RaycastHit? raycast)
        {
            if (Input.GetMouseButton(2))
            {
                if (!raycast.HasValue) return;

                point = raycast.Value.point;
                normal = raycast.Value.normal;
                wardDrawInput.SetPlane(point, normal);
                return;
            }

        }

        void IState.OnDrawGizmos()
        {
        }
    }
}