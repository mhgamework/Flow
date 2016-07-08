using System.Linq;
using Assets.TheWizards.Mathematics.DataTypes;
using DirectX11;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.VoxelEngine
{
    [ExecuteInEditMode]
    public class VoxelRendererComponent:MonoBehaviour
    {
        public VoxelData VoxelData;
        public Point3 ViewMin;
        public Point3 ViewMax;
        private MeshFilter meshFilter;
        private VoxelMaterial mat;
        public RaycastHit? raycastHitInfo;
        public int HightlightRange = 2;

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

            var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hitInfo;
            if (GetComponent<MeshCollider>().Raycast(ray, out hitInfo, 1000))
            {
                raycastHitInfo = hitInfo;
                Debug.DrawRay(ray.origin, ray.direction * hitInfo.distance, Color.yellow);

                var clickpoint = ray.GetPoint(hitInfo.distance);
                var localclickPoint = transform.InverseTransformPoint(clickpoint);
                meshFilter.sharedMesh.SetColors(
                    meshFilter.sharedMesh.vertices.Select(v => Color.Lerp(Color.red, Color.white, (localclickPoint - v).magnitude <= HightlightRange ? 0:1)).ToList());
            }
            else
            {
                if (raycastHitInfo != null)
                    meshFilter.sharedMesh.SetColors(
                    meshFilter.sharedMesh.vertices.Select(v => Color.white).ToList());

                raycastHitInfo = null;
            }


        }


        public void markDirty(Point3 min, Point3 max)
        {
            var view  = new Bounds();
            view.SetMinMax(ViewMin,ViewMax);

            var dirty = new Bounds();
            dirty.SetMinMax(min,max);

            dirty.Expand(0.1f);

            if (view.Intersects(dirty))
                updateMesh();
        }

    }
}