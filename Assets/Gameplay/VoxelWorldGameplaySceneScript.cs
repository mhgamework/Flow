using Assets.Gameplay.Tools;
using Assets.MarchingCubes.VoxelWorldMVP;
using UnityEngine;

namespace Assets.Gameplay
{
    public class VoxelWorldGameplaySceneScript : MonoBehaviour
    {
        public VoxelWorldEditorScript VoxelWorldEditorScript;

        public void Start()
        {
            VoxelWorldEditorScript.StartTool = KeyCode.Alpha0;
            VoxelWorldEditorScript.RegisterTool(KeyCode.Alpha0, (w, s, p) => new NullState());
            VoxelWorldEditorScript.RegisterTool(KeyCode.Alpha1, (w, s, p) => new WithdrawDepositMagicTool(VoxelWorldEditorScript, w, s));
            VoxelWorldEditorScript.RegisterTool(KeyCode.Alpha2, (w, s, p) => new SmoothMagicTool(VoxelWorldEditorScript, w, s));
            VoxelWorldEditorScript.RegisterTool(KeyCode.Alpha3, (w, s, p) => new FlattenMagicTool(VoxelWorldEditorScript, w, s, p));
        }
    }
}