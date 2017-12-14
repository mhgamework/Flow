﻿using DirectX11;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;

namespace Assets.MarchingCubes.VoxelWorldMVP
{
    public class VoxelChunkMeshGenerator : IVoxelMeshGenerator
    {
        public VoxelChunkMeshGenerator(MarchingCubesService s)
        {
            this.s = s;
        }

        Point3[] Vertices = new Point3[] { new Point3(0, 0, 1), new Point3(1, 0, 1), new Point3(1, 0, 0), new Point3(0, 0, 0), new Point3(0, 1, 1), new Point3(1, 1, 1), new Point3(1, 1, 0), new Point3(0, 1, 0) };
        private MarchingCubesService s;

        public VoxelMeshData GenerateMeshFromVoxelData(Array3D<VoxelData> data)
        {
            List<Vector3> verts;
            int numMeshes;
            List<int[]> indicesList;
            List<Color> color;
            List<Color> vertexColors;
            generateMeshVertexColors(data, out verts, out numMeshes, out indicesList, out color, out vertexColors);

            return new VoxelMeshData
            {
                colors = color,
                vertices = verts,
                indicesList = indicesList,
                numMeshes = numMeshes,
                vertexColors = vertexColors
            };
        }
        public void generateMeshSeparateMaterials(Array3D<VoxelData> data, out List<Vector3> doubledVertices, out int numMeshes, out List<int[]> indicesList, out List<Color> colors)
        {
            var triangles = new List<int>();
            var materials = new List<Color>();
            var vertices = new List<Vector3>();

            var vertexColors = new List<Color>();

            var gridvals = new double[8];
            var matvals = new Color[8];

            var maxX = data.Size.X - 1;//size.X - 2;
            var maxY = data.Size.Y - 1;//size.Y - 2;
            var maxZ = data.Size.Z - 1;//size.Z - 2;


            var actualIsoSurface = 0;

            var points = Vertices.Select(v => v.ToVector3()).ToArray(); // *0.99f to show edges :)

            //var individualColors = new[] { Color.red, Color.green, Color.blue };
            var firstColor = new Color();
            var firstColorFound = false;
            var theCurrentColor = new Color();
            var isMultiColor = false;

            var cellColorList = new List<Color>();

            // Voxelize per color-
            //foreach (var iColor in individualColors)
            for (int x = 0; x < maxX; x++)
                for (int y = 0; y < maxY; y++)
                    for (int z = 0; z < maxZ; z++)
                    {
                        isMultiColor = false;
                        firstColorFound = false;

                        var p = new Point3(x, y, z);
                        // STart at -1, the first run is an extra run to check if the runs per color are neccessary
                        for (int iColor = -1; iColor < cellColorList.Count; iColor++)
                        {
                            if (iColor == 0)
                            {
                                // Start of multi color algorithm
                                // Sort the list first, so we can filter duplicates
                                cellColorList.Sort((a, b) => a.GetHashCode() - b.GetHashCode()); //TODO could cause duplicate hash hits?, better just convert color to int somehow  
                            }
                            if (iColor > -1)
                            {
                                var nextColor = cellColorList[iColor];
                                if (nextColor == theCurrentColor)
                                    continue; // Color already handled (should be duplicate)

                                theCurrentColor = cellColorList[iColor];
                            }
                            if (iColor > -1 && theCurrentColor == firstColor)
                                continue; // skip the first color, it is already visited

                            for (int i = 0; i < 8; i++)
                            {
                                var thePos = Vertices[i] + p;
                                var val = data.GetFast(thePos.X, thePos.Y, thePos.Z).Density;
                                var material = data.GetFast(thePos.X, thePos.Y, thePos.Z).Material;
                                var mat = material != null ? material.color : new Color();
                                if (!firstColorFound)
                                {
                                    firstColorFound = true;
                                    firstColor = mat;
                                    theCurrentColor = firstColor;
                                }
                                if (mat != theCurrentColor)
                                {

                                    if (iColor == -1) // Only build color list on first run
                                    {
                                        if (!isMultiColor)
                                        {
                                            // First indication of multicolor, start adding colors
                                            cellColorList.Clear();
                                        }
                                        cellColorList.Add(mat);
                                    }
                                    if (val > 0)
                                    {
                                        isMultiColor = true; // Maybe add if ( val < 0 ), because it is also possible that a voxel has a other material but is air?
                                        val = Math.Max(val, -val); // Make air by mirroring around the isosurface level, should remain identical?
                                    }

                                }

                                gridvals[i] = val;
                                // TODO: use thecurrentcolor here, since we are mimicking single color kernels, 
                                // but not sure it matters i think matval is not used perhaps
                                matvals[i] = mat;
                            }
                            Color outColor;
                            s.Polygonise(gridvals, matvals, points, 0, vertices, p, materials, vertexColors); // Vertex colors not used

                            if (!isMultiColor) break; // default case, speedup!
                        }

                    }







            // Double the vertices to include backfaces!!
            vertexColors.AddRange(vertexColors);// double them also
            doubledVertices = vertices.Concat(vertices).ToList();
            var outMaterials = new List<Color>();
            var groups = materials.Select((c, i) => new { mat = c, index = i * 3 }).GroupBy(f => f.mat);
            //.OrderBy(g => g.Key.a + 255 * (g.Key.r + 255 * (g.Key.g + 255 * g.Key.b)));//.ToArray();
            numMeshes = groups.Count();
            indicesList = new List<int[]>(numMeshes);
            colors = new List<Color>(numMeshes);
            foreach (var matPair in groups)
            {
                var color = matPair.Key;
                outMaterials.Add(color);
                // Also adds the backface for easy debugging
                var indices = matPair.SelectMany(f => new[] { f.index, f.index + 1, f.index + 2, vertices.Count + f.index, vertices.Count + f.index + 2, vertices.Count + f.index + 1 }).ToArray();

                indicesList.Add(indices);
                colors.Add(color);
            }
        }

