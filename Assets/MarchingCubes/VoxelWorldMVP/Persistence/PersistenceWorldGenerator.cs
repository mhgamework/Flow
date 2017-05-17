using System;
using System.Collections.Generic;
using System.Linq;
using DirectX11;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using UnityEngine;
using UnityEngine.Profiling;

namespace Assets.MarchingCubes.VoxelWorldMVP.Persistence
{
    /// <summary>
    /// Decorator for world generator, using a saved asset to generate the world
    /// </summary>
    public class PersistenceWorldGenerator : IWorldGenerator
    {
        private readonly IWorldGenerator _decorated;
        private readonly VoxelWorldAsset _asset;
        private readonly int _skipVersions;

        private List<Dictionary<PosAndResolution, SerializedChunk>> optimizedVersions;

        public PersistenceWorldGenerator(IWorldGenerator decorated, VoxelWorldAsset asset, int skipVersions)
        {
            _decorated = decorated;
            _asset = asset;
            _skipVersions = skipVersions;

            // Build and optimized map
            optimizedVersions = asset.Versions.Select(v =>
            {
                var ret = new Dictionary<PosAndResolution, SerializedChunk>();
                v.Chunks.ForEach(a => ret.Add(new PosAndResolution(a.LowerLeft, a.RelativeResolution), a));
                return ret;
            }).Reverse().ToList();

        }

        private struct PosAndResolution
        {
            public Point3 Lowerleft;
            public int Resolution;

            public PosAndResolution(Point3 lowerleft, int resolution)
            {
                Lowerleft = lowerleft;
                Resolution = resolution;
            }

            public bool Equals(PosAndResolution other)
            {
                return Lowerleft.Equals(other.Lowerleft) && Resolution == other.Resolution;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is PosAndResolution && Equals((PosAndResolution)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Lowerleft.GetHashCode() * 397) ^ Resolution;
                }
            }
        }

        public UniformVoxelData Generate(Point3 start, Point3 chunkSize, int sampleResolution)
        {

            //UnityEngine.Debug.Log(chunkSize.ToString() + " " + _asset.ChunkSize + " " + _asset.ChunkOversize);
            for (int i = 0; i < 3; i++)
                if (chunkSize[i] != _asset.ChunkOversize + _asset.ChunkSize) // Should be multiple of the chunk size in the asset, minus the LOD overflow
                    throw new InvalidOperationException("The stored voxel world asset does not use the same chunk sizes");

            var relativeResolution = sampleResolution;


            // Optimized chunks is already reversed
            var theChunk = optimizedVersions.Skip(_skipVersions).Select(version =>
                {
                    SerializedChunk chunk;
                    if (version.TryGetValue(new PosAndResolution(start, relativeResolution), out chunk)) return chunk;
                    return null;
                }
            ).FirstOrDefault(f => f != null);

            if (theChunk != null)
            {
                //Debug.Log("Loaded chunk " + start + " " + chunkSize + " " + relativeResolution);

                return toChunkData(theChunk);
            }
            // This should mean that there is no change in the chunk data for this sector so use the world generator
            //Debug.Log("Unable to load chunk " + start + " " + chunkSize + " " + relativeResolution);

            return _decorated.Generate(start, chunkSize, sampleResolution);
        }

        private UniformVoxelData toChunkData(SerializedChunk chunk)
        {
            Profiler.BeginSample("toCHunkData");
            var d = new UniformVoxelData();
            d.LastChangeFrame = 0;
            var size = _asset.ChunkSize + _asset.ChunkOversize;
            d.Data = new Array3D<VoxelData>(new Point3(size, size, size));

            d.Data.ForEach((v, p) =>
            {
                var index = SerializedChunk.toIndex(p, size);

                d.Data[p] = new VoxelData()
                {
                    Density = chunk.Densities[index],
                    Material = new VoxelMaterial() { color = chunk.Colors[index] } // TODO need material map?
                };
            });


            Profiler.EndSample();
            return d;

        }
    }
}