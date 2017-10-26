using Assets.Gameplay.Tools;
using Assets.MarchingCubes.VoxelWorldMVP;
using UnityEngine;

namespace Assets.Gameplay
{
    public class VoxelWorldGameplaySceneScript : MonoBehaviour
    {
        public VoxelWorldEditorScript VoxelWorldEditorScript;

        private bool cheatsEnabled = false;

        public void Start()
        {
            VoxelWorldEditorScript.StartTool = KeyCode.Alpha1;
            VoxelWorldEditorScript.RegisterTool(KeyCode.Alpha1, (w, s, p) => new NullState());
            VoxelWorldEditorScript.RegisterTool(KeyCode.Alpha2, (w, s, p) => new PushPullTool(VoxelWorldEditorScript, w, s));
            //VoxelWorldEditorScript.RegisterTool(KeyCode.Alpha2, (w, s, p) => new WithdrawDepositMagicTool(VoxelWorldEditorScript, w, s));
            VoxelWorldEditorScript.RegisterTool(KeyCode.Alpha3, (w, s, p) => new SmoothMagicTool(VoxelWorldEditorScript, w, s));
            VoxelWorldEditorScript.RegisterTool(KeyCode.Alpha4, (w, s, p) => new FlattenMagicTool(VoxelWorldEditorScript, w, s, p));
        }
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.End) && !cheatsEnabled)
            {
                cheatsEnabled = true;
                enableCheats();
            }
            if (cheatsEnabled)
            {
                var inventory = LocalPlayerSingleton.Instance.Inventory;
                inventory.Add("Fire", 9999);
                inventory.Add("Water", 9999);
                inventory.Add("Earth", 9999);
                inventory.Add("Air", 9999);
            }
        }

        private void enableCheats()
        {
            VoxelWorldEditorScript.RegisterTool(KeyCode.Alpha5, (w, s, p) => new PlaceSphereState(VoxelWorldEditorScript,w,s));
            VoxelWorldEditorScript.RegisterTool(KeyCode.Alpha6, (w, s, p) => new PlaceSphereStateMidair(VoxelWorldEditorScript, w, s));
            VoxelWorldEditorScript.RegisterTool(KeyCode.Alpha7, (w, s, p) => new ReplaceMaterialTool(VoxelWorldEditorScript, w, s));
            VoxelWorldEditorScript.RegisterTool(KeyCode.Alpha8, (w, s, p) => new DrawOnPlaneTool(VoxelWorldEditorScript, w, s, p));
            VoxelWorldEditorScript.ActivateNewTools();
        }
    }
}