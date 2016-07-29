using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.VoxelEngine;
using DirectX11;
using MHGameWork.TheWizards;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;

[ExecuteInEditMode]
public class VoxelEditComponent : MonoBehaviour
{

    public VoxelAsset InitialVoxelData;

    private VoxelData data;
    private MeshFilter meshFilter;

    public Material VoxelMaterial;
    private List<VoxelRendererComponent> parts = new List<VoxelRendererComponent>();

    public VoxelRendererComponent VoxelRendererPrefab;

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



    }


    public void updateEditHighlight(RaycastHit? closest, float range, Color color)
    {
        if (closest == null)
        {
            parts.ForEach(v => v.ClearHightlight());
            return;
        }
        parts.ForEach(v => v.SetHighlight(closest.Value.point, range, color));
    }

    public RaycastHit? Raycast()
    {
        var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit? closest = null;
        foreach (var c in parts)
        {
            var info = c.Raycast(ray);
            if (info == null) continue;
            if (closest == null || closest.Value.distance > info.Value.distance)
                closest = info;
        }
        if (closest != null)
            Debug.DrawRay(ray.origin, ray.direction * closest.Value.distance, Color.yellow);

        return closest;
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

    public void MarkDirty(Bounds bounds)
    {
        parts.ForEach(p => p.markDirty(bounds.min.ToFloored(), bounds.max.ToCeiled()));
    }
}
