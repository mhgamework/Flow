using UnityEngine;

namespace Assets.MarchingCubes.SdfModeling
{
    public class Translation : DistObject
    {
        private DistObject stBase;
        private Vector3 vector3;

        public Translation(DistObject stBase, Vector3 vector3)
        {
            this.stBase = stBase;
            this.vector3 = vector3;
        }

        public Translation(DistObject k, float i, float f, float f1)
            : this(k, new Vector3(i, f, f1))
        {

        }

        public override float Sdf(Vector3 p)
        {
            p = -vector3 + p;
            return stBase.Sdf(p);
        }

        public override Color Color(Vector3 p)
        {
            p = -vector3 + p;
            return stBase.Color(p);
        }
    }
}