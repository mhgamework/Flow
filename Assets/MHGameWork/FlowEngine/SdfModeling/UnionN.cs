using UnityEngine;

namespace Assets.MHGameWork.FlowEngine.SdfModeling
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
            //TODO: very suboptimal to evaluate sdf twice
            var min = float.MaxValue;
            var minI = 0;
            for (int i = 0; i < obj.Length; i++)
            {
                var f = obj[i].Sdf(p);
                if (f < min)
                {
                    min = f;
                    minI = i;
                }
            }
            return obj[minI].Color(p);
        }
    }
}