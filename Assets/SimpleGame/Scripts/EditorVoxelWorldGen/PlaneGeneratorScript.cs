using System;
using Assets.MarchingCubes.SdfModeling;
using Assets.MarchingCubes.VoxelWorldMVP;
using DirectX11;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.SimpleGame.Scripts
{
    [ExecuteInEditMode]
    public class PlaneGeneratorScript : BaseVoxelObjectScript
    {

        public float Radius = 10;
        public Color Color;


        protected override void onChange()
        {
            Min = (transform.position - Radius * Vector3.one);
            Max = (transform.position + Radius * Vector3.one);

            Min = Min.ChangeY(transform.position.y - 0.01f);
            Max = Max.ChangeY(transform.position.y + 0.01f);
        }

        public override bool Sdf(Point3 p, VoxelData v, out float density, out Color color)
        {
            density = p.Y - transform.position.y;
            color = Color;
            return false;
        }
    }
}