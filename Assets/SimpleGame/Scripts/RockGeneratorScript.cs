using System;
using Assets.MarchingCubes.VoxelWorldMVP;
using DirectX11;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.SimpleGame.Scripts
{
    [ExecuteInEditMode]
    public class RockGeneratorScript : MonoBehaviour, IVoxelObject
    {
        public UniformVoxelRendererScript UniformRenderer;

        public int Seed = 0;
        public Color RockColor;
        public float Radius;

        private bool changed = false;

        public float noiseScale = 1;
        public float noiseCoordScale = 1;
        public float noise2Scale = 1;
        public float noise2CoordScale = 1;
        private Perlin perlin;

        public void Update()
        {
            if (!changed && !transform.hasChanged) return;
            perlin = new Perlin(Seed);

            var extendedRadius = Radius + 2;
            Min = (transform.position - extendedRadius * Vector3.one).ToFloored();
            Max = (transform.position + extendedRadius * Vector3.one).ToFloored();

            changed = false;
            transform.hasChanged = false;
            IsChanged = true;

        }

        private void OnValidate()
        {
            changed = true;

        }

        public bool IsChanged { get; private set; }
        public Vector3 Min { get; private set; }
        public Vector3 Max { get; private set; }
        public void Sdf(Point3 p, VoxelData v, out float density, out Color color)
        {
            density = (p.ToVector3() - transform.position).magnitude - Radius;
            density += perlin.Noise(p.X * noiseCoordScale, p.Y * noiseCoordScale, p.Z * noiseCoordScale) * noiseScale;
            density += perlin.Noise(p.X * noise2CoordScale, p.Y * noise2CoordScale, p.Z * noise2CoordScale) * noise2Scale;
            color = RockColor;
        }

        public void RemoveChanged()
        {
            IsChanged = false;
        }
    }
}