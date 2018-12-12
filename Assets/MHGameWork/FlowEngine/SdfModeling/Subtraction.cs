using UnityEngine;

namespace Assets.MHGameWork.FlowEngine.SdfModeling
{
    public class Subtraction : DistObject
    {
        private readonly DistObject d1;
        private readonly DistObject d2;

        public Subtraction(DistObject d2, DistObject d1)
        {
            this.d1 = d1;
            this.d2 = d2;
        }

        public override float Sdf(Vector3 p)
        {
            return Mathf.Max(-d1.Sdf(p), d2.Sdf(p));
        }

        public override Color Color(Vector3 p)
        {
            throw new System.NotImplementedException();
        }
    }
}