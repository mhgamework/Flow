using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.VoxelEngine;
using DirectX11;
using MHGameWork.TheWizards;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;

[ExecuteInEditMode]
public class VoxelEditController : MonoBehaviour
{
    public VoxelEditComponent VoxelComponent;

    private VoxelMaterial mat = new VoxelMaterial();

    public Material VoxelMaterial;



    public int EditRange = 2;
    private Color HighlightEditColor = Color.red;

    // Use this for initialization
    void Start()
    {
    }


    // Update is called once per frame
    void Update()
    {
        var closest = VoxelComponent.Raycast();

        VoxelComponent.updateEditHighlight(closest, EditRange, HighlightEditColor);

        if (closest != null)
        {
            if (Input.GetMouseButtonDown(0))
                setSphere(closest.Value.point, EditRange, null);
            if (Input.GetMouseButtonDown(1))
                setSphere(closest.Value.point, EditRange, mat);
            if (Input.GetKeyDown(KeyCode.KeypadMultiply))
                smooth(closest.Value.point, EditRange, mat);
        }


        if (Input.mouseScrollDelta.y < 0)
            setEditRange(EditRange - 1);
        if (Input.mouseScrollDelta.y > 0)
            setEditRange(EditRange + 1);


    }

    private void smooth(Vector3 point, int editRange, VoxelMaterial voxelMaterial)
    {
        var data = VoxelComponent.GetVoxelData();

        var range = Vector3.one * editRange;
        var bounds = new Bounds(point, range * 2);

        //TODO: this is a guassion smooth, so smooth each dir separately

        var output = new Array3D<float>(bounds.IterateCellDimensions(1));

        bounds.IterateCells(1, (x, y, z) =>
        {
            var pos = new Point3(x, y, z);

            var sum = 0;
            //sum += data.GridPoints.Get(pos) != null ? 1 : -1; // Tell the node itself double? (hackyhacky gaussion)

            //TODO: make guassion 
            // http://pages.stat.wisc.edu/~mchung/teaching/768/reading/lecture02-smoothing.pdf
            //TODO: do optimized blur (two steps onedimensional)


            var kernelSize = 2;
            for (int kX = -kernelSize + pos.X; kX <= kernelSize + pos.X; kX++)
                for (int kY = -kernelSize + pos.Y; kY <= kernelSize + pos.Y; kY++)
                    for (int kZ = -kernelSize + pos.Z; kZ <= kernelSize + pos.Z; kZ++)
                    {
                        sum += data.GridPoints[new Point3(kX, kY, kZ)] == null ? -1 : 1;
                    }
            output[pos - bounds.min.ToFloored()] = sum;
            //Debug.Log(pos + " -> " + sum);

        });

        output.ForEach((val, p) =>
        {
            var real = p + bounds.min.ToFloored();
            // Only change when substantially difference
            float substDiff = 0.2f;
            if (val > substDiff)
                data.GridPoints[real] = voxelMaterial;
            else if (val < -substDiff)
                data.GridPoints[real] = null;
            else
                ; // Do nothing
        });

        VoxelComponent.MarkDirty(bounds);
    }


    private void setEditRange(int value)
    {
        EditRange = Mathf.Max(1, value);
    }

    public void setSphere(Vector3 clickpoint, int radius, VoxelMaterial voxelMaterial)
    {
        var data = VoxelComponent.GetVoxelData();
        var range = Vector3.one * radius;
        var bounds = new Bounds(clickpoint, range * 2);

        bounds.IterateCells(1, (x, y, z) =>
        {
            var pos = new Point3(x, y, z);
            if (data.GridPoints.InArray(pos) && (clickpoint - pos.ToVector3()).magnitude < radius)
                data.GridPoints[pos] = voxelMaterial;
        });


        VoxelComponent.MarkDirty(bounds);

    }


}
