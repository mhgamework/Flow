using Assets.MHGameWork.FlowEngine.Models;
using DirectX11;
using UnityEngine;

namespace Assets.MHGameWork.FlowEngine._Cleanup.EditorVoxelWorldGen
{
    [ExecuteInEditMode]
    public abstract class BaseVoxelObjectScript : MonoBehaviour, IVoxelObject
    {
        [SerializeField]
        private bool subtract;

        public bool Subtract
        {
            get { return subtract; }
        }
        public void Start()
        {
            onChange(); // Init at runtime
        }

        private void OnEnable()
        {
            onChange(); // Init for build release?
        }



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
        public abstract void Sdf(Point3 p, VoxelData v, out float density, out Color color);

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube((Max + Min) * 0.5f, Max - Min);
        }
    }
}