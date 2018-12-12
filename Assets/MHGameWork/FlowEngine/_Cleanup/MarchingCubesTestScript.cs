using System;
using System.Linq;
using Assets.MarchingCubes;
using Assets.MarchingCubes.VoxelWorldMVP;
using DirectX11;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using UnityEngine;

/// <summary>
/// Control script for the marching cubes test screen
/// Used in the demo for 170814
/// For testing the marchingcubes renderer
/// </summary>
public class MarchingCubesTestScript : MonoBehaviour
{
    public int Resolution = 30;
    public float Size = 10f;

    public bool Change = true;
    public int Min = 1;
    public int Max = 16;
    public float ChangeSpeed = 1;

    public TestModeEnum TestMode;

    public Material TemplateMaterial;
    private VoxelChunkRendererScript voxelChunkRenderer;

    public enum TestModeEnum
    {
        MultiMaterial = 0,
        WorldEditTool

    }

    // Use this for initialization
    void Start()
    {
        voxelChunkRenderer = GetComponent<VoxelChunkRendererScript>();
        voxelChunkRenderer.MaterialsDictionary = new[] { Color.green, Color.red, Color.blue }.ToDictionary(c => c, c =>
           {
               var ret = new Material(TemplateMaterial);
               ret.color = c;
               return ret;
           });
    }

    private UniformVoxelData createChunkData()
    {

        var ret = new UniformVoxelData();
        ret.Data = new Array3D<VoxelData>(new Point3(Resolution, Resolution, Resolution));

        switch (TestMode)
        {
            case TestModeEnum.MultiMaterial:
                fillDataMultiMaterial(ret);
                break;
            case TestModeEnum.WorldEditTool:
                fillDataEditTool(ret);
                break;


        }

        return ret;
    }



    private void fillDataMultiMaterial(UniformVoxelData ret)
    {
        var sphereRadius = 0.3f;
        var sphereA = new Vector3(0, 0, 0);
        var sphereB = new Vector3(1, 1, 1);
        var sphereC = new Vector3(0.5f, 0.6f, 0.4f);
        var sphereD = new Vector3(0.5f, 0.4f, 0.6f);


        var material1 = new VoxelMaterial() { color = Color.green };
        var material2 = new VoxelMaterial() { color = Color.red };
        var material3 = new VoxelMaterial() { color = Color.blue };


        ret.Data.ForEach((val, p) =>
        {
            var normP = p.ToVector3() / (Resolution - 1);
            var voxel = Union(
                sphereSdf(normP, sphereA, sphereRadius, material1), // colors not used by renderer
                sphereSdf(normP, sphereB, sphereRadius, material1),
                sphereSdf(normP, sphereC, sphereRadius, material2),
                sphereSdf(normP, sphereD, sphereRadius, material3)
                );
            ret.Data[p] = new VoxelData()
            {
                Density = voxel.Val,
                Material = voxel.Color
            };
        });
    }

    private void fillDataEditTool(UniformVoxelData ret)
    {

        ret.Data.ForEach((v, p) => ret.Data[p] = new VoxelData() { Density = 1000 });// Set empty space

        var scale = (float)(Resolution - 1);

        var sphereRadius = 0.3f * scale;
        var sphereA = new Vector3(0, 0, 0) * scale;
        var sphereB = new Vector3(1, 1, 1) * scale;
        var sphereC = new Vector3(0.5f, 0.6f, 0.4f) * scale;
        var sphereD = new Vector3(0.5f, 0.4f, 0.6f) * scale;
        var sphereE = new Vector3(0.5f, 0.7f, 0.7f) * scale;


        var material1 = new VoxelMaterial() { color = Color.green };
        var material2 = new VoxelMaterial() { color = Color.red };
        var material3 = new VoxelMaterial() { color = Color.blue };


        var tool = new WorldEditTool();
        tool.addSphere(ret, sphereA, sphereRadius, material1);
        tool.addSphere(ret, sphereB, sphereRadius, material1);
        tool.addSphere(ret, sphereC, sphereRadius, material2);
        tool.removeSphere(ret, sphereD, sphereRadius, material3);
        //tool.removeSphere(ret, sphereE, sphereRadius, material1);
    }

    private struct Voxel
    {
        public float Val;
        public VoxelMaterial Color;
        public Voxel(float val, VoxelMaterial color)
        { Val = val; Color = color; }
    }

    private Voxel sphereSdf(Vector3 x, Vector3 center, float size, VoxelMaterial color)
    {
        return new Voxel(SdfFunctions.Sphere(x, center, size), color);
    }
    private Voxel Union(params Voxel[] voxels)
    {
        var ret = voxels[0];
        for (int i = 1; i < voxels.Length; i++)
        {
            if (ret.Val > voxels[i].Val)
                ret = voxels[i];
        }

        return ret;
    }

    public void Update()
    {
        if (Change)
        {
            var time = Time.timeSinceLevelLoad;
            time *= ChangeSpeed / (Max - Min);
            Resolution = (int)Math.Round( Mathf.PingPong(time,Max-Min)+Min);
        }

        var data = createChunkData();

        data.LastChangeFrame = Time.frameCount;
        throw new NotImplementedException();
        //voxelChunkRenderer.SetChunk(data);
        //voxelChunkRenderer.SetWorldcoords(new Point3(0, 0, 0), 1f / (Resolution-1) * Size);

    }


    //private void tryClick()
    //{
    //    var dir = 0;
    //    if (Input.GetKey(KeyCode.F))
    //        dir = 1;
    //    else if (Input.GetKey(KeyCode.R))
    //        dir = -1;
    //    else
    //        return;

    //    //var ray = Camera.main.ScreenPointToRay(new Vector3(0.5f, 0.5f, 0));// new Ray(RayPosition, RayDirection.normalized);
    //    var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
    //    RaycastHit hitInfo;
    //    if (!Physics.Raycast(ray, out hitInfo)) return;

    //    var point = hitInfo.point;

    //    data.ForEach((val, p) =>
    //    {
    //        var dist = (p - point).magnitude;
    //        if (dist > PlacementSize) return;

    //        val += PlacementSpeed * Time.deltaTime * dir;
    //        data[p] = Math.Max(Math.Min(val, 1), -1);
    //    });



    //}


}
