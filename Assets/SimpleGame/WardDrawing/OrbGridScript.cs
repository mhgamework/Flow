using System.Collections.Generic;
using DirectX11;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.SimpleGame.WardDrawing
{
    public class OrbGridScript : MonoBehaviour
    {

        public bool set = false;

        public Transform GridPoint;

        public int Radius = 5;
        public float GridCellSize;
        public float PointSize;
        public float SelectedPointSize;

        private Dictionary<Point3, GameObject> dict = new Dictionary<Point3, GameObject>();

        private GameObject hoveredGridPoint;
        private Point3 selectedCell;
        private Vector3 targetPoint;
        private Plane drawPlane;

        private GameObject GridContainer;

        public bool UseCircle = true;

        public Vector3 CursorPointWorld
        {
            get { return targetPoint; }
        }

        public Vector3 HoveredPointWorld
        {
            get { return transform.TransformPoint(selectedCell.ToVector3() * GridCellSize); }
        }

        public Point3 HoveredPointLocal
        {
            get { return selectedCell; }
        }

        public void Start()
        {
            GridContainer = new GameObject("GridContainer");
            GridContainer.transform.SetParent(transform, false);


            for (int x = -Radius; x <= Radius; x++)
                for (int y = -Radius; y <= Radius; y++)
                {
                    if (x * x + y * y > Radius * Radius) continue;
                    var p = Instantiate(GridPoint.gameObject, GridContainer.transform);
                    p.gameObject.SetActive(true);
                    p.transform.localPosition = (new Vector3(x, y,0)) * GridCellSize;
                    p.transform.localScale = new Vector3(1, 1, 1) * PointSize * GridCellSize;
                    dict.Add(new Point3(x, y, 0), p);
                }
            SetPlane(new Vector3(), Vector3.forward);
        }

        public void Update()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);


            float enter;
            drawPlane.Raycast(ray, out enter);
            Debug.DrawRay(ray.origin, ray.direction);

            targetPoint = ray.GetPoint(enter);

            Debug.DrawRay(ray.GetPoint(enter), ray.direction);


            var localPoint = transform.worldToLocalMatrix.MultiplyPoint(targetPoint);

            Debug.DrawLine(CursorPointWorld, CursorPointWorld + Vector3.up, Color.red);
            Debug.DrawLine(HoveredPointWorld, CursorPointWorld + Vector3.up, Color.green);

            var cell = (localPoint * (1f / GridCellSize)).ToPoint3Rounded();
            if (hoveredGridPoint != null)
            {
                hoveredGridPoint.transform.localScale = new Vector3(1, 1, 1) * PointSize * GridCellSize;
                hoveredGridPoint = null;

            }
            if (dict.ContainsKey(cell))
            {
                hoveredGridPoint = dict[cell];
                hoveredGridPoint.transform.localScale = new Vector3(1, 1, 1) * SelectedPointSize * GridCellSize;
            }
            selectedCell = cell;

        }




        //Display a spline between 2 points derived with the Catmull-Rom spline algorithm
        public void Show(Vector3 point, Vector3 normal)
        {
            //transform.localScale = new Vector3(cellSize, cellSize, cellSize);
            gameObject.SetActive(true);
            SetPlane(point, normal);
        }

        public void Hide()
        {
        }

        public void SetPlane(Vector3 point, Vector3 normal)
        {
            //point = point.ToPoint3Rounded().ToVector3();
            transform.position = point;
            transform.LookAt(point + normal);
            /*var euler = GridContainer.transform.rotation.eulerAngles;
            euler.x = Mathf.Round(euler.x / 90f) * 90f;
            euler.y = Mathf.Round(euler.y / 90f) * 90f;
            euler.z = Mathf.Round(euler.z / 90f) * 90f;
            GridContainer.transform.rotation = Quaternion.Euler(euler);*/


            drawPlane = new Plane(transform.forward, point);



        }

    }
}