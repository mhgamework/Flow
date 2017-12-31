using UnityEngine;

namespace Assets.MarchingCubes.SdfModeling
{
    public class UnionN : DistObject
    {
        private readonly DistObject[] obj;

        public UnionN(params DistObject[] obj)
        {
            this.obj = obj;
        }

        public override float Sdf(Vector3 p)
        {
            var min = float.MaxValue;
            for (int i = 0; i < obj.Length; i++)
            {
                var f = obj[i].Sdf(p);
                if (f < min) min = f;
            }
            return min;
        }

        public override Color Color(Vector3 p)
        {
            throw new System.NotImplementedException();
        }
    }
}