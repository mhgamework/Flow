using UnityEngine;

namespace Assets.MarchingCubes.ProceduralTutorial
{
    public static class Noise
    {
        public static float[,] noise(int size, int octaves, float scale, float persistence, float lacunarity, int seed, Vector2 offset, float sampleResolution = 1)
        {
            var test = new TerrainTest();
            var rand = new System.Random(seed);

            Vector2[] octaveOffsets = new Vector2[octaves];
            for (int i = 0; i < octaves; i++)
            {
                var maxValue = 100; // Reduced this with respect to the 100,000 in demo, because of floating precision problems
                float offsetX = rand.Next(-maxValue, maxValue);
                float offsetY = rand.Next(-maxValue, maxValue);
                octaveOffsets[i] = new Vector2(offsetX, offsetY);// + offset;
            }

            var min = float.MaxValue;
            var max = float.MinValue;

            //float halfSize = size / 2f;


            var map = new float[size, size];
            for (int x = 0; x < size; x++)
            for (int z = 0; z < size; z++)
            {

                float noiseHeight = 0.0f;
                float amplitude = 0.5f;
                float frequency = 1.0f;
                for (int i = 0; i < octaves; i++)
                {
                    //var sampleX = (x - halfSize) / scale * frequency + octaveOffsets[i].x;
                    //var sampleZ = (z - halfSize) / scale * frequency + octaveOffsets[i].y;

                    var sampleX = (((x ) * sampleResolution + offset.x) / scale) * frequency + octaveOffsets[i].x;
                    var sampleZ = (((z ) * sampleResolution + offset.y) / scale) * frequency + octaveOffsets[i].y;

                    float n = Mathf.PerlinNoise(sampleX, sampleZ);//* 2 - 1;
                    //float n = test.noised(new Vector3(sampleX, 0, sampleZ)).x * 2 - 1;

                    noiseHeight += amplitude * n; // accumulate values		
                    amplitude *= persistence; // amplitude decrease

                    frequency *= lacunarity; // frequency increase
                }


                map[x, z] = noiseHeight;
                if (noiseHeight < min)
                    min = noiseHeight;
                if (noiseHeight > max)
                    max = noiseHeight;


            }

            var diff = max - min;
            for (int x = 0; x < size; x++)
            for (int z = 0; z < size; z++)
            {
                //map[x, z] = (map[x, z] - min) / diff;
            }

            return map;
        }

        public static float[,] calibrationNoise(int size,  float scale, Vector2 offset)
        {
            var map = new float[size, size];

            for (int x = 0; x < size; x++)
            for (int z = 0; z < size; z++)
            {
                map[x, z] = new Vector2((x + offset.x) / scale, (z + offset.y) / scale).magnitude;
            }
            return map;

        }

    }
}