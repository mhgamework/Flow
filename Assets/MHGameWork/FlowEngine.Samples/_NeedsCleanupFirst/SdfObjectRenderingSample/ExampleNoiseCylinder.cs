using Assets.MHGameWork.FlowEngine.Models;
using Assets.MHGameWork.FlowEngine.Samples._NeedsCleanupFirst.SdfObjectRenderingSample.SystemToMove;
using Assets.MHGameWork.FlowEngine.SdfModeling;
using DirectX11;
using OldNoiseNotSurWhatFor;
using UnityEngine;

namespace Assets.MHGameWork.FlowEngine.Samples._NeedsCleanupFirst.SdfObjectRenderingSample
{
    public class ExampleNoiseCylinder : ISuggestedSdfInterfaceColor
    {
        public float TreeBaseSize = 5;
        public float SegmentLength = 20;
        public int Seed = 0;
        public Color WoodColor = Color.black;
        //public float Radius;

        private DistObject c;

        public bool noise = true;
        public float noiseScale = 3;
        public float noiseCoordScale = 0.1f;

        private Perlin perlin;
        //public float noise2Scale = 1;
        //public float noise2CoordScale = 1;

        public ExampleNoiseCylinder()
        {
            onChange();
        }

        protected void onChange()
        {
            perlin = new Perlin(Seed);

            var Radius = 5;

            var extendedRadius = Radius + 2;
//            Min = (transform.position - extendedRadius * Vector3.one);
//            Max = (transform.position + extendedRadius * Vector3.one);

            var rotation = Quaternion.identity;
            var pos = new Vector3(10,10,10);

            c = new Translation(new Rotation(new Cylinder(TreeBaseSize, SegmentLength), rotation), pos);
        }

        public void GetSdf(Vector3 p, out SdfDataColor outData)
        {
            outData.Distance= c.Sdf(p);
            if (noise)
                outData.Distance += perlin.Noise(p.x * noiseCoordScale, p.y * noiseCoordScale, p.z * noiseCoordScale) * noiseScale;
            //v.Density += perlin.Noise(p.X * noise2CoordScale, p.Y* noise2CoordScale, p.Z* noise2CoordScale) * noise2Scale;
            outData.Color = WoodColor;
        }

        public void SetDirtyHandler(IDirtyHandler handler)
        {
            // Ignore for now
        }
    }
}