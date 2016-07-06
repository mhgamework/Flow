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
        }

        public VoxelData(Point3 size)
        {
            gridPoints = new Array3D<VoxelMaterial>(size);
        }


        public Mesh BuildUnityMesh()
        {
            var algo  = new DCUniformGridAlgorithm(new MidPointQEF());

            List<int> indices = new List<int>();
            AbstractHermiteGrid grid = new ArrayHermiteGrid(gridPoints);
            Dictionary<Point3, int> vIndex  = new Dictionary<Point3, int>();
            List<DCVoxelMaterial> mats = new List<DCVoxelMaterial>();
            List<Vector3> vertices = new List<Vector3>();
            algo.GenerateSurface(vertices,indices,grid);
            Mesh mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            //mesh.uv = newUV;
            mesh.triangles = indices.ToArray();

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            return mesh;
        }

    }
}
