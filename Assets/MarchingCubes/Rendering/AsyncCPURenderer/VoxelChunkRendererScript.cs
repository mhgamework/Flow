﻿using System;
using DirectX11;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Assets.VR;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    /// <summary>
    /// Responsible for rendering and physics of a single chunk of a VoxelWorld
    /// Dynamically updates the renderer based on the dirty feature of the voxel world chunks
    /// 
    /// The class has two modes, one where it is self sufficiently updating the voxel data, another where it is used by external update mechanisms. 
    /// This is switched with AutomaticallyGenerateMesh;
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshCollider))]
    [RequireComponent(typeof(MeshRenderer))]
    public class VoxelChunkRendererScript : MonoBehaviour
    {
        //public Material VoxelMaterial;
        private UniformVoxelData chunkData;
        public Dictionary<Color, Material> MaterialsDictionary = new Dictionary<Color, Material>();
        public Material VertexColorMaterial;

        public void SetChunk(UniformVoxelData chunkData)
        {
            this.chunkData = chunkData;
        }
        /// <summary>
        /// Only used by uniform renderer
        /// </summary>
        /// <param name="chunkCoord"></param>
        /// <param name="chunkSize"></param>
        public void SetWorldcoords(Point3 chunkCoord, Point3 chunkSize)
        {
            transform.position = chunkCoord.Multiply(chunkSize);
        }
        public void SetWorldcoords(Vector3 lowerLeft, float scale)
        {
            transform.localScale = new Vector3(scale, scale, scale);
            transform.position = lowerLeft;
        }

        /// <summary>
        /// Used to make this component autonomously track the voxel data or a subcomponent of external systems.
        /// </summary>
        public bool AutomaticallyGenerateMesh = true;

        //public int IsoSurface = 40;
        //public int Resolution = 30;
        //public float OffsetFactor = 1;
        private MeshFilter meshFilter;
        private Mesh mesh;
        private MeshRenderer renderer;
        VoxelChunkMeshGenerator meshGenerator = new VoxelChunkMeshGenerator(new MarchingCubesService());

        private VoxelMeshData lastMeshData = null;
        private int lastUpdatedFrame = -1;

        // Use this for initialization
        void Start()
        {
            initialize();

        }

        public void initialize()
        {
            if (meshFilter == null)
                meshFilter = GetComponent<MeshFilter>();

            if (mesh == null)
            {
                mesh = new Mesh();
                meshFilter.mesh = mesh;
            }
            if (renderer == null)
                renderer = GetComponent<MeshRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            if (!AutomaticallyGenerateMesh)
            {
                if (lastMeshData != null)
                    setMeshToUnity(lastMeshData);
                lastMeshData = null;
                return;
            }
            if (chunkData == null) return;
            if (lastUpdatedFrame >= chunkData.LastChangeFrame) return;
            var data = generateMesh(meshGenerator, chunkData.Data);

            setMeshToUnity(data);
            lastUpdatedFrame = Time.frameCount;
        }



        public void setMeshToUnity(VoxelMeshData data)
        {
            Start();
            //if (mesh == null)
            //{
            //    lastMeshData = data;
            //    return;
            //}
            data.UpdateMesh(mesh);

            //GetComponent<MeshCollider>().enabled = false;
            GetComponent<MeshCollider>().sharedMesh = mesh;
            if (data.colors != null)
                renderer.sharedMaterials = data.colors.Select(c =>
                {
                    Material val;
                    if (MaterialsDictionary.TryGetValue(c, out val)) return val;
                    return MaterialsDictionary[Color.black]; // Return black, this is confusing
                }).ToArray();
            else
                renderer.sharedMaterial = VertexColorMaterial;
        }

        public static VoxelMeshData generateMesh(VoxelChunkMeshGenerator meshGenerator, Array3D<VoxelData> chunkData)
        {
            return meshGenerator.GenerateMeshFromVoxelData(chunkData);

        }
    }
}