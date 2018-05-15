using Assets.MarchingCubes.SdfModeling;
using Assets.MarchingCubes.VoxelWorldMVP;
using Assets.Reusable.Utils;
using Assets.SimpleGame._UtilsToMove;
using UnityEngine;

namespace Assets.SimpleGame.Tools
{
    public class DigTool
    {
        private VoxelWorldEditingHelper editor;


        public DigTool(VoxelWorldEditingHelper editor)
        {
            this.editor = editor;
        }

        public void Start()
        {

        }

        public void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                dig();
            }
        }

        private void dig()
        {
            var point = editor.RaycastPlayerCursor();
            if (!point.HasValue) return;

            var obj = new Ball(point.Value, 1);


            editor.Subtract(obj);

            //var s = new Ball(transform.position / scale, ExplosionRadius / scale);

            //var b = new Bounds();
            //b.SetMinMax((this.transform.position / scale - Vector3.one * ExplosionRadius / scale), (this.transform.position / scale + Vector3.one * ExplosionRadius / scale));

            //new SDFWorldEditingService().Subtract(world, s, b);
        }

      

        public void Stop()
        {

        }
    }
}