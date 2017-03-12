using UnityEngine;
using System.Collections;
using System;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using DirectX11;
using System.Collections.Generic;
using System.Linq;
using Assets.MarchingCubes;

/// <summary>
/// For testing the marchingcubes renderer
/// </summary>
public class MarchingCubesScript : MonoBehaviour
{
    public int IsoSurface = 40;
    public int Resolution = 30;
    public float OffsetFactor = 1;
    private MeshFilter meshFilter;
    private Mesh mesh;
    Array3D<double> data;
    Array3D<Color> dataMaterials;

    MarchingCubesService s = new MarchingCubesService();

    public Vector3 RayPosition;
    public Vector3 RayDirection;
    public float PlacementSize = 3;
    public float PlacementSpeed = 1;




    // Use this for initialization
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        meshFilter.mesh = mesh;

        data = new Array3D<double>(new Point3(Resolution, Resolution, Resolution));
        dataMaterials = new Array3D<Color>(new Point3(Resolution, Resolution, Resolution));

        var sphereRadius = 0.3f;
        var sphereA = new Vector3(0, 0, 0);
        var sphereB = new Vector3(1, 1, 1);
        var sphereC = new Vector3(0.5f, 0.6f, 0.4f);
        var sphereD = new Vector3(0.5f, 0.4f, 0.6f);


        data.ForEach((val, p) =>
        {
            var normP = p.ToVector3() / (Resolution - 1);
            var voxel = Union(
                sphereSdf(normP, sphereA, sphereRadius,Color.green), // colors not used by renderer
                sphereSdf(normP, sphereB, sphereRadius, Color.green),
                sphereSdf(normP, sphereC, sphereRadius, Color.red),
                sphereSdf(normP, sphereD, sphereRadius, Color.blue)
                );
            data[p] = voxel.Val;
            dataMaterials[p] = voxel.Color;

            //+ sphereSdf(normP, sphereB, sphereRadius);
        });

    }
    private struct Voxel
    {
        public float Val;
        public Color Color;
        public Voxel(float val, Color color)
        { Val = val; Color = color; }
    }

    private Voxel sphereSdf(Vector3 x, Vector3 center, float size, Color color)
    {
        return new Voxel((x - center).magnitude - size, color);
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

    Point3[] Vertices = new Point3[] { new Point3(0, 0, 1), new Point3(1, 0, 1), new Point3(1, 0, 0), new Point3(0, 0, 0), new Point3(0, 1, 1), new Point3(1, 1, 1), new Point3(1, 1, 0), new Point3(0, 1, 0) };


    // Update is called once per frame
    void Update()
    {
        tryClick();
        updateMesh();

    }

    private void tryClick()
    {
        var dir = 0;
        if (Input.GetKey(KeyCode.F))
            dir = 1;
        else if (Input.GetKey(KeyCode.R))
            dir = -1;
        else
            return;

        //var ray = Camera.main.ScreenPointToRay(new Vector3(0.5f, 0.5f, 0));// new Ray(RayPosition, RayDirection.normalized);
        var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hitInfo;
        if (!Physics.Raycast(ray, out hitInfo)) return;

        var point = hitInfo.point;

        data.ForEach((val, p) =>
        {
            var dist = (p - point).magnitude;
            if (dist > PlacementSize) return;

            val += PlacementSpeed * Time.deltaTime * dir;
            data[p] = Math.Max(Math.Min(val, 1), -1);
        });



    }
    private bool updated = false;
    private void updateMesh()
    {
        if (updated) return;
        updated = true;
        var triangles = new List<int>();
        var materials = new List<Color>();
        var vertices = new List<Vector3>();

        var gridvals = new double[8];
        var matvals = new Color[8];

        var size = new Point3(1, 1, 1) * Resolution;

        var maxX = data.Size.X - 1;//size.X - 2;
        var maxY = data.Size.Y - 1;//size.Y - 2;
        var maxZ = data.Size.Z - 1;//size.Z - 2;


        var offset = new Vector3(Mathf.Sin(Time.realtimeSinceStartup + 2), Mathf.Cos(Time.realtimeSinceStartup + 3), Mathf.Sin(Time.realtimeSinceStartup + 7)) * Resolution * OffsetFactor;

        var actualIsoSurface = IsoSurface * 0.01f * Resolution / 30f;

        var points = Vertices.Select(v => v.ToVector3() * 0.99f).ToArray();

        //var individualColors = new[] { Color.red, Color.blue, Color.green };
        //// Voxelize per color
        //foreach(var iColor in individualColors)
        for (int x = 0; x < maxX; x++)
            for (int y = 0; y < maxY; y++)
                for (int z = 0; z < maxZ; z++)
                {
                    var p = new Point3(x, y, z);
                    for (int i = 0; i < 8; i++)
                    {
                        //var pos = Vertices[i] + p;
                        //var diff = pos - (size.ToVector3() * 0.5f + offset*Mathf.Abs(Mathf.Sin(Time.realtimeSinceStartup)));
                        //var val = diff.magnitude;
                        //val =Mathf.Min(val, (pos - (size.ToVector3() * 0.5f - offset * Mathf.Abs(Mathf.Sin(Time.realtimeSinceStartup)))).magnitude);
                        //cell.val[i] =val; //data.Get(Vertices[i] + p);
                        gridvals[i] = data.Get(Vertices[i] + p);
                        matvals[i] = dataMaterials.Get(Vertices[i] + p);

                    }
                    //var outTriangles = new List<TRIANGLE>();

                    Color outColor;
                    //s.Polygonise(gridvals, points, actualIsoSurface, vertices, p);
                    s.Polygonise(gridvals, matvals, points, 0, vertices, p, materials);
                    //outTriangles.ForEach(t =>
                    //{
                    //    vertices.Add(t.p[0]);
                    //    vertices.Add(t.p[2]); // Invert culling
                    //    vertices.Add(t.p[1]);
                    //});
                }



        mesh.Clear();

        //mesh.SetVertices(vertices.Select(v => v / Resolution - new Vector3(1, 1, 1) * 0.5f).ToList());
        // Double the vertices to include backfaces!!
        var doubledVertices = vertices.Concat(vertices).ToList();
        mesh.SetVertices(doubledVertices);

        var outMaterials = new List<Color>();
        var index = 0;
        var groups = materials.Select((c, i) => new { mat = c, index = i * 3 }).GroupBy(f => f.mat);
        mesh.subMeshCount = groups.Count();
        foreach (var matPair in groups)
        {
            var color = matPair.Key;
            outMaterials.Add(color);
            // Also adds the backface for easy debugging
            var indices = matPair.SelectMany(f => new[] { f.index, f.index + 1, f.index + 2, vertices.Count + f.index, vertices.Count + f.index + 2, vertices.Count + f.index + 1 }).ToArray();
            mesh.SetIndices(indices, MeshTopology.Triangles, index);
            index++;
        }

        // NOTE MATERIALS ARE SET FROM unity editor and do not respect the colors!!

        //mesh.SetIndices(vertices.Select((v, i) => i).ToArray(), MeshTopology.Triangles, 0);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        GetComponent<MeshCollider>().sharedMesh = mesh;
        //transform.localScale = new Vector3(1,1,1)* 1f / Resolution;
    }
}
