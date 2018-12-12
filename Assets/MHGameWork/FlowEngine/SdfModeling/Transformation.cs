using UnityEngine;

namespace Assets.MHGameWork.FlowEngine.SdfModeling
{
    public class Transformation : DistObject
    {
        private readonly DistObject pu;
        private readonly Matrix4x4 transform;

        public Transformation(DistObject pu, Matrix4x4 transform)
        {
            this.pu = pu;
            this.transform = transform.inverse;
        }

        public override float Sdf(Vector3 p)
        {
            p = transform.MultiplyPoint(p);
            return pu.Sdf(p);
        }

        public override Color Color(Vector3 p)
        {
            throw new System.NotImplementedException();
        }
    }
}