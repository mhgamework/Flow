using UnityEngine;

namespace Assets.MarchingCubes.SdfModeling
{
    public class Subtraction : DistObject
    {
        private readonly DistObject d1;
        private readonly DistObject d2;

        public Subtraction(DistObject d1, DistObject d2)
        {
            this.d1 = d2;
            this.d2 = d1;
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