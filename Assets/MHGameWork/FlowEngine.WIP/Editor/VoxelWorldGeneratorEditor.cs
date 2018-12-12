using Assets.MarchingCubes;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VoxelWorldGenerator))]
public class VoxelWorldGeneratorEditor : Editor {

    public override void OnInspectorGUI()
    {
        VoxelWorldGenerator worldGen = (VoxelWorldGenerator)target;

        if (DrawDefaultInspector())
        {
            if (worldGen.AutoGenerate)
            {
                worldGen.DrawMapInEditor();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            worldGen.DrawMapInEditor();
        }
    }
}
