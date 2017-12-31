using System.Linq;
using Assets.MarchingCubes.VoxelWorldMVP;
using Assets.SimpleGame.Scripts;
using Assets.TheWizards.Mathematics.DataTypes;
using DirectX11;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.MarchingCubes._Unorganized.VoxelWorldMVP.Uniform
{
    [ExecuteInEditMode]
    public class EditorVoxelGeneratorScript : MonoBehaviour
    {
        public UniformVoxelRendererScript UniformRenderer;

        public void Start()
        {

        }

        public void Update()
        {
            UpdateVoxels();
            UniformRenderer.UpdateRenderers();
        }

        public void UpdateVoxels()
        {
            var w = UniformRenderer.World;

            var objects = GetComponentsInChildren<IVoxelObject>();
            if (objects.All(k => !k.IsChanged)) return;


            var bounds = new Bounds();
            bool first;
            first = true;
            foreach (var changed in objects.Where(k => k.IsChanged))
            {
                if (first)
                {
                    bounds.SetMinMax(changed.Min, changed.Max);
                    first = false;
                    continue;
                }
                bounds.Encapsulate(changed.Min);
                bounds.Encapsulate(changed.Max);
            }

            //Debug.Log(bounds);
            first = true;
            foreach (var o in objects)
            {
                var tb = new Bounds();
                tb.SetMinMax(o.Min, o.Max);
                if (!bounds.Intersects(tb)) continue;
                //Debug.Log("Rerendering " + o);
                w.RunKernel1by1(bounds.min.ToFloored(), bounds.max.ToCeiled(), (v, p) =>
                {
                    Color color;
                    float density;
                    o.Sdf(p, v, out density, out color);
                    if (first)
                    {
                        v.Density = density;
                        v.Material = new VoxelMaterial() { color = color };
                        return v;
                    }

                    if (density >= v.Density) return v;
                    v.Density = density;
                    v.Material = new VoxelMaterial() { color = color };
                    return v;

                }, Time.frameCount);


                o.RemoveChanged();
                first = false;
            }
        }
    }
}