using Assets.MarchingCubes.VoxelWorldMVP;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.MarchingCubes.Rendering
{
    public class WorldDisplay : Singleton<WorldDisplay>
    {
        public Renderer textureRenderer;
        public MeshFilter meshFilter;
        //public MeshRenderer meshRenderer;

        public void DrawTexture(Texture2D texture)
        {
            textureRenderer.sharedMaterial.mainTexture = texture;
            textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
        }


        public static Texture2D TextureFromHeightMap(float[,] heightMap)
        {
            var sizeX = heightMap.GetLength(0);
            var sizeY = heightMap.GetLength(1);
            Texture2D texture = new Texture2D(sizeX, sizeY);

            Color[] colors = new Color[sizeX * sizeY];
            for (int x = 0; x < sizeX; x++)
                for (int z = 0; z < sizeY; z++)
                {
                    colors[x + sizeX * z] = Color.Lerp(Color.black, Color.white, heightMap[x, z]);
                }

            return TextureFromColorMap(colors, sizeX, sizeY);
        }

        public static Texture2D TextureFromColorMap(Color[] colorMap, int width, int height)
        {
            Texture2D texture = new Texture2D(width, height);

            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.SetPixels(colorMap);
            texture.Apply();
            return texture;

        }

        public void DrawMesh(VoxelMeshData meshData, Vector3 scale, float MeshSizeXZ)
        {
            meshFilter.sharedMesh = meshData.CreateMesh();
            meshFilter.transform.localScale = scale;

            meshFilter.transform.localPosition = (-scale.TakeXZ() * MeshSizeXZ / 2f).ToXZ();

        }
    }
}