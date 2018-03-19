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
        public float Depth;
        public Color Color;

        public float Color2Depth;
        public Color Color2;
        public float Color3Depth;
        public Color Color3;

        private Vector3 pos;

        protected override void onChange()
        {
            Min = (transform.position - Radius * Vector3.one);
            Max = (transform.position + Radius * Vector3.one);

            Min = Min.ChangeY(transform.position.y - Depth);
            Max = Max.ChangeY(transform.position.y + 0.01f);

            pos = transform.position; // Cache pos
        }

        public override void Sdf(Point3 p, VoxelData v, out float density, out Color color)
        {
            var d = p.Y - pos.y;
            density = d;
            if (d < -Color3Depth)
                color = Color3;
            else if (d < -Color2Depth)
                color = Color2;
            else
                color = Color;
        }
    }
}