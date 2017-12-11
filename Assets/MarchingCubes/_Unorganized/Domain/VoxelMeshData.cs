using System.Collections.Generic;
using UnityEngine;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    public class VoxelMeshData
    {
        public List<Vector3> doubledVertices;
        public int numMeshes;
        public List<int[]> indicesList;
        public List<Color> colors;

        public Mesh CreateMesh()
        {
            var ret = new Mesh();
            UpdateMesh(ret);
            return ret;
        }

        public void UpdateMesh(Mesh mesh)
        {
            mesh.Clear();
            mesh.SetVertices(doubledVertices);
            mesh.subMeshCount = numMeshes;
            for (int i = 0; i < indicesList.Count; i++)
                mesh.SetIndices(indicesList[i], MeshTopology.Triangles, i);

            // NOTE MATERIALS ARE SET FROM unity editor and do not respect the colors!! ( They should match )

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

        }
    }
}