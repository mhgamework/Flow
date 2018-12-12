using UnityEngine;

namespace Assets.MHGameWork.FlowEngine.SdfModeling
{
    public class Rotation : DistObject
    {
        private readonly DistObject pu;
        private readonly Quaternion angleAxis;

        public Rotation(DistObject pu, Quaternion angleAxis)
        {
            this.pu = pu;
            this.angleAxis = angleAxis;
        }

        public static Rotation Get(DistObject pu, float halfPi, Vector3 unitX)
        {
            return new Rotation(pu, Quaternion.Inverse(Quaternion.AngleAxis(halfPi, unitX)));
        }

        public override float Sdf(Vector3 p)
        {
            p = angleAxis * p;
            return pu.Sdf(p);
        }

        public override Color Color(Vector3 p)
        {
            throw new System.NotImplementedException();
        }
    }
}