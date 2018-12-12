using UnityEngine;

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
}