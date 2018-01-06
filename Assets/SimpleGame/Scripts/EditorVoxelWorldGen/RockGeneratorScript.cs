using System;
using Assets.MarchingCubes.VoxelWorldMVP;
using DirectX11;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.SimpleGame.Scripts
{
    public class RockGeneratorScript : BaseVoxelObjectScript
    {

        public int Seed = 0;
        public Color RockColor;

        public float Radius;

        private bool changed = false;

        public float noiseScale = 1;
        public float noiseCoordScale = 1;
        public float noise2Scale = 1;
        public float noise2CoordScale = 1;
        private Perlin perlin;


        protected override void onChange()
        {
            perlin = new Perlin(Seed);

            var extendedRadius = Radius + (noise2Scale + noiseScale) * 0.5f; // I think the perlin is between -0.5 -> 0.5
            Min = (transform.position - extendedRadius * Vector3.one);
            Max = (transform.position + extendedRadius * Vector3.one);
        }

        public override bool Sdf(Point3 p, VoxelData v, out float density, out Color color)
        {
            if (perlin == null)
                perlin = new Perlin(Seed);
            density = (p.ToVector3() - transform.position).magnitude - Radius;
            var local = p - transform.position;
            density += perlin.Noise(local.x * noiseCoordScale, local.y * noiseCoordScale, local.z * noiseCoordScale) * noiseScale;
            density += perlin.Noise(local.x * noise2CoordScale, local.y * noise2CoordScale, local.z * noise2CoordScale) * noise2Scale;
            color = RockColor;

            return false;
        }
    }
}