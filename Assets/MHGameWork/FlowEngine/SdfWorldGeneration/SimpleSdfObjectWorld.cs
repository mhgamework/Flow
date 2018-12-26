using System.Collections.Generic;
using Assets.MHGameWork.FlowEngine._Cleanup.EditorVoxelWorldGen;
using DirectX11;
using UnityEngine;

namespace Assets.MHGameWork.FlowEngine.SdfWorldGeneration
{
    public class SimpleSdfObjectWorld : ISdfObjectWorld
    {
        public List<IVoxelObject> Objects { get; set; }

        public SimpleSdfObjectWorld()
        {
            Objects = new List<IVoxelObject>();
        }

        public void GetObjectsInRange(Point3 min, Point3 max, List<IVoxelObject> outList)
        {
            var bounds = new Bounds();
            bounds.SetMinMax(min, max);

            for (var index = 0; index < Objects.Count; index++)
            {
                var v = Objects[index];
                var bounds1 = new Bounds((v.Max + v.Min) * 0.5f, v.Max - v.Min);
                if (!bounds1.Intersects(bounds)) continue;
                outList.Add(v);
            }
        }
    }
}