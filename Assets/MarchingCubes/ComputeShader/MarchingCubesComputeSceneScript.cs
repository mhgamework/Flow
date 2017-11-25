using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;

namespace Assets.MarchingCubes.ComputeShader
{
    public class Buffers
    {
        public Buffers(ComputeBuffer densityBuffer, ComputeBuffer cellsBuffer, ComputeBuffer countBuffer, ComputeBuffer trianglesBuffer)
        {
            DensityBuffer = densityBuffer;
            CellsBuffer = cellsBuffer;
            CountBuffer = countBuffer;
            TrianglesBuffer = trianglesBuffer;
        }

        public ComputeBuffer DensityBuffer { get; private set; }
        public ComputeBuffer CellsBuffer { get; private set; }
        public ComputeBuffer CountBuffer { get; private set; }
        public ComputeBuffer TrianglesBuffer { get; private set; }
    }

    class Chunk
    {
        public float[] sdf;

        public Buffers buffers;
        public Vector3 offset;
    }

    public class MarchingCubesComputeSceneScript : MonoBehaviour
    {
        public UnityEngine.ComputeShader shader;
        private Mesh mesh;
        private Buffers buffers;
        int configCellSize = 64;

        private List<Chunk> chunks = new List<Chunk>();


        public void Start()
        {
            var filter = GetComponent<MeshFilter>();
            filter.mesh = new Mesh();
            mesh = filter.mesh;
            buffers = createBuffers(configCellSize);

            var repeat = 3;
            var entireSize = configCellSize * repeat;
            var sphereSize = 16;



            for (int z = 0; z < repeat; z++)
                for (int y = 0; y < repeat; y++)
                    for (int x = 0; x < repeat; x++)
                    {
                        var c = new Chunk();

                        var offset = new Vector3(x, y, z) * configCellSize;
                        c.sdf = generateSphereData(configCellSize, new Vector3(1, 1, 1) * entireSize * 0.5f, sphereSize, offset);
                        c.buffers = createBuffers(configCellSize);
                        c.offset = offset;
                        //var triangles = CalculateTriangles(sdf, configCellSize, buffers);
                        //pointList.AddRange(triangles.Select(p => p + offset));

                        chunks.Add(c);
                    }
        }

        private List<Vector3> pointList = new List<Vector3>();
        public void Update()
        {
            PerfTest();

            //pointList.Clear();

            //foreach (var c in chunks)
            //{
               
            //    runCalculateTrianglesShader(shader, c.sdf, configCellSize, c.buffers);
             
            //}
            //foreach (var c in chunks)
            //{
            //    var triangles = retrieveCalculationOutput(c.buffers);
            //    pointList.AddRange(triangles.Select(p => p + c.offset));
            //}

            ////Debug.Log(triangles.Length);

            //mesh.Clear();
            //mesh.SetVertices(pointList.ToList());
            //mesh.SetIndices(pointList.Select((v, k) => k).ToArray(), MeshTopology.Triangles, 0);
            //mesh.RecalculateBounds();
            //mesh.RecalculateNormals();
        }

        void PerfTest()
        {
            var w = new Stopwatch();

            var sdf = generateSphereData(configCellSize, new Vector3(1, 1, 1) * configCellSize * 0.5f, configCellSize*0.4f, new Vector3());
            var buffers  = createBuffers(configCellSize);
            setSdfToBuffer(sdf,buffers);
            runCellGatheringKernel(shader, configCellSize, buffers);
            var data = getCellBufferData(buffers); // Force GPU-CPU sync

            var times = 10;
            w.Start();
            for (int i = 0; i < times; i++)
            {
                buffers.TrianglesBuffer.SetCounterValue(0);
                runTriangleKernel(shader, buffers);
                var triangles = retrieveCalculationOutput(buffers);
            }
            w.Stop();

            Debug.Log(w.ElapsedMilliseconds / (float)times);
        }



