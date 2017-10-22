using System.Collections.Generic;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.Flow.WardDrawing
{
    public class WardDrawInput3D : MonoBehaviour
    {
        public int GridSize = 3;
        public float Distance = 5;
        public float DistanceGap = 5;
        public float GridPointSize = 0.1f;
        public GameObject GridPoint;

        public Transform PlayerPos;
        private List<Transform> points = new List<Transform>();

        public void Start()
        {

        }
        public void Update()
        {
            var center = PlayerPos.position.ToPoint3Rounded().ToVector3();
            var offset = -GridSize * 0.5f * new Vector3(1, 1, 1);

            var iP = 0;
            for (int x = 0; x < GridSize; x++)
                for (int y = 0; y < GridSize; y++)
                    for (int z = 0; z < GridSize; z++)
                    {
                        var pos = center + offset + new Vector3(x, y, z);  
                        var dist = (pos - center).magnitude;
                        if (dist < Distance - DistanceGap || dist > Distance + DistanceGap) continue;
                        var point = getOrCreatePoint(iP++);
                        point.transform.position = pos;

                        point.localScale = new Vector3(1, 1, 1) * GridPointSize;
                        point.gameObject.SetActive(true);
                    }

            while (iP < points.Count)
            {
                points[iP++].gameObject.SetActive(false);
            }
        }

        private Transform getOrCreatePoint(int i)
        {
            while (points.Count <= i)
            {
                var instantiate = Instantiate(GridPoint);
                instantiate.transform.SetParent(transform);
                points.Add(instantiate.transform);
            }

            return points[i];
        }
    }
}