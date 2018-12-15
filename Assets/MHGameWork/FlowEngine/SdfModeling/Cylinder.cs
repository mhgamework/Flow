using UnityEngine;

namespace Assets.MHGameWork.FlowEngine.SdfModeling
{
    public class Cylinder : DistObject
    {
        private readonly float radius;
        private readonly float f1;
        public Color color;

        public Cylinder(float radius, float length) : this(radius, length, UnityEngine.Color.black)
        {

        }
        public Cylinder(float radius, float length,Color c)
        {
            this.radius = radius;
            this.f1 = length;
            this.color = c;
        }

        public override float Sdf(Vector3 p)
        {
            var h = new Vector2(radius, f1);
            var d = abs(vec2(length(xz(p)), p.y)) - h;
            return min(max(d.x, d.y), 0) + length(max(d, 0));
        }

        public override Color Color(Vector3 p)
        {
            return color;
        }


    }
}