using System.Collections.Generic;
using DirectX11;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.SimpleGame.WardDrawing
{
    public static class WardDrawingUtils
    {
        public static readonly Point3 Separator = new Point3(-100, -100, -100);

        /// <summary>
        /// For conversion to editor inspector
        /// </summary>
        public static List<List<Point3>> UnflattenShape(List<Vector3> flattened)
        {
            var ret = new List<List<Point3>>();
            var current = new List<Point3>();
            foreach (var l in flattened)
            {
                var p = l.ToPoint3Rounded();
                if (p.Equals(Separator))
                {
                    ret.Add(current);
                    current = new List<Point3>();
                    continue;
                }
                current.Add(p);
            }
            return ret;
        }

        /// <summary>
        /// For conversion to editor inspector
        /// </summary>
        public static List<Vector3> FlattenShape(List<List<Point3>> lines)
        {
            var ret = new List<Vector3>();
            foreach (var l in lines)
            {
                foreach (var k in l)
                {
                    ret.Add(k);
                }
                ret.Add(Separator);

            }
            return ret;
        }
    }
}