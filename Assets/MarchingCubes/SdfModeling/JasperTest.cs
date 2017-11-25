using Assets.MarchingCubes.VoxelWorldMVP;
using UnityEngine;
using UnityStandardAssets.Vehicles.Ball;

namespace Assets.MarchingCubes.SdfModeling
{
    public class JasperTest
    {
        public static DistObject createA()
        {
            Box baseP = new Box(1, .75f, .4f);
            DistObject baseC = new Translation(new Box(1, .3f, .14f), 0, .45f, .3f);
            return new Translation(new Subtraction(baseP, baseC), 0f, .75f, 0);
        }

        public static DistObject createApple()
        {
            Ball apple = new Ball(new Vector3(0, 0, 0), .1f);
            Ball sub = new Ball(new Vector3(0, .15f, 0), .1f);
            Cylinder stBase = new Cylinder(.02f, .03f);
            stBase.color = Color.green;
            DistObject st = new Translation(stBase, new Vector3(0, .1f, 0));
            apple.color = Color.red;

            // Was Union without N
            return new UnionN(new Subtraction(apple, sub), st);
        }

        public static DistObject createSkull()
        {
            Cylinder a = new Cylinder(.215f, .05f);
            a.color = Color.red;
            DistObject tsa = new Translation(Rotation.Get(a, Mathf.PI / 2, Vector3.right), 0, .5f, -.5f);
            Cylinder cb = new Cylinder(.05f, .1f);
            DistObject tsb = new Translation(Rotation.Get(cb, Mathf.PI / 2, Vector3.right), 0, .5f, -.4f);
            Ball head = new Ball(0, .5f, 0, .3f);
            Ball EH0 = new Ball(0.3f, .7f, .3f, .4f);
            Ball EH1 = new Ball(-.3f, .7f, .3f, .4f);
            Ball EH2 = new Ball(0f, .9f, 0f, .3f);
            Ball EY0 = new Ball(0.11f, .5f, -.05f, .11f);
            EY0.color = Color.red;
            Ball EY1 = new Ball(-0.11f, .5f, -.05f, .11f);
            EY1.color = Color.red;
            Ball ER0 = new Ball(0.21f, .65f, -.2f, .15f);
            Ball ER1 = new Ball(-0.21f, .65f, -.2f, .15f);
            DistObject Skull = new Subtraction(new UnionN(ER0, ER1, head), new UnionN(EH0, EH1, EH2));
            return new UnionN(Skull, tsb, tsa, EY0, EY1);
        }


    }

    public class Box : DistObject
    {
        private Vector3 b;

        public Box(float x, float y, float z)
        {
            this.b = new Vector3(x, y, z);
        }

        public override float Sdf(Vector3 p)
        {
            var d = abs(p) - b;
            return min(max(d.x, max(d.y, d.z)), 0) + length(max(d, 0));
        }

        public override Color Color(Vector3 p)
        {
            throw new System.NotImplementedException();
        }
    }

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

    public class Rotation : DistObject
    {
        private readonly DistObject pu;
        private readonly Quaternion angleAxis;

        private Rotation(DistObject pu, Quaternion angleAxis)
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

    public class Cylinder : DistObject
    {
        private readonly float f;
        private readonly float f1;
        public Color color;

        public Cylinder(float f, float f1)
        {
            this.f = f;
            this.f1 = f1;
        }

        public override float Sdf(Vector3 p)
        {
            var h = new Vector2(f, f1);
            var d = abs(vec2(length(xz(p)), p.y)) - h;
            return min(max(d.x, d.y), 0) + length(max(d, 0));
        }

        public override Color Color(Vector3 p)
        {
            return color;
        }


    }
}