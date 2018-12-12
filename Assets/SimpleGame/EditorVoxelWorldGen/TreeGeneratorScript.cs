using System;
using Assets.MarchingCubes.VoxelWorldMVP;
using Assets.MHGameWork.FlowEngine.Models;
using Assets.MHGameWork.FlowEngine.SdfModeling;
using Assets.MHGameWork.FlowEngine._Cleanup.EditorVoxelWorldGen;
using DirectX11;
using MHGameWork.TheWizards;
using OldNoiseNotSurWhatFor;
using UnityEngine;

namespace Assets.SimpleGame.Scripts
{
    [ExecuteInEditMode]
    public class TreeGeneratorScript : BaseVoxelObjectScript
    {
        public float TreeBaseSize = 1;
        public float SegmentLength = 5;
        public int Seed = 0;
        public Color WoodColor;
        //public float Radius;

        private DistObject c;

        public bool noise = false;
        public float noiseScale = 1;
        public float noiseCoordScale = 1;

        private Perlin perlin;
        //public float noise2Scale = 1;
        //public float noise2CoordScale = 1;

        protected override void onChange()
        {
            perlin = new Perlin(Seed);

            var Radius = 5;

            var extendedRadius = Radius + 2;
            Min = (transform.position - extendedRadius * Vector3.one);
            Max = (transform.position + extendedRadius * Vector3.one);

            c = new Translation(new Rotation(new Cylinder(TreeBaseSize, SegmentLength), transform.rotation), transform.position);
        }

        public override void Sdf(Point3 p, VoxelData v, out float density, out Color color)
        {
            density = c.Sdf(p);
            if (noise)
                density += perlin.Noise(p.X * noiseCoordScale, p.Y * noiseCoordScale, p.Z * noiseCoordScale) * noiseScale;
            //v.Density += perlin.Noise(p.X * noise2CoordScale, p.Y* noise2CoordScale, p.Z* noise2CoordScale) * noise2Scale;
            color = WoodColor;
        }
    }
}