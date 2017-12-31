﻿using System;
using Assets.MarchingCubes.SdfModeling;
using Assets.MarchingCubes.VoxelWorldMVP;
using DirectX11;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.SimpleGame.Scripts
{
    [ExecuteInEditMode]
    public class TreeGeneratorScript : MonoBehaviour, IVoxelObject
    {
        public UniformVoxelRendererScript UniformRenderer;

        public float TreeBaseSize = 1;
        public float SegmentLength = 5;
        public int Seed = 0;
        public Color WoodColor;
        //public float Radius;

        private bool changed = false;
        private DistObject c;

        public bool noise = false;
        public float noiseScale = 1;
        public float noiseCoordScale = 1;

        private Perlin perlin;
        //public float noise2Scale = 1;
        //public float noise2CoordScale = 1;

        public void Update()
        {
            if (!changed && !transform.hasChanged) return;

            perlin = new Perlin(Seed);
            var w = UniformRenderer.World;

            var Radius = 5;

            var extendedRadius = Radius + 2;
            Min = (transform.position - extendedRadius * Vector3.one);
            Max = (transform.position + extendedRadius * Vector3.one);

            c = new Translation(new Rotation(new Cylinder(TreeBaseSize, SegmentLength), transform.rotation), transform.position);

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
            density = c.Sdf(p);
            if (noise)
                density += perlin.Noise(p.X * noiseCoordScale, p.Y * noiseCoordScale, p.Z * noiseCoordScale) * noiseScale;
            //v.Density += perlin.Noise(p.X * noise2CoordScale, p.Y* noise2CoordScale, p.Z* noise2CoordScale) * noise2Scale;
            color = WoodColor;
        }

        public void RemoveChanged()
        {
            IsChanged = false;
        }
    }
}