using Assets.MarchingCubes.SdfModeling;
using Assets.MarchingCubes.VoxelWorldMVP;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.SimpleGame.Voxel
{
    public class VoxelEditingFunctions
    {

        /// <summary>
        /// Returns true when something has been removed
        /// </summary>
        public static bool Subtract(IEditableVoxelWorld world, DistObject transform, Bounds b)
        {
           
            var hasDug = false;
            world.RunKernel1by1(b.min.ToFloored(),b.max.ToCeiled(),
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