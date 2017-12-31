using System;
using Assets.MarchingCubes.SdfModeling;
using Assets.MarchingCubes.VoxelWorldMVP;
using DirectX11;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.SimpleGame.Scripts
{
    [ExecuteInEditMode]
    public class PlaneGeneratorScript : MonoBehaviour, IVoxelObject
    {
        public UniformVoxelRendererScript UniformRenderer;

        public float Radius = 10;
        public Color Color;


        private bool changed = false;

        public void Update()
        {
            if (!changed && !transform.hasChanged) return;


            Min = (transform.position - Radius * Vector3.one);
            Max = (transform.position + Radius * Vector3.one);

            Min = Min.ChangeY( transform.position.y - 0.01f);
            Max = Max.ChangeY( transform.position.y + 0.01f);


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
            density = p.Y - transform.position.y;
            color = Color;
        }

        public void RemoveChanged()
        {
            IsChanged = false;
        }
    }
}