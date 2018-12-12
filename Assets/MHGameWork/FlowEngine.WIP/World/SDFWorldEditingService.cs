using System.Linq;
using Assets.MarchingCubes.VoxelWorldMVP;
using Assets.MHGameWork.FlowEngine.Models;
using Assets.MHGameWork.FlowEngine.OctreeWorld;
using Assets.MHGameWork.FlowEngine.SdfModeling;
using Boo.Lang;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.MarchingCubes.World
{
    public class SDFWorldEditingService
    {


        public void AddSDFObject(IEditableVoxelWorld world, DistObject obj, Bounds b, VoxelMaterial material, int frameCount)
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
                }, frameCount);
        }

        /// <summary>
        /// Returns true when something has been removed
        /// </summary>
        public bool Subtract(IEditableVoxelWorld world, DistObject transform, Bounds b, Counts outCounts = null)
        {

            var hasDug = false;
            world.RunKernel1by1(b.min.ToFloored(), b.max.ToCeiled(),
                (d, p) =>
                {
                    var newDensity = Mathf.Max(d.Density, -transform.Sdf(p));

                    if (d.Material != null && (d.Material.color == Color.red || d.Material.color == new Color(1f, 0.9215686f, 0.01568628f, 1f)) && d.Density < 0)
                        newDensity = newDensity * 0.3f + d.Density * 0.7f;

                    hasDug = hasDug || (newDensity - d.Density > 0.01);
                    



                    if (outCounts != null && d.Material != null)
                        outCounts.Change(d.Material, newDensity , d.Density);

                    d.Density = newDensity;


                    return d;
                }, Time.frameCount);
            return hasDug;
        }


        public class Counts
        {
            public List<Count> Amounts { get; private set; }

            public Counts()
            {
                Amounts = new List<Count>();
            }

            public void Change(VoxelMaterial material, float newDensity, float oldDensity)
            {
                if (newDensity >= 0 && oldDensity < 0)
                    Change(material, 1);
                else if (newDensity < 0 && oldDensity >= 0)
                    Change(material, -1);

                //Change(material, Mathf.Clamp( newDensity,-1,1) - Mathf.Clamp(oldDensity, -1, 1));
            }

            public void Change(VoxelMaterial material, float delta)
            {
                for (int i = 0; i < Amounts.Count; i++)
                {
                    var c = Amounts[i];
                    if (c.Material.color != material.color) continue;

                    Amounts[i] = new Count(material,c.Amount+ delta);
                    return;
                }

                Amounts.Add(new Count { Material = material, Amount = delta });
            }

            public void Clear()
            {
                Amounts.Clear();
            }

            public struct Count
            {
                public VoxelMaterial Material;
                public float Amount;

                public Count(VoxelMaterial material, float amount)
                {
                    Material = material;
                    Amount = amount;
                }
            }

            public override string ToString()
            {
                return string.Format("Amounts: {0}",string.Join(", ", Amounts.Select(f => f.Material.color + ": " + f.Amount).ToArray()));
            }
        }
    }
}