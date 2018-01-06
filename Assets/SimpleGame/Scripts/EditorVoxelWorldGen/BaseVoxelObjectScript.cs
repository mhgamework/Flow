using Assets.MarchingCubes.VoxelWorldMVP;
using Assets.SimpleGame.Scripts.EditorVoxelWorldGen;
using DirectX11;
using UnityEngine;

namespace Assets.SimpleGame.Scripts
{
    [ExecuteInEditMode]
    public abstract class BaseVoxelObjectScript : MonoBehaviour, IVoxelObject
    {
        public void Update()
        {
            if (transform.hasChanged)
            {
                OnValidate();
                transform.hasChanged = false;
            }
        }

        protected virtual void OnValidate()
        {
            onChange();

            var obj = EditorUniformVoxelRendererScript.InstanceOrNull;
            if (obj != null)
                obj.NotifyChanged(this);

        }

        protected abstract void onChange();

        public Vector3 Min { get; protected set; }
        public Vector3 Max { get; protected set; }
        public abstract bool Sdf(Point3 p, VoxelData v, out float density, out Color color);

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube((Max + Min) * 0.5f, Max - Min);
        }
    }
}