using UnityEngine;

namespace Assets.MHGameWork.FlowEngine.SdfModeling
{
    public class Ball : DistObject
    {
        public Color color = UnityEngine.Color.black;
        private Vector3 center;
        private float radius;

        public Ball(float v1, float v2, float v3, float v4)
        {
            center = new Vector3(v1, v2, v3);
            this.radius = v4;
        }

        public Ball(Vector3 center, float radius) : this(center, radius, UnityEngine.Color.black)
        {
            
        }

        public Ball(Vector3 center, float radius,Color c)
        {
            this.center = center;
            this.radius = radius;
            this.color = c;
        }

        public override float Sdf(Vector3 p)
        {
            return length(p - center) - radius;
        }

        public override Color Color(Vector3 p)
        {
            return color;
        }
        public override void TransformScale(float f)
        {
            center *= f;
            radius *= f;
        }

        public override Bounds GetBounds()
        {
            return new Bounds(center,Vector3.one*radius*2);
        }
    }
}