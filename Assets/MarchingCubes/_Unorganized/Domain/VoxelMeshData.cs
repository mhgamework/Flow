using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    public class VoxelMeshData
    {
        public List<Vector3> vertices;
        public int numMeshes;
        public List<int[]> indicesList;
        public List<Color> colors;
        public List<Color> vertexColors;

        public List<int> indices;

        public static VoxelMeshData CreatePreallocated(int size = 65000)
        {
            return new VoxelMeshData()
            {
                indices = new List<int>(size),
                vertices = new List<Vector3>(size),
                vertexColors = new List<Color>(size)
            };
        }

        public Mesh CreateMesh()
        {
            var ret = new Mesh();
            UpdateMesh(ret);
            return ret;
        }

        public void Clear()
        {
            indices.Clear();
            vertexColors.Clear();
            vertices.Clear();
        }

        public void UpdateMesh(Mesh mesh)
        {
            mesh.Clear();
            mesh.SetVertices(vertices);
            mesh.subMeshCount = numMeshes;
            if (indicesList != null) throw new System.Exception("Not supported anymore");
            //for (int i = 0; i < indicesList.Count; i++)
            //    mesh.SetIndices(indicesList[i], MeshTopology.Triangles, i);
            mesh.SetTriangles(indices,0);

            if (vertexColors != null)
                mesh.SetColors(vertexColors);

            // NOTE MATERIALS ARE SET FROM unity editor and do not respect the colors!! ( They should match )

            mesh.RecalculateNormals();

        }
    }
}