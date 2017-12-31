using System;
using Assets.MarchingCubes.VoxelWorldMVP;
using DirectX11;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.SimpleGame.Scripts
{
    [ExecuteInEditMode]
    public class RockGeneratorScript : MonoBehaviour
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

        public void Update()
        {
            if (!changed && !transform.hasChanged) return;
            var perlin = new Perlin(Seed);
            var w = UniformRenderer.World;

            var extendedRadius = Radius+2;
            var min = (transform.position - extendedRadius*Vector3.one).ToFloored();
            var max = (transform.position + extendedRadius*Vector3.one).ToFloored();
            w.RunKernel1by1(min, max, (v, p) =>
            {
                v.Density = (p.ToVector3() - transform.position).magnitude - Radius;
                v.Density += perlin.Noise(p.X* noiseCoordScale, p.Y* noiseCoordScale, p.Z* noiseCoordScale) * noiseScale;
                v.Density += perlin.Noise(p.X * noise2CoordScale, p.Y* noise2CoordScale, p.Z* noise2CoordScale) * noise2Scale;
                v.Material = new VoxelMaterial() {color = RockColor};
                return v;
            }, Time.frameCount);

            changed = false;
            transform.hasChanged = false;
        }

        private void OnValidate()
        {
            changed = true;
          
        }

    }
}