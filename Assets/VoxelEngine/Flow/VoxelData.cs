using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.DualContouring;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using UnityEngine;

namespace Assets.VoxelEngine
{
    public class VoxelData
    {
        private Array3D<VoxelMaterial> gridPoints;

        public Array3D<VoxelMaterial> GridPoints
        {
            get { return gridPoints; }
            set { gridPoints = value; }
        }

        public VoxelData(Point3 size)
        {
            gridPoints = new Array3D<VoxelMaterial>(size);
        }

        public Mesh BuildUnityMesh(Point3 min, Point3 max)
        {
            var algo = new DCUniformGridAlgorithm(new MidPointQEF());

            List<int> indices = new List<int>();
            AbstractHermiteGrid grid = new ArrayHermiteGrid(gridPoints,min,max-min);
            Dictionary<Point3, int> vIndex = new Dictionary<Point3, int>();
            List<DCVoxelMaterial> mats = new List<DCVoxelMaterial>();
            List<Vector3> vertices = new List<Vector3>();
            algo.GenerateSurface(vertices, indices, grid);
            Mesh mesh = new Mesh();

            // Link all
            //mesh.vertices = vertices.ToArray();
            ////mesh.uv = newUV;
            //mesh.triangles = indices.ToArray();

            // All separate triangles
            mesh.vertices = indices.Select(i => vertices[i]).ToArray();
            mesh.triangles = indices.Select((i, index) => index).ToArray();

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            return mesh;
        }
        public Mesh BuildUnityMesh()
        {
            return BuildUnityMesh(new Point3(0, 0, 0), gridPoints.Size);
        }

    }
}
