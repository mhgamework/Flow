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
        public Material[] Materials;

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
        private MeshRenderer renderer;
        VoxelChunkMeshGenerator meshGenerator;


        private int lastUpdatedFrame = -1;

        // Use this for initialization
        void Start()
        {
            meshGenerator = new VoxelChunkMeshGenerator(new MarchingCubesService());
            meshFilter = GetComponent<MeshFilter>();
            mesh = new Mesh();
            meshFilter.mesh = mesh;
            renderer = GetComponent<MeshRenderer>();

        }

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

            var colorDicts = new Dictionary<Color, int>();// Map from hardcoded colors to material ids
            colorDicts.Add(Color.green, 0);
            colorDicts.Add(Color.red, 1);
            colorDicts.Add(Color.blue, 2);

            List<Vector3> doubledVertices;
            int numMeshes;
            List<int[]> indicesList;
            List<Color> colors;
            meshGenerator.generateMesh(chunkData.Data, out doubledVertices, out numMeshes, out indicesList,out colors);

            mesh.Clear();
            mesh.SetVertices(doubledVertices);
            mesh.subMeshCount = numMeshes;
            for (int i = 0; i < indicesList.Count; i++)
                mesh.SetIndices(indicesList[i], MeshTopology.Triangles, i);

            // NOTE MATERIALS ARE SET FROM unity editor and do not respect the colors!!

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            
            GetComponent<MeshCollider>().sharedMesh = mesh;
            renderer.materials = colors.Select(c => Materials[colorDicts[c]]).ToArray();
        }



    }

}