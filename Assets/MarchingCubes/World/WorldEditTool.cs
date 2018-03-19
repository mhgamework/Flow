using System;
using DirectX11;
using UnityEngine;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    /// <summary>
    /// Service class
    /// </summary>
    public class WorldEditTool
    {

        public void addSphere(UniformVoxelData data, Vector3 position, float radius, VoxelMaterial material)
        {
            var func = createAddSphereKernel(position, radius, material);

            data.Data.ForEach((v, p) =>
            {
                data.Data[p] = func(v, p);
            });
        }

        public static Func<VoxelData, Point3, VoxelData> createAddSphereKernel(Vector3 position, float radius, VoxelMaterial material)
        {
            Func<VoxelData, Point3, VoxelData> func = (v, p) =>
            {
                var sphere = SdfFunctions.Sphere(p.ToVector3(), position, radius);
                // Min
                if (v.Density < sphere) return v;
                return new VoxelData(sphere, material);
            };
            return func;
        }

        public void removeSphere(UniformVoxelData data, Vector3 position, float radius, VoxelMaterial material)
        {
            var func = createRemoveSphereKernel(position, radius);

            data.Data.ForEach((v, p) =>
            {
                data.Data[p] = func(v, p);
            });
        }

        public static Func<VoxelData, Point3, VoxelData> createRemoveSphereKernel(Vector3 position, float radius)
        {
            Func<VoxelData, Point3, VoxelData> func = (v, p) =>
            {
                var sphere = SdfFunctions.Sphere(p.ToVector3(), position, radius);
                // d1-d2 = Max(d1,-d2) ??? reverse?
                if (v.Density > -sphere) return v;
                return new VoxelData(-sphere, v.Material);
            };
            return func;
        }


        //public Vector3 RayPosition;
        //public Vector3 RayDirection;
        //public float PlacementSize = 3;
        //public float PlacementSpeed = 1;

        //private void tryClick()
        //{
        //    var dir = 0;
        //    if (Input.GetMouseButton(0))
        //        dir = 1;
        //    else if (Input.GetMouseButton(1))
        //        dir = -1;
        //    else
        //        return;

        //    //var ray = Camera.main.ScreenPointToRay(new Vector3(0.5f, 0.5f, 0));// new Ray(RayPosition, RayDirection.normalized);
        //    var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        //    RaycastHit hitInfo;
        //    if (!Physics.Raycast(ray, out hitInfo)) return;

        //    var point = hitInfo.point;
        //    Array3D<float> data = null;

        //    data.ForEach((val, p) =>
        //    {
        //        var dist = (p - point).magnitude;
        //        if (dist > PlacementSize) return;

        //        val += PlacementSpeed * Time.deltaTime * dir;
        //        data[p] = Math.Max(Math.Min(val, 1), -1);
        //    });



        //}
    }
}
