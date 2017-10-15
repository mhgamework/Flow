using Assets.MarchingCubes.VoxelWorldMVP;
using DirectX11;
using UnityEngine;

namespace Assets.Gameplay.Tools
{
    public class SmoothMagicTool : SmoothTool
    {
        private LocalPlayerSingleton localPlayer;

        public SmoothMagicTool(VoxelWorldEditorScript script, IEditableVoxelWorld world, GameObject sphereGizmo)
            : base(script, world, sphereGizmo)
        {
            localPlayer = LocalPlayerSingleton.Instance;
        }
        protected override void SmoothTerrain(float radius, VoxelMaterial material)
        {
            if (!localPlayer.TryUseSmoothMagic(radius)) return;

            base.SmoothTerrain(radius, material);
        }
     
    }
}