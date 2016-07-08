using UnityEngine;
using System.Collections;
using Assets.VoxelEngine;
using UnityEditor;

[CustomEditor(typeof(VoxelAsset))]
public class VoxelAssetEditor : Editor {
    public override void OnInspectorGUI()
    {
        var asset = (VoxelAsset) target;

        EditorGUILayout.IntField("SizeX", asset.SizeX, GUIStyle.none);
        EditorGUILayout.IntField("SizeY", asset.SizeY, GUIStyle.none);
        EditorGUILayout.IntField("SizeZ", asset.SizeZ, GUIStyle.none);

        EditorGUILayout.IntField("Data size", asset.GridPoints== null ? -1 : asset.GridPoints.Length, GUIStyle.none);
    }
}
