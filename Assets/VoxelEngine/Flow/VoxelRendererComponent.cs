using System.Linq;
using Assets.TheWizards.Mathematics.DataTypes;
using DirectX11;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.VoxelEngine
{
    [ExecuteInEditMode]
    public class VoxelRendererComponent : MonoBehaviour
    {
        public VoxelData VoxelData;
        public Point3 ViewMin;
        public Point3 ViewMax;
        private MeshFilter meshFilter;
        private VoxelMaterial mat;

        void Start()
        {
            mat = new VoxelMaterial();
            updateMesh();
        }

        private void updateMesh()
        {
            meshFilter = GetComponent<MeshFilter>();

            if (VoxelData == null)
            {
                meshFilter.sharedMesh = null;
                GetComponent<MeshCollider>().sharedMesh = null;
                return;
            }

            var m = VoxelData.BuildUnityMesh(ViewMin, ViewMax);
            meshFilter.sharedMesh = m;
            GetComponent<MeshCollider>().sharedMesh = meshFilter.sharedMesh;
        }

        // Update is called once per frame
        void Update()
        {



        }

        public RaycastHit? Raycast(Ray ray)
        {
            RaycastHit hitInfo;
            if (GetComponent<MeshCollider>().Raycast(ray, out hitInfo, 1000))
                return hitInfo;

            return null;
        }

        private bool highlightSet = false;

        public void ClearHightlight()
        {
            if (!highlightSet) return;
            highlightSet = false;
            meshFilter.sharedMesh.SetColors(
            meshFilter.sharedMesh.vertices.Select(v => Color.white).ToList());

        }
        public void SetHighlight(Vector3 pos, float range, Color col)
        {
            var clickpoint = pos;
            var localclickPoint = transform.InverseTransformPoint(clickpoint);

            if (!meshFilter.mesh.bounds.Intersects(new Bounds(localclickPoint, Vector3.one * range*2)))
            {
                ClearHightlight();
                return;
            }

            highlightSet = true;
            meshFilter.sharedMesh.SetColors(
                meshFilter.sharedMesh.vertices.Select(v => Color.Lerp(col, Color.white, (localclickPoint - v).magnitude <= range ? 0 : 1)).ToList());
        }



        public void markDirty(Point3 min, Point3 max)
        {
            var view = getViewBounds();

            var dirty = new Bounds();
            dirty.SetMinMax(min, max);

            dirty.Expand(0.1f);

            if (view.Intersects(dirty))
                updateMesh();
        }

        private Bounds getViewBounds()
        {
            var view = new Bounds();
            view.SetMinMax(ViewMin, ViewMax);
            return view;
        }
    }
}