        private float[] generateSphereData(int cellSize, Vector3 spherePos, float sphereSize, Vector3 offset)
        {
            var sdf = new float[(cellSize + 1) * (cellSize + 1) * (cellSize + 1)];
            int i = 0;
            for (int z = 0; z < (cellSize + 1); z++)
                for (int y = 0; y < (cellSize + 1); y++)
                    for (int x = 0; x < (cellSize + 1); x++)
                    {
                        sdf[i] = ((new Vector3(x, y, z) + offset) - spherePos).magnitude - sphereSize;
                        //sdf[i] = -1;
                        //if (x == 1 && y == 1 && z == 1)
                        //    sdf[i] = 1;
                        i++;
                    }
            return sdf;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct CellIntermediate
        {
            public int x;
            public int y;
            public int z;
            public int cubeindex;

            public override string ToString()
            {
                return string.Format("X: {0}, Y: {1}, Z: {2}, Cubeindex: {3}", x, y, z, cubeindex);
            }
        };
        [StructLayout(LayoutKind.Sequential)]
        struct Triangle
        {
            public Vector3 a;
            public Vector3 b;
            public Vector3 c;

            public override string ToString()
            {
                return string.Format("A: {0}, B: {1}, C: {2}", a, b, c);
            }
        };

        private Vector3[] calculateTriangles(float[] sdf, int size)
        {
            var buffers = createBuffers(size);
            return CalculateTriangles(shader, sdf, size, buffers);
        }

        private static Buffers createBuffers(int size)
        {
            var densityBuffer = new ComputeBuffer((size + 1) * (size + 1) * (size + 1), 4); //new ComputeBuffer(sdf.Length, 4);
            var cellsBuffer = new ComputeBuffer(size * size * size, 4 * 3 + 4, ComputeBufferType.Append);
            var countBuffer = new ComputeBuffer(3, 4, ComputeBufferType.IndirectArguments);
            countBuffer.SetData(new[] { 0, 1, 1 });
            var trianglesBuffer = new ComputeBuffer((size + 1) * (size + 1) * (size + 1) * 12, 4 * 3 * 3,
                ComputeBufferType.Append);


            var buffers = new Buffers(densityBuffer, cellsBuffer, countBuffer, trianglesBuffer);
            return buffers;
        }

        private static Vector3[] CalculateTriangles(UnityEngine.ComputeShader shader, float[] sdf, int size, Buffers buffers)
        {
            runCalculateTrianglesShader(shader, sdf, size, buffers);


            return retrieveCalculationOutput(buffers);
        }

        private static Vector3[] retrieveCalculationOutput(Buffers buffers)
        {
            ComputeBuffer.CopyCount(buffers.TrianglesBuffer, buffers.CountBuffer, 0);
            var argsData2 = new int[1];

            buffers.CountBuffer.GetData(argsData2);
            var trianglesCount = argsData2[0];

            Triangle[] output = new Triangle[trianglesCount];
            buffers.TrianglesBuffer.GetData(output);

            //foreach (var tp in output) Debug.Log(tp);

            return output.SelectMany(t => new[] { t.a, t.b, t.c }).ToArray();
        }

        private static void runCalculateTrianglesShader(UnityEngine.ComputeShader shader, float[] sdf, int size, Buffers buffers)
        {
            setSdfToBuffer(sdf, buffers);


            runCellGatheringKernel(shader, size, buffers);

            runTriangleKernel(shader, buffers);
        }

        private static CellIntermediate[] getCellBufferData(Buffers buffers)
        {
            ComputeBuffer.CopyCount(buffers.CellsBuffer, buffers.CountBuffer, 0);

            var argsData = new int[3];
            buffers.CountBuffer.GetData(argsData);
            var cellCount = argsData[0];
            
            var temp = new CellIntermediate[cellCount];
            buffers.CellsBuffer.GetData(temp);
            return temp;
        }

        private static void runTriangleKernel(UnityEngine.ComputeShader shader, Buffers buffers)
        {
            ComputeBuffer.CopyCount(buffers.CellsBuffer, buffers.CountBuffer, 0);

            //var argsData = new int[1];
            //buffers.CountBuffer.GetData(argsData);
            //var cellCount = argsData[0];
            //if (cellCount == 0) return new Vector3[0];
            //var temp = new CellIntermediate[cellCount];
            //cellsBuffer.GetData(temp);
            //foreach (var p in temp) Debug.Log(p);
            //buffers.CellsBuffer.SetCounterValue(0);


            buffers.TrianglesBuffer.SetCounterValue(0);


            shader.SetBuffer(shader.FindKernel("CSTriangle"), "densityBuffer", buffers.DensityBuffer);
            shader.SetBuffer(shader.FindKernel("CSTriangle"), "inCellsBuffer", buffers.CellsBuffer);
            shader.SetBuffer(shader.FindKernel("CSTriangle"), "trianglesBuffer", buffers.TrianglesBuffer);
            //shader.Dispatch(shader.FindKernel("CSTriangle"), cellCount, 1, 1);
            shader.DispatchIndirect(shader.FindKernel("CSTriangle"), buffers.CountBuffer); // cellCount, 1, 1);
        }

        private static void runCellGatheringKernel(UnityEngine.ComputeShader shader, int size, Buffers buffers)
        {
            int kernel = shader.FindKernel("CSMain");
            shader.SetBuffer(kernel, "densityBuffer", buffers.DensityBuffer);

            buffers.CellsBuffer.SetCounterValue(0);
            shader.SetBuffer(kernel, "outCellsBuffer", buffers.CellsBuffer);

            shader.Dispatch(kernel, size / 8, size / 8, size / 8);
        }

        private static void setSdfToBuffer(float[] sdf, Buffers buffers)
        {
            buffers.DensityBuffer.SetData(sdf);
        }
    }
}