using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.VoxelEngine;
using DirectX11;
using MHGameWork.TheWizards;

[ExecuteInEditMode]
public class VoxelEditComponent : MonoBehaviour
{

    public VoxelAsset InitialVoxelData;

    private VoxelData data;
    private MeshFilter meshFilter;
    private VoxelMaterial mat = new VoxelMaterial();

    public Material VoxelMaterial;
    private List<VoxelRendererComponent> parts = new List<VoxelRendererComponent>();

    public VoxelRendererComponent VoxelRendererPrefab;


    public int EditRange = 2;

    // Use this for initialization
    void Start()
    {
        if (InitialVoxelData != null)
        {
            SetVoxelData(InitialVoxelData.ToVoxelData());


        }



        //addSubpart(new Point3(0, 0, 0), new Point3(size-1, size-1, size-1));
        //addSubpart(new Point3(10, 0, 0), new Point3(20, 10, 10));
    }

    public void CreateRenderers()
    {
        var b = new Bounds();
        b.SetMinMax(new Vector3(0, 0, 0), data.GridPoints.Size - new Point3(1, 1, 1) * 2); // Magic 2 here, dont know why
        b.IterateCells(10, (x, y, z) =>
        {
            var min = new Point3(x, y, z) * 10;
            addSubpart(min, min + new Point3(1, 1, 1) * 10);
        });
    }


    private void addSubpart(Point3 viewMin, Point3 viewMax)
    {
        var obj = GameObject.Instantiate(VoxelRendererPrefab); 
        VoxelRendererComponent subPart;
        subPart = obj.GetComponent<VoxelRendererComponent>();
        //subPart = obj.AddComponent<VoxelRendererComponent>();
        //obj.AddComponent<MeshFilter>();
        //obj.AddComponent<MeshRenderer>();
        //obj.AddComponent<MeshCollider>();
        //obj.GetComponent<MeshRenderer>().material = VoxelMaterial;
        subPart.VoxelData = data;
        subPart.ViewMin = viewMin;
        subPart.ViewMax = viewMax + new Point3(1, 1, 1);
        subPart.transform.parent = transform;
        subPart.transform.localPosition = viewMin;

        parts.Add(subPart);
    }


    // Update is called once per frame
    void Update()
    {
        RaycastHit? closest = null;
        foreach (var c in parts)
        {
            if (c.raycastHitInfo == null) continue;
            if (closest == null || closest.Value.distance > c.raycastHitInfo.Value.distance)
                closest = c.raycastHitInfo;
        }
        if (closest != null)
        {
            if (Input.GetMouseButtonDown(0))
                setSphere(closest.Value.point, EditRange, null);
            if (Input.GetMouseButtonDown(1))
                setSphere(closest.Value.point, EditRange, mat);
        }


        if (Input.mouseScrollDelta.y < 0)
            setEditRange(EditRange - 1);
        if (Input.mouseScrollDelta.y > 0)
            setEditRange(EditRange + 1);


    }

    private void setEditRange(int value)
    {
        EditRange = Mathf.Max(1, value);
        parts.ForEach(p => p.HightlightRange = EditRange);
    }

    public void setSphere(Vector3 clickpoint, int radius, VoxelMaterial voxelMaterial)
    {
        var range = Vector3.one * radius;
        var bounds = new Bounds(clickpoint, range * 2);

        bounds.IterateCells(1, (x, y, z) =>
        {
            var pos = new Point3(x, y, z);
            if (data.GridPoints.InArray(pos) && (clickpoint - pos.ToVector3()).magnitude < radius)
                data.GridPoints[pos] = voxelMaterial;
        });


        parts.ForEach(p => p.markDirty(bounds.min.ToFloored(), bounds.max.ToCeiled()));

    }

    public void SetVoxelData(VoxelData voxelData)
    {
        //TODO: cleanup renderers
        foreach (var voxelRendererComponent in GetComponentsInChildren<VoxelRendererComponent>())
        {
            DestroyImmediate(voxelRendererComponent.gameObject);
        }
        data = voxelData;
        CreateRenderers();
    }

    public VoxelData GetVoxelData()
    {
        return data;
    }
}
