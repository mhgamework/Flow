using UnityEngine;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    public class PlaceSphereStateMidair : IState
    {
        private VoxelWorldEditorScript script;
        private PlaceSphereState tool;
        private IEditableVoxelWorld world;
        private readonly GameObject sphereGizmo;
        private Vector3 point;

        public string Name
        {
            get { return "Add/Remove sphere midair"; }
        }
        public PlaceSphereStateMidair(VoxelWorldEditorScript script, IEditableVoxelWorld world, GameObject sphereGizmo)
        {
            this.script = script;
            this.world = world;
            this.sphereGizmo = sphereGizmo;
            this.tool = new PlaceSphereState(script, world, sphereGizmo);
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public void Update(RaycastHit? raycast)
        {

            var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            point = ray.origin + ray.direction * script.ActiveSize * 2f;

            sphereGizmo.transform.position = point;
            sphereGizmo.transform.localScale = Vector3.one * script.ActiveSize;

            tool.tryPlaceSphere(script.ActiveSize, script.ActiveMaterial, point, true);


        }

        public void OnDrawGizmos()
        {
            Gizmos.DrawSphere(point, script.ActiveSize);

        }
    }
}