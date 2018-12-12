using System;
using Assets.UnityAdditions;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.MarchingCubes
{
    public class TerrainTest
    {

        float hash(float n) { return fract(sin(n) * 753.5453123f); }
        float hash(Vector3 p)  // replace this by something better
        {
            p = 50.0f * fract(p * 0.3183099f + new Vector3(0.71f, 0.113f, 0.419f));
            return -1.0f + 2.0f * fract(p.x * p.y * p.z * (p.x + p.y + p.z));
        }

        //---------------------------------------------------------------
        // value noise, and its analytical derivatives
        //---------------------------------------------------------------

        public Vector4 noised(Vector3 x)
        {

            var p = floor(x);
            var w = fract(x);
            var u = w.Multiply(w).Multiply(Vector3.one * 3.0f - 2.0f * w);
            var du = 6.0f * w.Multiply(Vector3.one * 1.0f - w);

            float n = p.x + p.y * 157.0f + 113.0f * p.z;

            float a = hash(p + new Vector3(0.0f, 0.0f, 0.0f));
            float b = hash(p + new Vector3(1.0f, 0.0f, 0.0f));
            float c = hash(p + new Vector3(0.0f, 1.0f, 0.0f));
            float d = hash(p + new Vector3(1.0f, 1.0f, 0.0f));
            float e = hash(p + new Vector3(0.0f, 0.0f, 1.0f));
            float f = hash(p + new Vector3(1.0f, 0.0f, 1.0f));
            float g = hash(p + new Vector3(0.0f, 1.0f, 1.0f));
            float h = hash(p + new Vector3(1.0f, 1.0f, 1.0f));

            float k0 = a;
            float k1 = b - a;
            float k2 = c - a;
            float k3 = e - a;
            float k4 = a - b - c + d;
            float k5 = a - c - e + g;
            float k6 = a - b - e + f;
            float k7 = -a + b + c - d + e - f - g + h;

            var first = k0 + k1 * u.x + k2 * u.y + k3 * u.z + k4 * u.x * u.y + k5 * u.y * u.z + k6 * u.z * u.x + k7 * u.x * u.y * u.z;
            var second = du.Multiply(new Vector3(k1, k2, k3) + yzx(u).Multiply(new Vector3(k4, k5, k6)) +
                                     zxy(u).Multiply(new Vector3(k6, k4, k5)) + k7 * yzx(u).Multiply(zxy(u)));

            return new Vector4(first, second.x, second.y, second.z);
        }

        public Vector4 fbmd(Vector3 x, float f1)
        {
            const float scale = 1.5f;

            float a = 0.0f;
            float b = 0.5f;
            float f = 1.0f;
            var d = new Vector3();
            for (int i = 0; i < 1; i++) // was 8
            {
                if (f * 1.1f > (1f / f1)) break;// To high frequency

                Vector4 n = noised(f * x * scale);
                a += b * n.x;           // accumulate values		
                d += b * yzw(n) * f * scale; // accumulate derivatives
                b *= 0.5f;             // amplitude decrease
                f *= 1.8f;             // frequency increase
            }

            return new Vector4(a, d.x, d.y, d.z);
        }

        private Vector3 yzx(Vector3 v)
        {
            return new Vector3(v.y, v.z, v.x);
        }
        private Vector3 zxy(Vector3 v)
        {
            return new Vector3(v.z, v.x, v.y);
        }
        public static Vector3 yzw(Vector4 v)
        {
            return new Vector3(v.y, v.z, v.w);
        }
        private Vector3 floor(Vector3 vector3)
        {
            return vector3.ToFloored();
        }

        float fract(float n)
        {
            return n - (float)Math.Truncate(n);
        }

        Vector3 fract(Vector3 n)
        {
            return new Vector3(fract(n.x), fract(n.y), fract(n.z));
        }
        Vector2 fract(Vector2 n)
        {
            return new Vector2(fract(n.x), fract(n.y));
        }
        Vector2 floor(Vector2 n)
        {
            return new Vector2(Mathf.Floor(n.x), Mathf.Floor(n.y));
        }

        float sin(float f)
        {
            return Mathf.Sin(f);
        }


        //const mat2 m = mat2(0.8, -0.6, 0.6, 0.8);
        float hash1(float n)
        {
            return fract(n * 17.0f * fract(n * 0.3183099f));
        }
        float hash1(Vector2 p)
        {
            p = 50.0f * fract(p * 0.3183099f);
            return fract(p.x * p.y * (p.x + p.y));
        }
        Vector3 noised(Vector2 x)
        {
            var p = floor(x);
            var w = fract(x);

            var u = w.Multiply(w).Multiply(w).Multiply(w.Multiply(w * 6.0f - 15.0f * Vector2.one) + 10.0f * Vector2.one);
            var du = 30.0f * w.Multiply(w).Multiply(w.Multiply(w - 2.0f * Vector2.one) + 1.0f * Vector2.one);

            float a = hash1(p + new Vector2(0, 0));
            float b = hash1(p + new Vector2(1, 0));
            float c = hash1(p + new Vector2(0, 1));
            float d = hash1(p + new Vector2(1, 1));

            float k0 = a;
            float k1 = b - a;
            float k2 = c - a;
            float k4 = a - b - c + d;

            var gradient = 2.0f * du.Multiply(new Vector2(k1 + k4 * u.y,
                               k2 + k4 * u.x));
            return new Vector3(-1.0f + 2.0f * (k0 + k1 * u.x + k2 * u.y + k4 * u.x * u.y),
                gradient.x, gradient.y);
        }

        public Vector3 fbmd(Vector2 x)
        {
            const float scale = 1.5f;

            float a = 0.0f;
            float b = 0.5f;
            float f = 1.0f;
            var d = new Vector2();
            for (int i = 0; i < 8; i++)
            {
                //if (f * 1.1f > (1f / f1)) break;// To high frequency

                Vector3 n = noised(x);//f * x * scale);
                a += b * n.x;           // accumulate values		
                d += b * new Vector2(n.y, n.z) * f * scale; // accumulate derivatives
                b *= 0.55f;             // amplitude decrease

                f *= 1.9f;             // frequency increase

                //const mat2 m2 = mat2(0.80, 0.60,
                //    -0.60, 0.80);
                // 0.8 -0.6
                // 0.6 0.8

                // m*x
                x = 1.9f * x;//(x.x * new Vector2(0.8f, 0.6f) + x.y * new Vector2(-0.6f, 0.8f));
            }

            return new Vector4(a, d.x, d.y);
        }


        public float terrainWierd(Vector2 p)
        {


            float a = 0.0f;
            float b = 1.0f;
            Vector2 d = new Vector2();
            for (int i = 0; i < 2; i++)//was 15
            {
                var n = noised(p);
                d += new Vector2(n.y, n.z);
                a += b * n.x / (1.0f + Vector2.Dot(d, d));
                b *= 0.5f;
                // Was m * p
                p = new Vector2(0.8f, -0.6f) * p.x + new Vector2(0.6f, 0.8f) * 2.0f;
            }
            return a;
        }
    }
}