        public void generateMeshVertexColors(Array3D<VoxelData> data, out List<Vector3> vertices, out int numMeshes, out List<int[]> indicesList, out List<Color> colors, out List<Color> vertexColors)
        {
            var materials = new List<Color>();
            vertices = new List<Vector3>();

            vertexColors = new List<Color>();

            var gridvals = new double[8];
            var matvals = new Color[8];

            var maxX = data.Size.X - 1;//size.X - 2;
            var maxY = data.Size.Y - 1;//size.Y - 2;
            var maxZ = data.Size.Z - 1;//size.Z - 2;


            var actualIsoSurface = 0;

            var points = Vertices.Select(v => v.ToVector3()).ToArray(); // *0.99f to show edges :)

            // Voxelize per color-
            //foreach (var iColor in individualColors)
            for (int x = 0; x < maxX; x++)
                for (int y = 0; y < maxY; y++)
                    for (int z = 0; z < maxZ; z++)
                    {
                        var p = new Point3(x, y, z);
                        for (int i = 0; i < 8; i++)
                        {
                            var thePos = Vertices[i] + p;
                            var val = data.GetFast(thePos.X, thePos.Y, thePos.Z).Density;
                            var material = data.GetFast(thePos.X, thePos.Y, thePos.Z).Material;
                            var mat = material != null ? material.color : new Color();

                            gridvals[i] = val;
                            matvals[i] = mat;
                        }
                        s.Polygonise(gridvals, matvals, points, 0, vertices, p, materials, vertexColors);


                    }





            indicesList = new List<int[]>(1);

            var indices = new int[vertices.Count * 2];

            var iIndex = 0;
            int iV = 0;
            for (; iV < vertices.Count; iV += 3)
            {
                indices[iIndex++] = iV + 0;
                indices[iIndex++] = iV + 1;
                indices[iIndex++] = iV + 2;
            }
            for (; iV < vertices.Count*2; iV += 3)
            {
                indices[iIndex++] = iV + 0;
                indices[iIndex++] = iV + 2;
                indices[iIndex++] = iV + 1;
            }


            indicesList.Add(indices);


            // Double the verts so normals can be correctly calculated
            vertices.AddRange(vertices);
            vertexColors.AddRange(vertexColors);
            
            numMeshes = 1;
            colors = null;//
        
        }

    }
}
