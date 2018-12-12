﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.MarchingCubes.Persistence;
using Assets.MHGameWork.FlowEngine.OctreeWorld;
using DirectX11;
using MHGameWork.TheWizards.DualContouring.Terrain;
using UnityEngine;

namespace Assets.MarchingCubes.VoxelWorldMVP.Persistence
{
    /// <summary>
    /// For use in unity editor
    /// </summary>
    public class RuntimeWorldSerializer : IWorldSerializer
    {
        /// <summary>
        /// Density clamping improves run length encoding compression by only storing data where there are surfaces and clamping the rest to a single value
        /// </summary>
        public const bool EnableDensityClamping = false; // Disabled for now, as this class doesnt use run length compression yet
        private readonly string savegamePathAndFilenamePrefix;

        public RuntimeWorldSerializer(string savegamePathAndFilenamePrefix)
        {
            this.savegamePathAndFilenamePrefix = savegamePathAndFilenamePrefix;
        }

        public void Save(int changesSinceFrame, VoxelWorldAsset asset, OctreeVoxelWorld world)
        {
            var version = createVersion(changesSinceFrame, asset, world);
            asset.Versions.Add(version);

            SaveToDisk(asset);
        }

        public VoxelWorldAsset LoadAsset(VoxelWorldAsset asset)
        {
            if (!File.Exists(getSavegameTxt())) return asset;
            // Ignore inboud asset
            try
            {
                var ret = new VoxelWorldAsset();
                LoadFromDisk(ret);
                return ret;

            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.Log("Could not load save!");
                return asset;
            }

        }

        private string getSavegameTxt()
        {
            return savegamePathAndFilenamePrefix + ".txt";
        }
        private string getSavegameDat()
        {
            return savegamePathAndFilenamePrefix + ".dat";
        }

        public VoxelWorldAsset CreateAsset(int minNodeSize)
        {
            var ret = new VoxelWorldAsset();
            CreateAsset(minNodeSize, ret);

            return ret;
        }

        public void CreateAsset(int minNodeSize, VoxelWorldAsset ret)
        {
            ret.ChunkSize = minNodeSize + 1; // Because the corner has an extra voxel !!
            ret.ChunkOversize = OctreeVoxelWorld.ChunkOversize;
            ret.Versions = new List<SerializedVersion>();
            ret.Versions.Add(new SerializedVersion()
            {
                Chunks = new List<SerializedChunk>(),
                SavedDate = new DateTime()
            });
        }

        public void ReconstructDepthAndChunkSizeFromSave(out int chunkSize, out int depth)
        {
            var metadataAsset = CreateAsset(-1); // Dummy load asset, needs refactor

            LoadFromDisk(metadataAsset, metadataOnly: true);

            // Heuristic to estimate the world depth from the stored data. Take the biggest chunk and assume thats the root. Could be incorrect i guess.
            var maxResolution = metadataAsset.Versions.SelectMany(v => v.Chunks).Max(c => c.RelativeResolution);
            depth = (int)Math.Round(Math.Log(maxResolution) / Math.Log(2));
            chunkSize = metadataAsset.ChunkSize - 1;
        }

        public void SaveToDisk(VoxelWorldAsset asset)
        {
            using (var fs = File.OpenWrite(getSavegameTxt()))
            using (var w = new BinaryWriter(File.OpenWrite(getSavegameDat())))
            {
                using (var mode = new RunLengthEncoder.WriteMode(fs))
                    PersistenceFunc(asset, mode, null, w);
            }
        }


        public void LoadFromDisk(VoxelWorldAsset asset, bool metadataOnly = false)
        {
            if (!File.Exists(getSavegameTxt()))
                throw new Exception("Cannot load level file: " + getSavegameTxt());

            using (var fs = File.OpenRead(getSavegameTxt()))
            using (var r = new BinaryReader(File.OpenRead(getSavegameDat())))
            {
                using (var mode = new RunLengthEncoder.ReadMode(fs))
                    PersistenceFunc(asset, mode, r, null, metadataOnly);
            }
        }


        private void SaveChunk(SerializedChunk chunk, BinaryWriter w)
        {
            w.Write("BEGIN RAW CHUNK\r\n");
            w.Write(chunk.Densities.Length);
            for (var i = 0; i < chunk.Densities.Length; i++)
            {
                var f = chunk.Densities[i];
                w.Write(f);
            }
            w.Write(chunk.Colors.Length);
            for (var i = 0; i < chunk.Colors.Length; i++)
            {
                var f = chunk.Colors[i];
                w.Write(f.r);
                w.Write(f.g);
                w.Write(f.b);
                w.Write(f.a);
            }
            w.Write("END RAW CHUNK\r\n");
        }
        private void LoadChunk(SerializedChunk chunk, BinaryReader r)
        {
            r.ReadString();

            chunk.Densities = new float[r.ReadInt32()];
            for (var i = 0; i < chunk.Densities.Length; i++)
            {
                chunk.Densities[i] = r.ReadSingle();
            }

            chunk.Colors = new Color[r.ReadInt32()];
            for (var i = 0; i < chunk.Colors.Length; i++)
            {
                chunk.Colors[i] = new Color(r.ReadSingle(), r.ReadSingle(), r.ReadSingle(), r.ReadSingle());
            }
            r.ReadString();

        }


