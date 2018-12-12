using UnityEngine;

namespace Assets.MHGameWork.FlowEngine.SdfModeling
{
    public class Box : DistObject
    {
        private Vector3 b;

        public Box(float x, float y, float z)
        {
            this.b = new Vector3(x, y, z);
        }

        public override float Sdf(Vector3 p)
        {
            var d = abs(p) - b;
            return min(max(d.x, max(d.y, d.z)), 0) + length(max(d, 0));
        }

        public override Color Color(Vector3 p)
        {
            throw new System.NotImplementedException();
        }
    }
}