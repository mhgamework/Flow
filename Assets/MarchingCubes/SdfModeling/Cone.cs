using UnityEngine;

namespace Assets.MarchingCubes.SdfModeling
{
    public class Cone : DistObject
    {
        private readonly Vector3 c;

        public Cone(Vector3 c)
        {
            this.c = c; //.normalized;
        }

        public override float Sdf(Vector3 p)
        {
            //// c must be normalized
            //float q = length(vec2(p.x, p.y));
            //return Vector2.Dot(_c, vec2(q, p.z));

            // From the raymarcher sample code
            var q = vec2(length(vec2(p.x, p.y)), p.y);
            float d1 = -q.y - c.z;
            float d2 = max(Vector2.Dot(q, vec2(c.x,c.y)), q.y);
            // TODO this max seems not correct maybe?
            return length(max(vec2(d1, d2), 0)) + min(max(d1, d2), 0) ;

        }


        public override Color Color(Vector3 p)
        {
            throw new System.NotImplementedException();
        }
    }
}