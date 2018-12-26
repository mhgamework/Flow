using System.Collections.Generic;
using System.Linq;
using Assets.MHGameWork.FlowEngine.Models;
using Assets.MHGameWork.FlowEngine.SdfWorldGeneration;
using Assets.Reusable;
using DirectX11;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using UnityEngine;
using UnityEngine.Profiling;

namespace Assets.MHGameWork.FlowEngine._Cleanup.EditorVoxelWorldGen
{
    /**
     * Provides a method to generate chunk data based on the Nested IVoxelObjects in the game object tree.
     */
    public class SDFWorldGeneratorScript : MonoBehaviour
    {
        public IVoxelObject[] ChildrenList { get; private set; }
        private List<IVoxelObject> tempList;

        private SdfVoxelWorldGenerator chunkGenerator;
        private SimpleSdfObjectWorld sdfObjectWorld;

        private void init()
        {
            sdfObjectWorld = new SimpleSdfObjectWorld();
            chunkGenerator = new SdfVoxelWorldGenerator( sdfObjectWorld,new VoxelMaterialFactory());
            tempList = new List<IVoxelObject>();
        }

        /// <summary>
        /// Cache for the child lookup, to reduce GC alloc
        /// </summary>
        public void UpdateChildrenList()
        {
            Profiler.BeginSample("UpdateChildrenList");
            ChildrenList = GetComponentsInChildren<IVoxelObject>();
            sdfObjectWorld.Objects = ChildrenList.ToList();
            if (tempList == null) tempList = new List<IVoxelObject>();
            Profiler.EndSample();
        }

        public void GenerateChunk(Point3 lowerleft, int size, int resolution, UniformVoxelData outData)
        {
            if (chunkGenerator == null) init();
            chunkGenerator.Generate(lowerleft, new Point3(size,size,size), resolution, outData);
        }

        public UniformVoxelData CreateNewChunkData(int chunkSize)
        {
            return SdfVoxelWorldGenerator.CreateNewChunkData(chunkSize);
        }
    }
}