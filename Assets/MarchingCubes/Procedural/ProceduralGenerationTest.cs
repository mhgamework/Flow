using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.VoxelEngine.DualContouring.Generation;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.MarchingCubes.Procedural
{
    public class ProceduralGenerationTest : MonoBehaviour
    {
        public float y;
        public int octaves;
        public float min = 0;
        public float max = 1;
        public float clip = 0.5f;
        public bool enableClip = true;
        public float zoom = 1f;
        public int theSize = 64;
        public bool generationEnabled = true;

        public float centerClouds = 64f;
        public float centerDecayStart = 10f;
        public float centerDecayEnd = 30f;
        public float centerRandomize = 10f;
        public float centerRandomizeScale = 1f;

        private Image image;
        private NoiseGenerator n = new NoiseGenerator();
        public float persistence = 0.5f;


        public bool debugRegenerate = false; // ACKY
        public void Start()
        {
            image = GetComponent<Image>();
        }
        public void Update()
        {
            if (!generationEnabled) return;
            Vector2 size = new Vector2(theSize, theSize);
            // Create a new 2x2 texture ARGB32 (32 bit with alpha) and no mipmaps
            var texture = new Texture2D((int)size.x, (int)size.y, TextureFormat.ARGB32, false);

            // set the pixel values




            for (int x = 0; x < size.x; x++)
            {
                for (int z = 0; z < size.y; z++)
                {
                    var val = 0f;

                    val = getTerrainVal( x, z, y);


                    texture.SetPixel(x, z, new Color(val, val, val));

                }
            }

            // Apply all SetPixel calls
            texture.Apply();

            // connect texture to material of GameObject this script is attached to
            image.sprite = Sprite.Create(texture, new Rect(new Vector2(), size), new Vector2());
        }

        public float getTerrainVal( float x, float y, float z)
        {
            var midDistance = Math.Abs(y - centerClouds + centerRandomize * n.CalculatePerlinNoise(new Vector3(x * zoom * centerRandomizeScale, -1324f, z * zoom * centerRandomizeScale)));

            if (midDistance > centerDecayEnd * 1.5f) return 1;
            var val = OctavePerlin(x * zoom, y * zoom, z * zoom, octaves, persistence);
            val = (val + 1) / 2;

            val = Mathf.Clamp(val, min, max);

            // TODO i think i shouldve used y*zoom instead of y for middle thingy
            midDistance = Mathf.Clamp(midDistance, centerDecayStart, centerDecayEnd);
            val = Mathf.Lerp(val, 1, (midDistance - centerDecayStart) / (centerDecayEnd - centerDecayStart));
            if (enableClip)
                val = val > clip ? 1 : 0;
            return val;
        }


        public float OctavePerlin(float x, float y, float z, int octaves, float persistence)
        {
            float total = 0;
            float frequency = 1;
            float amplitude = 1;
            float maxValue = 0;  // Used for normalizing result to 0.0 - 1.0
            for (int i = 0; i < octaves; i++)
            {
                total += n.CalculatePerlinNoise(new Vector3(x * frequency, y * frequency, z * frequency)) * amplitude;

                maxValue += amplitude;

                amplitude *= persistence;
                frequency *= 2;
            }

            return total / maxValue;
        }
    }
}
