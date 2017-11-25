using UnityEngine;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    class NullState : IState
    {
        public string Name
        {
            get { return "No tool"; }
        }
        public void Stop()
        {
        }

        public void Update(RaycastHit? raycast)
        {
        }

        public void Start()
        {
        }

        public void OnDrawGizmos()
        {

        }
    }
}