﻿using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            data.Data.ForEach((v, p) => {
                var sphere = SdfFunctions.Sphere(p.ToVector3(), position, radius);
                // Min
                if (v.Density < sphere) return;
                v.Density = sphere;
                v.Material = material;
            });
        }
        public void removeSphere(UniformVoxelData data, Vector3 position, float radius, VoxelMaterial material)
        {
            data.Data.ForEach((v, p) => {
                var sphere = SdfFunctions.Sphere(p.ToVector3(), position, radius);
                // d1-d2 = Max(d1,-d2) ??? reverse?
                if (v.Density > -sphere) return;
                v.Density = -sphere;
                //v.Material = material;
            });
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
