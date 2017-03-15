using System;
using DirectX11;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    /// <summary>
    /// Responsible for rendering and physics of a single chunk of a VoxelWorld
    /// Dynamically updates the renderer based on the dirty feature of the voxel world chunks
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshCollider))]
    [RequireComponent(typeof(MeshRenderer))]
    public class VoxelChunkRenderer : MonoBehaviour
    {
        //public Material VoxelMaterial;
        private UniformVoxelData chunkData;


        public void SetChunk(UniformVoxelData chunkData)
        {
            this.chunkData = chunkData;
        }

        public void SetWorldcoords(Point3 chunkCoord, Point3 chunkSize)
        {
            transform.position = chunkCoord.Multiply(chunkSize);
        }




        //public int IsoSurface = 40;
        //public int Resolution = 30;
        //public float OffsetFactor = 1;
        private MeshFilter meshFilter;
        private Mesh mesh;

        MarchingCubesService s = new MarchingCubesService();

      

        private int lastUpdatedFrame = -1;

        // Use this for initialization
        void Start()
        {
            meshFilter = GetComponent<MeshFilter>();
            mesh = new Mesh();
            meshFilter.mesh = mesh;
            //GetComponent<MeshRenderer>().material = VoxelMaterial;

        }

        Point3[] Vertices = new Point3[] { new Point3(0, 0, 1), new Point3(1, 0, 1), new Point3(1, 0, 0), new Point3(0, 0, 0), new Point3(0, 1, 1), new Point3(1, 1, 1), new Point3(1, 1, 0), new Point3(0, 1, 0) };


        // Update is called once per frame
        void Update()
        {
            if (chunkData == null) return;
            if (lastUpdatedFrame >= chunkData.LastChangeFrame) return;
            updateMesh();
            lastUpdatedFrame = Time.frameCount;
        }

        private void updateMesh()
        {
            List<Vector3> doubledVertices;
            int numMeshes;
            List<int[]> indicesList;
            generateMesh(out doubledVertices, out numMeshes, out indicesList);

            mesh.SetVertices(doubledVertices);
            mesh.subMeshCount = numMeshes;
            for (int i = 0; i < indicesList.Count; i++)
                mesh.SetIndices(indicesList[i], MeshTopology.Triangles, i);

            // NOTE MATERIALS ARE SET FROM unity editor and do not respect the colors!!

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            GetComponent<MeshCollider>().sharedMesh = mesh;
        }

        private void generateMesh(out List<Vector3> doubledVertices, out int numMeshes, out List<int[]> indicesList)
        {
            var data = chunkData.Data;
            var triangles = new List<int>();
            var materials = new List<Color>();
            var vertices = new List<Vector3>();

            var gridvals = new double[8];
            var matvals = new Color[8];

            var maxX = data.Size.X - 1;//size.X - 2;
            var maxY = data.Size.Y - 1;//size.Y - 2;
            var maxZ = data.Size.Z - 1;//size.Z - 2;


            var actualIsoSurface = 0;

            var points = Vertices.Select(v => v.ToVector3() * 0.99f).ToArray(); // *0.99f to show edges :)

            var individualColors = new[] { Color.red, Color.blue, Color.green };
            // Voxelize per color-
            foreach (var iColor in individualColors)
                for (int x = 0; x < maxX; x++)
                {
                    for (int y = 0; y < maxY; y++)
                        for (int z = 0; z < maxZ; z++)
                        {
                            var p = new Point3(x, y, z);
                            for (int i = 0; i < 8; i++)
                            {
                                gridvals[i] = data.Get(Vertices[i] + p).Density;
                                var material = data.Get(Vertices[i] + p).Material;
                                matvals[i] = material != null ? material.color : new Color();
                                if (matvals[i] != iColor)
                                    gridvals[i] = Math.Max(gridvals[i], -gridvals[i]); // Make air by mirroring around the isosurface level, should remain identical?

                            }
                            Color outColor;
                            s.Polygonise(gridvals, matvals, points, 0, vertices, p, materials);
                        }
                }



            mesh.Clear();

            // Double the vertices to include backfaces!!
            doubledVertices = vertices.Concat(vertices).ToList();
            var outMaterials = new List<Color>();
            var groups = materials.Select((c, i) => new { mat = c, index = i * 3 }).GroupBy(f => f.mat);
            numMeshes = groups.Count();
            indicesList = new List<int[]>();
            foreach (var matPair in groups)
            {
                var color = matPair.Key;
                outMaterials.Add(color);
                // Also adds the backface for easy debugging
                var indices = matPair.SelectMany(f => new[] { f.index, f.index + 1, f.index + 2, vertices.Count + f.index, vertices.Count + f.index + 2, vertices.Count + f.index + 1 }).ToArray();
                indicesList.Add(indices);
            }
        }

    }

}