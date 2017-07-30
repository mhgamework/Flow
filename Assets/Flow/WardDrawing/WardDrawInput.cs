using System.Collections.Generic;
using System.Linq;
using DirectX11;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.Flow.WardDrawing
{
    public class WardDrawInput : MonoBehaviour
    {
        private List<List<Vector3>> Lines = new List<List<Vector3>>();
        private List<List<Vector3>> Target = new List<List<Vector3>>();

        public List<Vector3> ActiveShapeEditor = new List<Vector3>();
        public List<Vector3> TargetShapeEditor = new List<Vector3>();
        public bool set = false;

        public Transform GridPoint;

        public int Size;
        public float GridCellSize;
        public float PointSize;
        public float SelectedPointSize;

        private Dictionary<Point3, GameObject> dict = new Dictionary<Point3, GameObject>();

        private GameObject selected;
        private Point3 selectedCell;
        private Vector3 targetPoint;

        public void Start()
        {
            Lines.Add(new List<Vector3>());
            for (int x = 0; x < Size; x++)
                for (int y = 0; y < Size; y++)
                {
                    var p = Instantiate(GridPoint.gameObject, transform);
                    p.transform.position = new Vector3(x, y) * GridCellSize;
                    p.transform.localScale = new Vector3(1, 1, 1) * PointSize;
                    dict.Add(new Point3(x, y, 0), p);
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
            updateSelected();
            if (selected != null)
                if (Input.GetMouseButtonDown(0))
                {
                    Lines.Last().Add(selectedCell);
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

            DrawShape(Target, Color.gray);
            Lines.Last().Add(targetPoint);
            DrawShape(Lines, Color.white);
            Lines.Last().Remove(targetPoint);

            if (matches.Any())
            {
                DrawShape(matches.OrderByDescending(m => m.MatchingXLines.Count).First().MatchingXLines, Color.yellow);

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

        private void updateSelected()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            var plane = new Plane(Vector3.forward, 0);

            float enter;
            plane.Raycast(ray, out enter);
            Debug.DrawRay(ray.origin, ray.direction);

            targetPoint = ray.GetPoint(enter);

            Debug.DrawRay(ray.GetPoint(enter), ray.direction);


            var cell = (targetPoint * (1f / GridCellSize)).ToPoint3Rounded();
            if (selected != null)
            {
                selected.transform.localScale = new Vector3(1, 1, 1) * PointSize;
                selected = null;

            }
            if (dict.ContainsKey(cell))
            {
                selected = dict[cell];
                selected.transform.localScale = new Vector3(1, 1, 1) * SelectedPointSize;
                selectedCell = cell;
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


        //Display a spline between 2 points derived with the Catmull-Rom spline algorithm
        void DisplayCatmullRomSpline(List<Vector3> controlPointsList, int pos, Color color)
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

            for (int i = 1; i <= loops; i++)
            {
                //Which t position are we at?
                float t = i * resolution;

                //Find the coordinate between the end points with a Catmull-Rom spline
                Vector3 newPos = GetCatmullRomPosition(t, p0, p1, p2, p3);

                //Draw this line segment
                Debug.DrawLine(lastPos, newPos, color);

                //Save this pos so we can draw the next line segment
                lastPos = newPos;
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
    }
}