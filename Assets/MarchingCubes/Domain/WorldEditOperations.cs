using System;
using System.Linq;
using Assets.MarchingCubes.VoxelWorldMVP;
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

        public static void SmoothTerrain(Point3 radius, VoxelMaterial material, int currentFrameCount, IEditableVoxelWorld world, Vector3 centerPoint)
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
                    if (d > 0 && val.Material == null)
                        val.Material = material;
                    //val.Material = material;
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