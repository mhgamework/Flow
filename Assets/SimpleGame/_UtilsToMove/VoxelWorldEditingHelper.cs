using Assets.MarchingCubes.Domain;
using Assets.MarchingCubes.Rendering;
using Assets.MarchingCubes.VoxelWorldMVP;
using Assets.MarchingCubes.World;
using Assets.MHGameWork.FlowEngine.OctreeWorld;
using Assets.MHGameWork.FlowEngine.SdfModeling;
using Assets.MHGameWork.FlowEngine._Cleanup;
using Assets.Reusable.Utils;
using Boo.Lang;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.SimpleGame._UtilsToMove
{
    /// <summary>
    /// Responsible for providing easy-to-use editing operations for the voxel world
    /// Takes into account render scaling
    /// Also added raycast method as it is commonly used at the same time
    /// </summary>
    public class VoxelWorldEditingHelper
    {
        private VoxelRenderingEngineScript renderingEngine;
        private OctreeVoxelWorld world;

        public VoxelWorldEditingHelper(VoxelRenderingEngineScript renderingEngine, OctreeVoxelWorld world)
        {
            this.renderingEngine = renderingEngine;
            this.world = world;
        }


        public void Subtract(DistObject obj, SDFWorldEditingService.Counts outCounts)
        {
            var scale = renderingEngine.RenderScale;

            obj.TransformScale(1f / scale);

            var b = obj.GetBounds();

            new SDFWorldEditingService().Subtract(world, obj, b, outCounts);
        }

        public Vector3? RaycastPlayerCursor()
        {
            var cast = new VoxelWorldRaycaster().raycast();
            return cast.Select(f => f.point);
        }


        public void Smooth(Vector3 center, float radius, SDFWorldEditingService.Counts counts)
        {
            var scale = renderingEngine.RenderScale;

            center /= scale;
            radius /= scale;

            WorldEditOperations.SmoothTerrain((Vector3.one * radius).ToCeiled(), null, Time.frameCount, world, center,counts);
        }
    }
}