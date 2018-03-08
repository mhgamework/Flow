using System.Collections.Generic;
using System.Linq;
using DirectX11;
using UnityEngine;

namespace Assets.SimpleGame.WardDrawing
{
    public class WardDrawInputScript : MonoBehaviour
    {
        private List<List<Vector3>> Lines = new List<List<Vector3>>();
        private List<List<Point3>> LinesLocal = new List<List<Point3>>();
        //private List<List<Vector3>> Target = new List<List<Vector3>>();

        public List<Vector3> ActiveShapeEditor = new List<Vector3>();
        //public List<Vector3> TargetShapeEditor = new List<Vector3>();
        //public Vector3 TargetShapeOffset = new Vector3();
        public bool set = false;


        public int Size;
        public float GridCellSize;
        public float PointSize;
        public float SelectedPointSize;


        public MeshWardViewScript meshWardView;

        public Transform MouseParticles;

        public OrbGridScript orbGridScript;

        public WardScriptableObject wardScriptableObject ;
        public WardScriptableObject wardScriptableObject2;

        public List<List<Vector3>> CurrentShapeWorld
        {
            get { return Lines; }
        }
        public List<List<Point3>> CurrentShapeLocal
        {
            get { return LinesLocal; }
        }

        public void Start()
        {
            wardScriptableObject = new WardScriptableObject();
            ClearCurrentShape();

            //for (int i = 0; i < TargetShapeEditor.Count; i++)
            //{
            //    if (TargetShapeEditor[i] == new Vector3(-1, -1, -1)) continue;
            //    TargetShapeEditor[i] += TargetShapeOffset;
            //}
        }

        private void ClearCurrentShape()
        {
            Lines.Clear();
            Lines.Add(new List<Vector3>());
            LinesLocal.Clear();
            LinesLocal.Add(new List<Point3>());
        }

        public void Update()
        {
            ActiveShapeEditor = WardDrawingUtils.FlattenShape(LinesLocal);
            wardScriptableObject.Points = ActiveShapeEditor;
            //Target = fromEditor(TargetShapeEditor);
            if (set)
            {
                //TargetShapeEditor = toEditor(Lines);
                set = false;
            }
            //if (selected != null)
            if (Input.GetMouseButtonDown(0))
            {
                Lines.Last().Add(orbGridScript.HoveredPointWorld);
                LinesLocal.Last().Add(orbGridScript.HoveredPointLocal);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                Lines.Add(new List<Vector3>());
                LinesLocal.Add(new List<Point3>());
            }

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (Lines.Count > 0) { 
                    Lines.RemoveAt(Lines.Count - 1);
                    LinesLocal.RemoveAt(Lines.Count - 1);
                }
            }


            //var matcher = new WardComparer();

            //var matches = matcher.Match(Lines, Target);

            Lines.Last().Add(orbGridScript.CursorPointWorld);

            meshWardView.SetShape(Lines);
            Lines.Last().Remove(orbGridScript.CursorPointWorld);


            //DrawShape(Target, Color.gray);
            Lines.Last().Add(orbGridScript.CursorPointWorld);
            //DrawShape(Lines, Color.white);
            Lines.Last().Remove(orbGridScript.CursorPointWorld);

            //if (matches.Any())
            //{
            //    DrawShape(matches.OrderByDescending(m => m.MatchingXLines.Count).First().MatchingXLines, Color.yellow);

            //}

            MouseParticles.position = orbGridScript.CursorPointWorld;

        }


     
        public void SetPlane(Vector3 point, Vector3 normal)
        {
            ClearCurrentShape();

            orbGridScript.SetPlane(point, normal);



        }
    }
}