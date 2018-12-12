using System;
using UnityEngine;

namespace Assets.MHGameWork.FlowEngine.SdfModeling
{
    public abstract class DistObject
    {
        public abstract float Sdf(Vector3 p);
        public abstract Color Color(Vector3 p);

        protected Vector2 min(Vector2 p, float f)
        {
            return new Vector2(Mathf.Min(p.x, f), Mathf.Min(p.y, f));
        }

        protected float min(float p, float f)
        {
            return Mathf.Min(p, f);
        }
        protected float max(float p, float f)
        {
            return Mathf.Max(p, f);
        }

        protected Vector2 max(Vector2 p, float f)
        {
            return new Vector2(Mathf.Max(p.x, f), Mathf.Max(p.y, f));
        }
        protected Vector3 max(Vector3 p, float f)
        {
            return new Vector3(Mathf.Max(p.x, f), Mathf.Max(p.y, f), Mathf.Max(p.z, f));
        }

        protected Vector3 abs(Vector3 v)
        {
            return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
        }

        protected Vector2 abs(Vector2 v)
        {
            return new Vector2(Mathf.Abs(v.x), Mathf.Abs(v.y));
        }

        protected Vector2 vec2(float length1, float pY)
        {
            return new Vector2(length1, pY);
        }

        protected float length(Vector3 p0)
        {
            return p0.magnitude;
        }
        protected float length(Vector2 p0)
        {
            return p0.magnitude;
        }

        protected Vector2 xz(Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }

        public virtual void TransformScale(float f)
        {
            throw new NotImplementedException();
        }

        public virtual Bounds GetBounds()
        {
            throw new NotImplementedException();
        }
    }
}