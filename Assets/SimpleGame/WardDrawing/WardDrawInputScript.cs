using System.Collections.Generic;
using System.Linq;
using DirectX11;
using UnityEngine;

namespace Assets.SimpleGame.WardDrawing
{
    public class WardDrawInputScript : MonoBehaviour
    {
        private List<List<Vector3>> Lines = new List<List<Vector3>>();
        private List<List<Vector3>> Target = new List<List<Vector3>>();

        public List<Vector3> ActiveShapeEditor = new List<Vector3>();
        public List<Vector3> TargetShapeEditor = new List<Vector3>();
        public Vector3 TargetShapeOffset = new Vector3();
        public bool set = false;


        public int Size;
        public float GridCellSize;
        public float PointSize;
        public float SelectedPointSize;


        public MeshWardViewScript meshWardView;

        public Transform MouseParticles;

        public OrbGridScript orbGridScript;

        public void Start()
        {

            Lines.Add(new List<Vector3>());

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

            meshWardView.SetShape(Lines);
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
                    //DisplayCatmullRomSpline(l, i, c);
                }
            }
        }
        //private IEnumerable<Vector3> GetShapePoints(List<List<Vector3>> lines)
        //{
        //    //foreach (var l in lines)
        //    //{
        //    //    foreach (var vector3 in GetLinePoints(l)) yield return vector3;
        //    //}
        //}

 




     



        public void Show()
        {
        }

        public void Hide()
        {
        }

        public void SetPlane(Vector3 point, Vector3 normal)
        {
            Lines.Clear();
            Lines.Add(new List<Vector3>());

            orbGridScript.SetPlane(point, normal);



        }
    }
}