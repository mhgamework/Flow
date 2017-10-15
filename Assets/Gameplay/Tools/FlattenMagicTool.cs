using Assets.MarchingCubes.VoxelWorldMVP;
using DirectX11;
using UnityEngine;

namespace Assets.Gameplay.Tools
{
    public class FlattenMagicTool : FlattenTool
    {
        private LocalPlayerSingleton localPlayer;
        public FlattenMagicTool(VoxelWorldEditorScript script, IEditableVoxelWorld world, GameObject sphereGizmo, GameObject planeGizmo)
            : base(script, world, sphereGizmo, planeGizmo)
        {
            localPlayer = LocalPlayerSingleton.Instance;
        }


        protected override void FlattenTerrain(Vector3 localPoint, Plane targetPlane, float range, VoxelMaterial material)
        {
            if (!localPlayer.TryUseFlattenMagic(range)) return;

            base.FlattenTerrain(localPoint, targetPlane, range, material);
        }


      
    }
}