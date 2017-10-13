using UnityEngine;

namespace Assets.MineMinecraft.MathUtils
{
    public static class MathExtensions
    {
        public static Vector3 Round(this Vector3 v)
        {
            return new Vector3(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z));
        }

        public static Vector3 IntegerPart(this Vector3 v)
        {
            return new Vector3(Mathf.Sign(v.x) * Mathf.Floor(Mathf.Abs(v.x)), Mathf.Sign(v.y) * Mathf.Floor(Mathf.Abs(v.y)), Mathf.Sign(v.z) * Mathf.Floor(Mathf.Abs(v.z)));
        }
    }
}