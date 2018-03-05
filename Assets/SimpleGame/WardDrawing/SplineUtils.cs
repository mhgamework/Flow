using System.Collections.Generic;
using UnityEngine;

namespace Assets.SimpleGame.WardDrawing
{
    public static class SplineUtils
    {
        public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return
                oneMinusT * oneMinusT * oneMinusT * p0 +
                3f * oneMinusT * oneMinusT * t * p1 +
                3f * oneMinusT * t * t * p2 +
                t * t * t * p3;
        }

        public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return
                oneMinusT * oneMinusT * p0 +
                2f * oneMinusT * t * p1 +
                t * t * p2;
        }

        public static IEnumerable<Vector3> GetCatmullSplinePoints(List<Vector3> controlPointsList, int pos)
        {
            var a = pos - 1;
            var b = pos;
            var c = pos + 1;
            var d = pos + 2;
            var isLoop = controlPointsList[0] == controlPointsList[controlPointsList.Count - 1];

            var end = controlPointsList.Count - 1;

            if (a < 0) a = isLoop ? end - 1 : 0;
            if (d > end) d = isLoop ? 1 : end;

            //The 4 points we need to form a spline between p1 and p2
            Vector3 p0 = controlPointsList[a];
            Vector3 p1 = controlPointsList[b];
            Vector3 p2 = controlPointsList[c];
            Vector3 p3 = controlPointsList[d];

            //The start position of the line
            Vector3 lastPos = p1;

            //The spline's resolution
            //Make sure it's is adding up to 1, so 0.3 will give a gap, but 0.2 will work
            float resolution = 0.2f;

            //How many times should we loop?
            int loops = Mathf.FloorToInt(1f / resolution);
            yield return lastPos;
            for (int i = 1; i <= loops; i++)
            {
                //Which t position are we at?
                float t = i * resolution;

                //Find the coordinate between the end points with a Catmull-Rom spline
                Vector3 newPos = GetCatmullRomPosition(t, p0, p1, p2, p3);

                yield return newPos;
                //Draw this line segment
                //Debug.DrawLine(lastPos, newPos, color);

                //Save this pos so we can draw the next line segment
                lastPos = newPos;
            }
        }


        //Returns a position between 4 Vector3 with Catmull-Rom spline algorithm
        //http://www.iquilezles.org/www/articles/minispline/minispline.htm
        public static Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            //The coefficients of the cubic polynomial (except the 0.5f * which I added later for performance)
            Vector3 a = 2f * p1;
            Vector3 b = p2 - p0;
            Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
            Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

            //The cubic polynomial: a + b * t + c * t^2 + d * t^3
            Vector3 pos = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));

            return pos;
        }

        //Display a spline between 2 points derived with the Catmull-Rom spline algorithm
        public static void DebugDrawlineCatmullRomSpline(List<Vector3> controlPointsList, int pos, Color color)
        {
            Vector3 lastPos = new Vector3();
            bool first = true;
            foreach (var p in GetCatmullSplinePoints(controlPointsList, pos))
            {
                if (!first)
                {
                    Debug.DrawLine(lastPos, p, color);

                }
                first = false;
                lastPos = p;
            }

        }

        public static IEnumerable<Vector3> GetLinePoints(List<Vector3> l)
        {
            for (int i = 0; i < l.Count - 1; i++)
            {
                foreach (var p in GetCatmullSplinePoints(l, i)) yield return p;
            }
        }
    }
}