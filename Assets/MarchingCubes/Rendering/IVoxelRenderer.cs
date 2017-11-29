using System.Collections.Generic;
using Assets.MarchingCubes.Rendering.AsyncCPURenderer;
using Assets.MarchingCubes.VoxelWorldMVP;

namespace Assets.MarchingCubes.Rendering
{
    public interface IVoxelRenderer
    {
        int UnavailableChunks { get; }

        /// <summary>
        /// Prepare a chunk to be rendered. Should generate all render data offscreen.
        /// Results in CanShowChunk becoming true
        /// </summary>
        /// <param name="tempTaskList"></param>
        void PrepareShowChunk(List<ChunkCoord> tasks);

        bool CanShowChunk(ChunkCoord c);

        /// <summary>
        /// Warning: Allthough this is called show/hide, these methods are currently
        /// probably NOT idempotent.
        /// </summary>
        VoxelChunkRendererScript ShowChunk(ChunkCoord node, out int frame);

        /// <summary>
        /// Warn: Not idempotent!!
        /// </summary>
        /// <param name="renderObject"></param>
        void HideChunk(VoxelChunkRendererScript renderObject);

        int GetLastChangeFrame(ChunkCoord node);
    }
}