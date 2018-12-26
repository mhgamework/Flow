using Assets.MHGameWork.FlowEngine.Models;
using Assets.MHGameWork.FlowEngine.SdfModeling;
using Assets.MHGameWork.FlowEngine._Cleanup.EditorVoxelWorldGen;
using DirectX11;
using UnityEngine;

namespace Assets.MHGameWork.FlowGame.Scenes.BoxLevelGenerator
{
    public class BoxVoxelObjectScript : BaseVoxelObjectScript
    {
        [SerializeField] public Color color = Color.red;
        private DistObject box;

        protected override void onChange()
        {
            box = new Transformation(new Box(0.5f, 0.5f, 0.5f), transform.localToWorldMatrix);
            Min = GetComponent<Renderer>().bounds.min;
            Max = GetComponent<Renderer>().bounds.max;
        }

        public override void Sdf(Point3 p, VoxelData v, out float density, out Color outcolor)
        {
            density= box.Sdf(p);
            outcolor = color;
        }
    }
}