using System;
using DirectX11;
using UnityEngine;

namespace MHGameWork.TheWizards
{
    /// <summary>
    /// Contains helper methods for the math classes in SlimDX
    /// </summary>
    public static class UtilityExtensions
    {
        public static Vector3 ToXZ(this Vector2 v)
        {
            return v.ToXZ(0);
        }
        public static Vector3 ToXZ(this Vector2 v, float y)
        {
            return new Vector3(v.x, y, v.y);
        }
        public static Vector2 TakeXZ(this Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }
        public static Vector2 TakeXY(this Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }

        public static Vector3 ChangeX(this Vector3 v, float x)
        {
            return new Vector3(x, v.y, v.z);
        }
        public static Vector3 ChangeY(this Vector3 v, float y)
        {
            return new Vector3(v.x, y, v.z);
        }
        public static Vector3 ChangeZ(this Vector3 v, float z)
        {
            return new Vector3(v.x, v.y, z);
        }

        public static float MaxComponent(this Vector3 v)
        {
            return Math.Max(v.x, Math.Max(v.y, v.z));
        }
        public static float MaxComponent(this Vector4 v)
        {
            return Math.Max(v.x, Math.Max(v.y, Math.Max(v.z, v.w)));
        }
        public static float MaxComponent(this Vector2 v)
        {
            return Math.Max(v.x, v.y);
        }

        public static float MinComponent(this Vector3 v)
        {
            return Math.Min(v.x, Math.Min(v.y, v.z));
        }
        public static float MinComponent(this Vector4 v)
        {
            return Math.Min(v.x, Math.Min(v.y, Math.Min(v.z, v.w)));
        }
        public static float MinComponent(this Vector2 v)
        {
            return Math.Min(v.x, v.y);
        }



        public static Vector3 TakeXYZ(this Vector4 v)
        {
            return new Vector3(v.x, v.y, v.z);
        }

        public static Point3 ToPoint3Rounded(this Vector3 v)
        {
            return new Point3(((int)Math.Round(v.x)), ((int)Math.Round(v.y)), ((int)Math.Round(v.z)));
        }
        public static Point3 ToFloored(this Vector3 v)
        {
            return new Point3(((int)Math.Floor(v.x)), ((int)Math.Floor(v.y)), ((int)Math.Floor(v.z)));
        }



        public static Vector3 GetCenter(this Bounds bb)
        {
            return (bb.max + bb.min) * 0.5f;
        }
        public static Vector3 GetSize(this Bounds bb)
        {
            return bb.max - bb.min;
        }

        public static Bounds GetShrinked(this Bounds bb, float percentage)
        {
            var scale = bb.GetSize().MaxComponent();
            bb.min += Vector3.one* percentage * scale;
            bb.max -= Vector3.one * percentage * scale;
            return bb;
        }

        public static Vector3 GetPoint(this Ray ray, float dist)
        {
            return ray.origin + ray.direction* dist;
        }


        public static bool IsSameAs(this Vector3 a, Vector3 b)
        {
            return Math.Abs(a.x - b.x) < 0.0001f
                   && Math.Abs(a.y - b.y) < 0.0001f
                   && Math.Abs(a.z - b.z) < 0.0001f;
        }
        public static bool IsSameAs(this Vector2 a, Vector2 b)
        {
            return Math.Abs(a.x - b.x) < 0.0001f
                   && Math.Abs(a.y - b.y) < 0.0001f;
        }
        public static bool IsSameAs(this Quaternion a, Quaternion b)
        {
            return Math.Abs(a.x - b.x) < 0.0001f
                   && Math.Abs(a.y - b.y) < 0.0001f
                   && Math.Abs(a.z - b.z) < 0.0001f
                   && Math.Abs(a.w - b.w) < 0.0001f;
        }
    }
}
