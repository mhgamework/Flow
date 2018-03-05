using System.Collections.Generic;
using System.Linq;
using a;
using Assets.UnityAdditions;
using DirectX11;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.Flow.WardDrawing
{
    public class WardDrawInputScript : MonoBehaviour
    {
        private List<List<Vector3>> Lines = new List<List<Vector3>>();
        private List<List<Vector3>> Target = new List<List<Vector3>>();

        public List<Vector3> ActiveShapeEditor = new List<Vector3>();
        public List<Vector3> TargetShapeEditor = new List<Vector3>();
        public Vector3 TargetShapeOffset = new Vector3();
        public bool set = false;

        public Transform CylinderLine;

        public int Size;
        public float GridCellSize;
        public float PointSize;
        public float SelectedPointSize;

        private Dictionary<Point3, GameObject> dict = new Dictionary<Point3, GameObject>();


        private GameObject LineContainer;

        public Transform MouseParticles;

        public OrbGridScript orbGridScript;

        public void Start()
        {

            Lines.Add(new List<Vector3>());

            LineContainer = new GameObject("LineContainer");
            LineContainer.transform.SetParent(transform, false);

            for (int i = 0; i < TargetShapeEditor.Count; i++)
            {
                if (TargetShapeEditor[i] == new Vector3(-1, -1, -1)) continue;
                TargetShapeEditor[i] += TargetShapeOffset;
            }

        }

        public void Update()
        {
            ActiveShapeEditor = toEditor(Lines);
            Target = fromEditor(TargetShapeEditor);
            if (set)
            {
                TargetShapeEditor = toEditor(Lines);
                set = false;
            }
            //if (selected != null)
            if (Input.GetMouseButtonDown(0))
            {
                Lines.Last().Add(orbGridScript.HoveredPointWorld);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                Lines.Add(new List<Vector3>());
            }

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (Lines.Count > 0)
                    Lines.RemoveAt(Lines.Count - 1);
            }


            var matcher = new WardComparer();

            var matches = matcher.Match(Lines, Target);

            Lines.Last().Add(orbGridScript.CursorPointWorld);

            DrawCylinderShape(Lines, LineContainer.transform);
            Lines.Last().Remove(orbGridScript.CursorPointWorld);


            DrawShape(Target, Color.gray);
            Lines.Last().Add(orbGridScript.CursorPointWorld);
            DrawShape(Lines, Color.white);
            Lines.Last().Remove(orbGridScript.CursorPointWorld);

            if (matches.Any())
            {
                DrawShape(matches.OrderByDescending(m => m.MatchingXLines.Count).First().MatchingXLines, Color.yellow);

            }

            MouseParticles.position = orbGridScript.CursorPointWorld;

        }

        private void DrawCylinderShape(List<List<Vector3>> lines, Transform container)
        {
            var i = 0;
            foreach (var l in Lines)
            {
                i = DrawCylinderLine(GetLinePoints(l).ToList(), LineContainer.transform, i);

            }
            ClearCylinderLines(container, i);

        }

        private int DrawCylinderLine(List<Vector3> points, Transform container, int start)
        {
            int i = 0;
            for (; i + 1 < points.Count; i++)
            {
                if (i + start == container.childCount)
                    Instantiate(CylinderLine, container);

                var c = container.GetChild(i + start);
                c.gameObject.SetActive(true);
                c.position = points[i];
                c.LookAt(points[i + 1]);
                c.localScale = new Vector3(1, 1, (points[i] - points[i + 1]).magnitude);
            }

            return i + start;
        }

        private static void ClearCylinderLines(Transform container, int i)
        {
            for (; i < container.childCount; i++)
            {
                container.GetChild(i).gameObject.SetActive(false);
            }
        }

        private List<List<Vector3>> fromEditor(List<Vector3> targetShapeEditor)
        {
            var ret = new List<List<Vector3>>();
            var current = new List<Vector3>();
            foreach (var l in targetShapeEditor)
            {
                if (l.Equals(new Vector3(-1, -1, -1)))
                {
                    ret.Add(current);
                    current = new List<Vector3>();
                    continue;
                }
                current.Add(l);
            }
            return ret;
        }

        private List<Vector3> toEditor(List<List<Vector3>> lines)
        {
            var ret = new List<Vector3>();
            foreach (var l in lines)
            {
                foreach (var k in l)
                {
                    ret.Add(k);
                }
                ret.Add(new Vector3(-1, -1, -1));

            }
            return ret;
        }

        private void DrawShape(List<List<Vector3>> lines, Color c)
        {
            foreach (var l in lines)
            {
                for (int i = 0; i < l.Count - 1; i++)
                {
                    DisplayCatmullRomSpline(l, i, c);
                }
            }
        }
        private IEnumerable<Vector3> GetShapePoints(List<List<Vector3>> lines)
        {
            foreach (var l in lines)
            {
                foreach (var vector3 in GetLinePoints(l)) yield return vector3;
            }
        }

        private IEnumerable<Vector3> GetLinePoints(List<Vector3> l)
        {
            for (int i = 0; i < l.Count - 1; i++)
            {
                foreach (var p in GetCatmullSplinePoints(l, i)) yield return p;
            }
        }




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

        IEnumerable<Vector3> GetCatmullSplinePoints(List<Vector3> controlPointsList, int pos)
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

        //Display a spline between 2 points derived with the Catmull-Rom spline algorithm
        void DisplayCatmullRomSpline(List<Vector3> controlPointsList, int pos, Color color)
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


        //Returns a position between 4 Vector3 with Catmull-Rom spline algorithm
        //http://www.iquilezles.org/www/articles/minispline/minispline.htm
        Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
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

        public void Show()
        {
        }

        public void Hide()
        {
        }

        public void SetPlane(Vector3 point, Vector3 normal)
        {
            orbGridScript.SetPlane(point, normal);



        }
    }
}