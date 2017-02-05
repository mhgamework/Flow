using System;
using DirectX11;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshCollider))]
    [RequireComponent(typeof(MeshRenderer))]
    public class VoxelChunkRenderer : MonoBehaviour
    {
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

        }

        Point3[] Vertices = new Point3[] { new Point3(0, 0, 1), new Point3(1, 0, 1), new Point3(1, 0, 0), new Point3(0, 0, 0), new Point3(0, 1, 1), new Point3(1, 1, 1), new Point3(1, 1, 0), new Point3(0, 1, 0) };


        // Update is called once per frame
        void Update()
        {
            if (lastUpdatedFrame >= chunkData.LastChangeFrame) return;
            updateMesh();
            lastUpdatedFrame = Time.frameCount;
        }

        private void updateMesh()
        {
            var data = chunkData.Data;
            var triangles = new List<int>();
            var vertices = new List<Vector3>();

            var gridvals = new double[8];

            var size = data.Size;//  new Point3(1, 1, 1) * Resolution;

            var maxX = size.X - 1;
            var maxY = size.Y - 1;
            var maxZ = size.Z - 1;


            //var offset = new Vector3(Mathf.Sin(Time.realtimeSinceStartup + 2), Mathf.Cos(Time.realtimeSinceStartup + 3), Mathf.Sin(Time.realtimeSinceStartup + 7)) * Resolution * OffsetFactor;

            //var actualIsoSurface = IsoSurface * 0.01f * Resolution / 30f;

            var points = Vertices.Select(v => v.ToVector3()).ToArray();


            for (int x = 0; x < maxX; x++)
                for (int y = 0; y < maxY; y++)
                    for (int z = 0; z < maxZ; z++)
                    {
                        var p = new Point3(x, y, z);
                        for (int i = 0; i < 8; i++)
                        {
                            //var pos = Vertices[i] + p;
                            //var diff = pos - (size.ToVector3() * 0.5f + offset*Mathf.Abs(Mathf.Sin(Time.realtimeSinceStartup)));
                            //var val = diff.magnitude;
                            //val =Mathf.Min(val, (pos - (size.ToVector3() * 0.5f - offset * Mathf.Abs(Mathf.Sin(Time.realtimeSinceStartup)))).magnitude);
                            //cell.val[i] =val; //data.Get(Vertices[i] + p);
                            gridvals[i] = data.Get(Vertices[i] + p);
                        }
                        //var outTriangles = new List<TRIANGLE>();

                        //s.Polygonise(gridvals, points, actualIsoSurface, vertices, p);
                        s.Polygonise(gridvals, points, 0, vertices, p);
                        //outTriangles.ForEach(t =>
                        //{
                        //    vertices.Add(t.p[0]);
                        //    vertices.Add(t.p[2]); // Invert culling
                        //    vertices.Add(t.p[1]);
                        //});
                    }
            mesh.Clear();
            //mesh.SetVertices(vertices.Select(v => v / Resolution - new Vector3(1, 1, 1) * 0.5f).ToList());
            mesh.SetVertices(vertices.ToList());
            mesh.SetIndices(vertices.Select((v, i) => i).ToArray(), MeshTopology.Triangles, 0);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            GetComponent<MeshCollider>().sharedMesh = mesh;
            //transform.localScale = new Vector3(1,1,1)* 1f / Resolution;
        }
    }
}