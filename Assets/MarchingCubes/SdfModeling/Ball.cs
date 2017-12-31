using UnityEngine;

namespace Assets.MarchingCubes.SdfModeling
{
    public class Ball : DistObject
    {
        public object color;
        private Vector3 vector3;
        private float v;

        public Ball(float v1, float v2, float v3, float v4)
        {
            vector3 = new Vector3(v1, v2, v3);
            this.v = v4;
        }

        public Ball(Vector3 vector3, float v)
        {
            this.vector3 = vector3;
            this.v = v;
        }

        public override float Sdf(Vector3 p)
        {
            return length(p - vector3) - v;
        }

        public override Color Color(Vector3 p)
        {
            throw new System.NotImplementedException();
        }
    }
}