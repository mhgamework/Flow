using Assets.MarchingCubes.SdfModeling;
using Assets.MarchingCubes.VoxelWorldMVP;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.MarchingCubes.World
{
    public class SDFWorldEditingService
    {


        public void AddSDFObject(IEditableVoxelWorld world, DistObject obj, Bounds b,VoxelMaterial material, int frameCount)
        {
            world.RunKernel1by1(b.min.ToFloored(), b.max.ToCeiled(),
                (d, p) =>
                {
                    //var newDensity =  Mathf.Min(d.Density, obj.Sdf(p));
                    var newDensity = obj.Sdf(p);

                    //hasDug = hasDug || (newDensity - d.Density > 0.01);

                    if (newDensity < d.Density)
                    {
                        d.Density = newDensity;
                        d.Material = material;

                    }


                    return d;
                },frameCount);
        }

        /// <summary>
        /// Returns true when something has been removed
        /// </summary>
        public bool Subtract(IEditableVoxelWorld world, DistObject transform, Bounds b)
        {

            var hasDug = false;
            world.RunKernel1by1(b.min.ToFloored(), b.max.ToCeiled(),
                (d, p) =>
                {
                    var newDensity = Mathf.Max(d.Density, -transform.Sdf(p));

                    hasDug = hasDug || (newDensity - d.Density > 0.01);


                    d.Density = newDensity;


                    return d;
                }, Time.frameCount);
            return hasDug;
        }
    }
}