using System;
using System.Linq;
using Assets.MarchingCubes.VoxelWorldMVP;
using Assets.MarchingCubes.World;
using Assets.MHGameWork.FlowEngine.Models;
using Assets.MHGameWork.FlowEngine.OctreeWorld;
using DirectX11;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.MarchingCubes.Domain
{
    public class WorldEditOperations
    {
        /// <summary>
        /// Mode 0 is deposit, mode 1 is withdraw
        /// </summary>
        public static void DepositOrWithdrawTerrainMaterialImproved(Point3 radius, int mode, VoxelMaterial material, Vector3 point, IEditableVoxelWorld world, int currentFrameCount)
        {
            //var weights = new[] {0.5f, 1f, 0.5f};
            var weights = new[] { 1, 0, 1 };
            weights = weights.Select(f => f / weights.Sum()).ToArray();

            var plane = new Plane(Vector3.up, point + Vector3.up * Time.deltaTime);

            world.RunKernelXbyXUnrolled(point.ToFloored() - radius, point.ToCeiled() + radius, (data, p) =>
            {


                var val = data[1];
                //if ((p - point).magnitude <= range)

                var d = Mathf.Min(data[0].Density + 1, data[1].Density, data[2].Density + 1);
                var distToPlane = plane.GetDistanceToPoint(p);

                var change = distToPlane - d;
                var changeAbs = Mathf.Abs(change);
                var changeDir = Mathf.Sign(change);

                //d = (data[0].Density + data[2].Density) * 0.5f;
                d = d + changeDir * Mathf.Min(Time.deltaTime, changeAbs);// -= Time.deltaTime;

                //var d = 0f;
                //for (int i = 0; i < weights.Length; i++)
                //{
                //    d += Mathf.Clamp(data[i].Density, -1, 1) * weights[i];
                //}
                {
                    //val.Material = material;
                    val.Density = d;
                    if (val.Density > 0 && val.Material == null)
                        val.Material = material;
                    return val;
                }
                return val;
            }, 3, currentFrameCount);
        }

        /// <summary>
        /// Mode 0 is deposit, mode 1 is withdraw
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="mode"></param>
        /// <param name="material"></param>
        /// <param name="point"></param>
        /// <param name="world"></param>
        /// <param name="currentFrameCount"></param>
        public static void DepositOrWithdrawTerrainMaterial(Point3 radius, int mode, VoxelMaterial material, Vector3 point, IEditableVoxelWorld world, int currentFrameCount)
        {
            //var weights = new[] {0.5f, 1f, 0.5f};
            var weights = new[] { 0.1f, 2f, 0.1f };
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
            }, 3, currentFrameCount);
        }

        public static void SmoothTerrain(Point3 radius, VoxelMaterial material, int currentFrameCount,
            IEditableVoxelWorld world, Vector3 centerPoint, SDFWorldEditingService.Counts outCounts)
        {
            var weights = new[] { 0.1f, 2f, 0.1f };
            weights = weights.Select(f => f / weights.Sum()).ToArray();

            world.RunKernelXbyXUnrolled(centerPoint.ToFloored() - radius, centerPoint.ToCeiled() + radius, (data, p) =>
            {
                var val = data[1];
                //if ((p - point).magnitude <= range)
                var d = 0f;
                for (int i = 0; i < weights.Length; i++)
                {
                    d += Mathf.Clamp(data[i].Density, -1, 1) * weights[i];
                }
                {
                    if (val.Material != null && (val.Material.color == Color.red || val.Material.color == new Color(1f, 0.9215686f, 0.01568628f, 1f)) && val.Density < 0) return val;

                    if (d > 0 && val.Material == null)
                        val.Material = material;
                    //val.Material = material;
                    if (outCounts != null && val.Material != null)
                        outCounts.Change(val.Material, d , val.Density);

                    val.Density = d;
                    return val;
                }
                return val;
            }, 3, currentFrameCount);
        }

        public static void FlattenTerrain(Vector3 localPoint, float range, VoxelMaterial material, IEditableVoxelWorld world, int currentFrameCount, Plane targetPlane)
        {
            var radius = new Point3(1, 1, 1) * (int)Math.Ceiling(range);

            world.RunKernel1by1(localPoint.ToFloored() - radius, localPoint.ToCeiled() + radius, (data, p) =>
            {
                if ((p - localPoint).magnitude <= range) // && data.Density >  0)
                {
                    if (data.Material == null || data.Material.color.Equals(new Color()))
                        data.Material = material;
                    data.Density = targetPlane.GetDistanceToPoint(p); // Idea: interpolate instead of jump
                    return data;
                }
                return data;
            }, currentFrameCount);
        }
    }
}