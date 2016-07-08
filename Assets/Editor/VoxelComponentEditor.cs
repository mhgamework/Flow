using Assets.UnityAdditions;
using UnityEditor;
using UnityEngine;

namespace Assets.VoxelEngine
{
    [CustomEditor(typeof(VoxelEditComponent))]
    public class VoxelComponentEditor : Editor
    {


        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Save OVERRIDE"))
            {


                var comp = (VoxelEditComponent)target;

                if (comp.InitialVoxelData == null)
                {
                    EditorUtility.DisplayDialog("Cannot save!", "No voxel asset set!", "Close");
                }
                else if (EditorUtility.DisplayDialog("Overwrite asset",
                    "Are you sure you want to override asset with name (" + comp.InitialVoxelData.name + ")", "Yes",
                    "No"))
                {
                    AssetDatabase.Refresh();
                    comp.InitialVoxelData.FromVoxelData(comp.GetVoxelData());
                    EditorUtility.SetDirty(comp.InitialVoxelData);

                    AssetDatabase.SaveAssets();
                }

            }
            if (GUILayout.Button("Save New"))
            {
                var asset = ScriptableObjectUtility.CreateAsset<VoxelAsset>("Assets/voxeldata");


                var comp = (VoxelEditComponent)target;

                asset.FromVoxelData(comp.GetVoxelData());
                EditorUtility.SetDirty(asset);

                AssetDatabase.SaveAssets();

            }


        }
    }
}