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

        public Mesh CreateMesh()
        {
            var ret = new Mesh();
            UpdateMesh(ret);
            return ret;
        }

        public void UpdateMesh(Mesh mesh)
        {
            mesh.Clear();
            mesh.SetVertices(vertices);
            mesh.subMeshCount = numMeshes;
            for (int i = 0; i < indicesList.Count; i++)
                mesh.SetIndices(indicesList[i], MeshTopology.Triangles, i);

            if (vertexColors != null)
                mesh.SetColors(vertexColors);

            // NOTE MATERIALS ARE SET FROM unity editor and do not respect the colors!! ( They should match )

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

        }
    }
}