using System;
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
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshCollider))]
    [RequireComponent(typeof(MeshRenderer))]
    public class VoxelChunkRendererScript : MonoBehaviour
    {
        public Dictionary<Color, Material> MaterialsDictionary = new Dictionary<Color, Material>();
        public Material VertexColorMaterial;

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
        public void setMeshToUnity(VoxelMeshData data, Vector3 lowerLeft, float scale)
        {
            initialize();

            transform.localScale = new Vector3(scale, scale, scale);
            transform.position = lowerLeft;

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



    }
}