        private VoxelWorldAsset PersistenceFunc(VoxelWorldAsset hermiteData, RunLengthEncoder.IMode mode, BinaryReader rData, BinaryWriter wData, bool skipChunkData = false)
        {
            mode.required("The Wizards Dual Contouring Engine - Density Grid Format - V1.0");
            mode.comment("ChunkSize ChunkOversize");
            mode.data(() => hermiteData.ChunkSize, arr => hermiteData.ChunkSize = arr);
            mode.data(() => hermiteData.ChunkOversize, arr => hermiteData.ChunkOversize = arr);

            mode.comment("Versions");
            mode.data(() => hermiteData.Versions.Count, versions => hermiteData.Versions = Enumerable.Range(0, versions).Select(f => new SerializedVersion()).ToList());
            for (int i = 0; i < hermiteData.Versions.Count; i++)
            {
                var localI = i;
                var version = hermiteData.Versions[i];
                mode.comment("Version - Num");
                mode.data(() => localI, num => { });
                mode.comment("Version - Chunks");
                mode.data(() => version.Chunks.Count, count => version.Chunks = Enumerable.Range(0, count).Select(f => new SerializedChunk()).ToList());

                for (int iChunk = 0; iChunk < version.Chunks.Count; iChunk++)
                {
                    mode.comment("LowerLeft RelativeResolution LayoutDescription");

                    var chunk = version.Chunks[iChunk];
                    mode.data(() => chunk.LowerLeft.ToArray(), count => chunk.LowerLeft = Point3.FromArray(count));
                    mode.data(() => chunk.RelativeResolution, count => chunk.RelativeResolution = count);

                    mode.comment("Layout - Densities * 10000 | Colors : " + chunk.LayoutDescription);

                    var numEls = (hermiteData.ChunkSize + hermiteData.ChunkOversize);
                    numEls = numEls * numEls * numEls;

                    if (!skipChunkData)
                        mode.data(wr =>
                        {
                            SaveChunk(chunk, wData);
                        }, r =>
                       {
                           LoadChunk(chunk, rData);
                       });
                }

            }

            return hermiteData;
        }

        public SerializedVersion createVersion(int changesSinceFrame, VoxelWorldAsset asset, OctreeVoxelWorld world)
        {
            var version = new SerializedVersion();
            version.SavedDate = new DateTime();
            version.Chunks = new List<SerializedChunk>();

            var first = true;
            new ClipMapsOctree<OctreeNode>().VisitTopDown(world.Root, n =>
            {
                //if (!first) return;
                if (n.VoxelData == null) return; // Non initialized chunk, not loaded yet
                if (n.VoxelData.LastChangeFrame < changesSinceFrame) return;

                //Debug.Log("Serializing " + n.LowerLeft + " " + n.Size + " Frame: " + n.VoxelData.LastChangeFrame);
                //first = false;
                var size = asset.ChunkSize + asset.ChunkOversize;

                var chunk = new SerializedChunk();
                chunk.RelativeResolution = world.getNodeResolution(n.Depth);
                chunk.Densities = new float[size * size * size];
                chunk.Colors = new Color[size * size * size];
                chunk.LowerLeft = n.LowerLeft;


                if (n.VoxelData.Data.Size.X != size)
                {
                    throw new InvalidOperationException(
                        "Serialized asset is not compatible with this voxel world, chunk size mismatch");
                }
                n.VoxelData.Data.ForEach((v, p) =>
                {
                    var index = SerializedChunk.toIndex(p, size);
                    if (EnableDensityClamping)
                        chunk.Densities[index] = Mathf.Clamp(v.Density, -2, 2); // Clamp to densities to compress space!
                    else
                        chunk.Densities[index] = v.Density; // Clamp to densities to compress space!
                    chunk.Colors[index] = v.Material == null ? new Color() : v.Material.color;
                });

                version.Chunks.Add(chunk);
            });
            return version;
        }


        public bool HasExistingData()
        {
            return File.Exists(getSavegameDat()) || File.Exists(getSavegameTxt());
        }
    }
}
