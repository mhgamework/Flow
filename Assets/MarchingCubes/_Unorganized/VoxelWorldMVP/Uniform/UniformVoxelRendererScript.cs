using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Reusable;
using Assets.Reusable.Threading;
using DirectX11;
using MHGameWork.TheWizards;
using MHGameWork.TheWizards.DualContouring;
using UnityEngine;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    [ExecuteInEditMode]
    public class UniformVoxelRendererScript : MonoBehaviour
    {
        public Material VertexColorMaterial;
        public int ChunkSize = 16;
        public int Size = 3;

        private VoxelMaterial MaterialGreen = new VoxelWorldMVP.VoxelMaterial() { color = Color.green };
        private VoxelMaterial MaterialRed = new VoxelWorldMVP.VoxelMaterial() { color = Color.red };
        private VoxelMaterial MaterialBlue = new VoxelWorldMVP.VoxelMaterial() { color = Color.blue };

        private ConcurrentObjectPool<VoxelMeshData> meshDataPool;
        VoxelChunkMeshGenerator gen = new VoxelChunkMeshGenerator(new MarchingCubesService());

        private Dictionary<Point3, RenderInfo> chunks;

        public void Start()
        {
            if (meshDataPool == null)
                meshDataPool = new ConcurrentObjectPool<VoxelMeshData>(() => VoxelMeshData.CreatePreallocated(), Size * Size * Size, 10, new MockDispatcher());

            if (World == null)
                World = new UniformVoxelWorld(new ConstantVoxelWorldGenerator(-1, MaterialGreen), new Point3(ChunkSize, ChunkSize, ChunkSize));
            //World = new UniformVoxelWorld(new DelegateVoxelWorldGenerator(v => new VoxelData(v.y - 8, MaterialGreen)), new Point3(ChunkSize, ChunkSize, ChunkSize));


            if (chunks == null)
            {
                foreach (var c in GetComponentsInChildren<VoxelChunkRendererScript>().ToArray())
                    DestroyImmediate(c.gameObject);
                chunks = new Dictionary<Point3, RenderInfo>();
                createRenderers(new Point3(1, 1, 1) * Size);
            }


        }

        public UniformVoxelWorld World { get; private set; }

        public void createRenderers(Point3 chunksToRender)
        {
            Point3.ForEach(chunksToRender, p =>
            {
                var obj = new GameObject();
                obj.transform.SetParent(transform);
                var subRenderer = obj.AddComponent<VoxelChunkRendererScript>();
                subRenderer.VertexColorMaterial = VertexColorMaterial;
                chunks.Add(p, new RenderInfo(subRenderer, -1));
            });
        }




        public void Update()
        {
            UpdateRenderers();
        }

        public void UpdateRenderers()
        {
            Start();

            Point3.ForEach(new Point3(1, 1, 1) * Size, p =>
            {
                var data = World.GetChunk(p);
                var info = chunks[p];

                if (info.renderedFrame == data.LastChangeFrame) return;

                var meshData = meshDataPool.Take();
                meshData.Clear();

                gen.GenerateMeshFromVoxelData(data.Data, meshData);

                info.renderer.setMeshToUnity(meshData, p * ChunkSize, 1);
                info.renderedFrame = data.LastChangeFrame;

                meshDataPool.Release(meshData);
            });
        }

        private class RenderInfo
        {
            public VoxelChunkRendererScript renderer;
            public int renderedFrame;

            public RenderInfo(VoxelChunkRendererScript renderer, int renderedFrame)
            {
                this.renderer = renderer;
                this.renderedFrame = renderedFrame;
            }
        }
    }
}
