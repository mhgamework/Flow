using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.MarchingCubes
{
    public static class SdfFunctions
    {
        public static float Sphere(Vector3 x, Vector3 center, float size)
        {
            return (x - center).magnitude - size;
        }
    }
}
