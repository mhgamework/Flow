using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.VoxelEngine;
using DirectX11;
using MHGameWork.TheWizards;

public class MapBuilderMain : MonoBehaviour
{

    private VoxelData data;
    private MeshFilter meshFilter;
    private int size;
    private VoxelMaterial mat;

    public Material VoxelMaterial;
    private List<VoxelRendererComponent> parts = new List<VoxelRendererComponent>();


    public VoxelEditComponent Voxels;

    // Use this for initialization
    void Start()
    {

        var editComp = Voxels;


        mat = new VoxelMaterial();

        size = 100;
        data = new VoxelData(new Point3(size + 2, size + 2, size + 2));

        data.GridPoints.ForEach((v, p) =>
        {
            if ((p - new Vector3(size * 0.5f, size * 0.5f, size * 0.5f)).magnitude < size * 0.4f)
                data.GridPoints[p] = mat;
            else
                data.GridPoints[p] = null;
        });

        editComp.SetVoxelData(data);

    }



}
