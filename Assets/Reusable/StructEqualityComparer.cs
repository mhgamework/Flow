using System.Collections.Generic;
using UnityEngine;

namespace Assets.Reusable
{
    public class ColorEqualityComparer : EqualityComparer<Color>
    {
        public override int GetHashCode(Color obj)
        {
            return obj.GetHashCode();
        }

        public override bool Equals(Color x, Color y)
        {
            return x == y;
        }
    }
}