using MHGameWork.TheWizards.DualContouring.QEFs;
using UnityEngine;

namespace Assets.VoxelEngine
{
    public class MidPointQEF : IQefCalculator
    {
        public Vector3 CalculateMinimizer(Vector3[] normals, Vector3[] posses, int numIntersections, Vector3 preferredPosition)
        {
            return Vector3.one*0.5f;
        }
    }
}