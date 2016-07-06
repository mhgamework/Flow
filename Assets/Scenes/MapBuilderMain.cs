using UnityEngine;
using System.Collections;
using Assets.VoxelEngine;
using DirectX11;

public class MapBuilderMain : MonoBehaviour
{

    private VoxelData data;
    private MeshFilter meshFilter;

    // Use this for initialization
    void Start()
    {
        var mat = new VoxelMaterial();

        data = new VoxelData(new Point3(10, 10, 10));

        data.GridPoints.ForEach((v, p) =>
        {
            if ((p - new Vector3(5, 5, 5)).magnitude < 2)
                data.GridPoints[p] = mat;
        });

        meshFilter = GetComponent<MeshFilter>();

        meshFilter.mesh = data.BuildUnityMesh();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
