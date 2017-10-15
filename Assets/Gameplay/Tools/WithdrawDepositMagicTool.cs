using Assets.MarchingCubes.VoxelWorldMVP;
using DirectX11;
using UnityEngine;

namespace Assets.Gameplay.Tools
{
    public class WithdrawDepositMagicTool : DepositTool
    {
        private LocalPlayerSingleton localPlayer;

        public WithdrawDepositMagicTool(VoxelWorldEditorScript script, IEditableVoxelWorld world, GameObject sphereGizmo)
            : base(script, world, sphereGizmo)
        {
            localPlayer = LocalPlayerSingleton.Instance;
        }

        protected override void DepositTerrainMaterial(float radius, VoxelMaterial material)
        {
            if (!localPlayer.TryUseDepositMagic(radius)) return;

            base.DepositTerrainMaterial(radius,  material);
        }
        protected override void WithdrawTerrainMaterial(float radius, VoxelMaterial material)
        {
            if (!localPlayer.TryUseWithdrawMagic(radius)) return;

            base.WithdrawTerrainMaterial(radius, material);
        }
       

     
    }
}