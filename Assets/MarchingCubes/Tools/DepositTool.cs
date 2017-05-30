using System;
using System.Linq;
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
            var radius = new Point3(1, 1, 1) * (int)Math.Ceiling(script.ActiveSize);

            var mode = -1;
            if (Input.GetMouseButton(0)) mode = 0;
            if (Input.GetMouseButton(1)) mode = 1;
            if (mode != -1)
            {
                //var weights = new[] {0.5f, 1f, 0.5f};
                var weights = new[] {0.1f, 2f, 0.1f};
                weights = weights.Select(f => f / weights.Sum()).ToArray();

                world.RunKernelXbyXUnrolled(point.ToFloored() - radius, point.ToCeiled() + radius, (data, p) =>
                {
                    var val = data[1];
                    //if ((p - point).magnitude <= range)


                    var sum =
                        -(Mathf.Clamp(data[0].Density, -1 + mode, 0 + mode) +
                          Mathf.Clamp(data[2].Density, -1 + mode, 0 + mode));

                    //var d = 0f;
                    //for (int i = 0; i < weights.Length; i++)
                    //{
                    //    d += Mathf.Clamp(data[i].Density, -1, 1) * weights[i];
                    //}
                    {

                        //val.Material = material;
                        val.Density -= sum * 0.1f;
                        if (val.Density > 0 && val.Material == null)
                            val.Material = material;
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