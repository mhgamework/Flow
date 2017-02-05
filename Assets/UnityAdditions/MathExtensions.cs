using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.UnityAdditions
{
   public static class MathExtensions
    {
        /// <summary>
        /// Component wise multiplication
        /// </summary>
        /// <returns></returns>
        public static Vector3 Multiply(this Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }
    }
}
