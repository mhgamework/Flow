using UnityEngine;
using System.Collections;
using System;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using DirectX11;
using System.Collections.Generic;
using System.Linq;
using Assets.MarchingCubes;

public class MarchingCubesScript : MonoBehaviour
{
    public int IsoSurface = 40;
    public int Resolution = 30;
    public float OffsetFactor = 1;
    private MeshFilter meshFilter;
    private Mesh mesh;
    Array3D<double> data;

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
        

        data = new Array3D<double>(new DirectX11.Point3(Resolution, Resolution, Resolution));


        data.ForEach((val, p) => data[p] = (p.ToVector3() - data.Size.ToVector3() * 0.5f).magnitude- data.Size.X/3.0);
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

            val += PlacementSpeed * Time.deltaTime* dir;
            data[p] =Math.Max(Math.Min(val, 1), -1);
        });



    }

    private void updateMesh()
    {
        var triangles = new List<int>();
        var vertices = new List<Vector3>();

        var gridvals = new double[8];

        var size = new Point3(1, 1, 1) * Resolution;

        var maxX = size.X - 2;
        var maxY = size.Y - 2;
        var maxZ = size.Z - 2;


        var offset = new Vector3(Mathf.Sin(Time.realtimeSinceStartup + 2), Mathf.Cos(Time.realtimeSinceStartup + 3), Mathf.Sin(Time.realtimeSinceStartup + 7)) * Resolution * OffsetFactor;

        var actualIsoSurface = IsoSurface * 0.01f * Resolution / 30f;

        var points = Vertices.Select(v => v.ToVector3()).ToArray();

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
                    }
                    //var outTriangles = new List<TRIANGLE>();

                    //s.Polygonise(gridvals, points, actualIsoSurface, vertices, p);
                    s.Polygonise(gridvals, points, 0, vertices, p);
                    //outTriangles.ForEach(t =>
                    //{
                    //    vertices.Add(t.p[0]);
                    //    vertices.Add(t.p[2]); // Invert culling
                    //    vertices.Add(t.p[1]);
                    //});
                }
        mesh.Clear();
        //mesh.SetVertices(vertices.Select(v => v / Resolution - new Vector3(1, 1, 1) * 0.5f).ToList());
        mesh.SetVertices(vertices.ToList());
        mesh.SetIndices(vertices.Select((v, i) => i).ToArray(), MeshTopology.Triangles, 0);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        GetComponent<MeshCollider>().sharedMesh = mesh;
        //transform.localScale = new Vector3(1,1,1)* 1f / Resolution;
    }
}
