using System.Collections.Generic;
using System.Linq;
using DirectX11;
using UnityEngine;

namespace Assets.SimpleGame.WardDrawing
{
    public class MeshWardViewScript : MonoBehaviour
    {
        public List<Vector3> Points;
        public GameObject CylinderLine;
        public float Thickness = 1;

        private bool shapeSet = false;

        public void Start()
        {
            if (!shapeSet) // For editor
                DrawCylinderShape(new[] { Points }.ToList(), transform);
        }

        public void SetShape(List<List<Vector3>> lines)
        {
            shapeSet = true;
            DrawCylinderShape(lines, transform);
        }
        public void SetShape(List<List<Point3>> lines, Matrix4x4 world)
        {
            shapeSet = true;

            DrawCylinderShape(lines.Select(l => l.Select(p => world.MultiplyPoint(p.ToVector3())).ToList()).ToList(), transform);
        }

        private void DrawCylinderShape(List<List<Vector3>> lines, Transform container)
        {
            var i = 0;
            foreach (var l in lines)
            {
                i = DrawCylinderLine(SplineUtils.GetLinePoints(l).ToList(), container, i);

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
                c.localScale = new Vector3(Thickness, Thickness, (points[i] - points[i + 1]).magnitude);
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
    }
}