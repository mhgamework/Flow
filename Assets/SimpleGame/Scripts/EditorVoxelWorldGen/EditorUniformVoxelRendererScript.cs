using System;
using System.Collections.Generic;
using System.Linq;
using Assets.MarchingCubes.VoxelWorldMVP;
using Assets.Reusable;
using Assets.Reusable.Threading;
using DirectX11;
using MHGameWork.TheWizards;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

namespace Assets.SimpleGame.Scripts.EditorVoxelWorldGen
{
    [ExecuteInEditMode]
    [InitializeOnLoad]
    public class EditorUniformVoxelRendererScript : Singleton<EditorUniformVoxelRendererScript>
    {
        static EditorUniformVoxelRendererScript()
        {
            EditorApplication.update += GlobalUpdate;
        }
        static void GlobalUpdate()
        {
            var i = InstanceOrNull;
            if (i != null) i.Update();
        }
        public SDFWorldGeneratorScript World;
        private VoxelChunkMeshGenerator meshGen = new VoxelChunkMeshGenerator(new MarchingCubes.MarchingCubesService());

        public VoxelChunkRendererScript renderer;

        public int Size = 1;
        public int Resolution = 1;
        public bool update = false;
        public int ChunkSize = 8;

        private UniformVoxelData uniformVoxelData;
        private VoxelMeshData voxelMeshData;

        private static EditorUniformVoxelRendererScript instance;
        private HashSet<Point3> dirtyChunks;
        public int RenderSpeed = 1;

        public bool DrawGizmos = false;

        public Transform ChunkRenderPool;

        [SerializeField]
        [HideInInspector]
        private List<RenderInfo> renderInfos = new List<RenderInfo>();

        [Serializable]
        public class RenderInfo
        {
            public VoxelChunkRendererScript renderer;
            public Point3 Chunk;

            public RenderInfo(VoxelChunkRendererScript renderer, Point3 chunk)
            {
                this.renderer = renderer;
                Chunk = chunk;
            }
        }

        private Dictionary<Point3, VoxelChunkRendererScript> chunkDict;

        public static EditorUniformVoxelRendererScript InstanceOrNull
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<EditorUniformVoxelRendererScript>();
                return instance;
            }
        }

        private VoxelChunkRendererScript getRenderer(Point3 chunk)
        {
            if (chunkDict == null)
            {
                chunkDict = new Dictionary<Point3, VoxelChunkRendererScript>();
                foreach (var info in renderInfos)
                {
                    if (info.renderer == null) continue;
                    chunkDict[info.Chunk] = info.renderer;
                }
            }

            VoxelChunkRendererScript val;
            if (chunkDict.TryGetValue(chunk, out val)) return val;
            return null;


        }

        private VoxelChunkRendererScript getOrCreateRenderer(Point3 chunk)
        {
            var val = getRenderer(chunk);
            if (val != null) return val;

            val = createNewRenderer();
            val.name = "Chunk " + chunk;
            chunkDict[chunk] = val;
            renderInfos.Add(new RenderInfo(val, chunk));

            return val;
        }

        public void Update()
        {
            var dirtyList = getDirtyChunksList();

            if (update)
            {
                uniformVoxelData = null;
                voxelMeshData = null;
                if (chunkDict == null) chunkDict = new Dictionary<Point3, VoxelChunkRendererScript>();
                chunkDict.Clear();
                renderInfos.Clear();
                foreach (var r in ChunkRenderPool.GetComponentsInChildren<VoxelChunkRendererScript>(true).ToArray())
                {
                    DestroyImmediate(r.gameObject);
                }
                dirtyList.Clear();
                World.UpdateChildrenList();
                foreach (var v in World.ChildrenList)
                {
                    addDirtyChunks(v);
                }

                update = false;
            }
            if (dirtyList.Count > 0)
            {
                allocVoxelGenCache();

                World.UpdateChildrenList();

                for (int i = 0; i < RenderSpeed && dirtyList.Count > 0; i++)
                {
                    var chunk = dirtyList.First();
                    dirtyList.Remove(chunk);

                    var hasUpdatedNonEmpty = updateChunk(chunk.X, chunk.Y, chunk.Z);
                    if (!hasUpdatedNonEmpty) i--;// Do an extra
                }

                if (dirtyList.Count > 0) EditorUtility.SetDirty(this);

                Debug.Log("Dirty " + dirtyList.Count);

            }

        }

        private bool updateChunk(int x, int y, int z)
        {
            Profiler.BeginSample("GenerateChunk");
            World.GenerateChunk(new Point3(x, y, z) * getActualChunkSize(), ChunkSize, Resolution, uniformVoxelData);
            Profiler.EndSample();

            var r = getRenderer(new Point3(x, y, z));
           

            if (uniformVoxelData.isEmpty)
            {
                if (r != null)
                    r.gameObject.SetActive(false);
                return false;

            }

            if (r == null)
                r = getOrCreateRenderer(new Point3(x, y, z));
            r.gameObject.SetActive(true);

            voxelMeshData.Clear();

            Profiler.BeginSample("GenerateMesh");
            meshGen.GenerateMeshFromVoxelData(uniformVoxelData.Data, voxelMeshData);
            Profiler.EndSample();

            Profiler.BeginSample("SetToUnity");
            r.setMeshToUnity(voxelMeshData, new Point3(x, y, z) * getActualChunkSize(),
                1 * Resolution);
            Profiler.EndSample();
            return true;
        }

        private void allocVoxelGenCache()
        {
            if (uniformVoxelData == null)
                uniformVoxelData = World.CreateNewChunkData(ChunkSize);
            if (voxelMeshData == null)
                voxelMeshData = VoxelMeshData.CreatePreallocated();
        }

        private VoxelChunkRendererScript createNewRenderer()
        {
            return Instantiate(renderer, ChunkRenderPool.transform);
        }

        private int getNumChunks1D()
        {
            return Size / Resolution;
        }

        private int getActualChunkSize()
        {
            return (ChunkSize - 1) * Resolution;
        }

        public void NotifyChanged(IVoxelObject voxelObject)
        {
            EditorUtility.SetDirty(this);

            addDirtyChunks(voxelObject);
        }

        private void addDirtyChunks(IVoxelObject voxelObject)
        {
            var voxelObjectMin = voxelObject.Min - Vector3.one;
            var voxelObjectMax = voxelObject.Max + Vector3.one;

            var min = (voxelObjectMin / ((ChunkSize - 1) * Resolution)).ToFloored();
            var max = (voxelObjectMax / ((ChunkSize - 1) * Resolution)).ToCeiled();

            addDirtyChunks(min, max);
        }

        private void addDirtyChunks(Point3 min, Point3 max)
        {
            var l = getDirtyChunksList();


            for (int z = min.Z; z < max.Z; z++)
                for (int y = min.Y; y < max.Y; y++)
                    for (int x = min.X; x < max.X; x++)
                    {
                        l.Add(new Point3(x, y, z));
                    }
        }

        private HashSet<Point3> getDirtyChunksList()
        {
            if (dirtyChunks == null) dirtyChunks = new HashSet<Point3>();
            return dirtyChunks;
        }

        private void OnDrawGizmos()
        {
            if (!DrawGizmos) return;

            int count = 0;
            foreach (var f in dirtyChunks)
            {
                Gizmos.DrawSphere((f.ToVector3() + Vector3.one * 0.5f) * getActualChunkSize(), getActualChunkSize() / 2f);
                if (count++ > 100) break;
            }
        }


    }
}