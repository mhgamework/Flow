using MHGameWork.TheWizards.VoxelEngine.DualContouring.Generation;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.MarchingCubes.Procedural
{
    public class ProceduralVoronoiGenerationTest : MonoBehaviour
    {
        public float zoom = 1f;
        public int theSize = 64;
        public bool generationEnabled = true;


        public float cellSize = 10f;

        private Image image;
        private NoiseGenerator n = new NoiseGenerator();

        public bool regenerate = true;
        [Range(0, 10)]
        public int numSmooths = 1;

        private Cell[] cells;
        [Range(0, 100000)]
        public int Seed;

        private Random random;

        [Range(0,1)]
        public float blendFactor;

        private struct Cell
        {
            public Vector2 Center;
            public Color Color;
        }

        public void Start()
        {
            image = GetComponent<Image>();
        }
        public void Update()
        {
            if (!generationEnabled) return;


            var numCells = (int)(theSize / cellSize) + 2 + 2;

            if (cells == null || cells.Length != numCells * numCells * numCells)
            {
                cells = new Cell[numCells * numCells * numCells];
            }
            //if (regenerate)
            {
                Random.InitState(Seed);

                regenerate = false;
                Vector2 size = new Vector2(theSize, theSize);
                // Create a new 2x2 texture ARGB32 (32 bit with alpha) and no mipmaps
                var texture = new Texture2D((int)size.x, (int)size.y, TextureFormat.ARGB32, false);

                for (int x = -2; x < numCells; x++)
                    for (int y = -2; y < numCells; y++)
                    {
                        cells[coord(x, y, numCells)] = new Cell()
                        {
                            Center = (new Vector2(x, y) + new Vector2(Random.value, Random.value)) * cellSize,
                            Color = new Color(Random.value, Random.value, Random.value, 1)
                        };

                    }
                for (int i = 0; i < numSmooths; i++)
                {
                    var swap = new Cell[numCells * numCells * numCells];

                    for (int x = 0; x < numCells; x++)
                        for (int y = 0; y < numCells; y++)
                        {
                            var sum = new Vector2();


                            for (int ix = -1; ix <= 1; ix++)
                                for (int iy = -1; iy <= 1; iy++)
                                {
                                    sum += cells[coord(x + ix, y + iy, numCells)].Center;
                                }

                            sum /= 3 * 3;
                            var theC = coord(x, y, numCells);
                            var val = cells[theC];
                            val.Center = val.Center*blendFactor + sum * (1-blendFactor);
                            swap[theC] = val;
                        }
                    cells = swap;
                }

                for (int x = 0; x < size.x; x++)
                    for (int y = 0; y < size.y; y++)
                    {
                        var dist = float.MaxValue;
                        var color = new Color();

                        var cellX = (int)(x / cellSize);
                        var cellY = (int)(y / cellSize);


                        for (int ix = -2; ix <= 2; ix++)
                            for (int iy = -2; iy <= 2; iy++)
                            {
                                var iCellX = cellX + ix;
                                var iCellY = cellY + iy;
                                var cell = cells[coord(iCellX, iCellY, numCells)];
                                var newDist = (cell.Center - new Vector2(x, y)).magnitude;
                                if (dist > newDist)
                                {
                                    dist = newDist;
                                    color = cell.Color;
                                }
                            }

                        if (Vector2.Distance(cells[coord(cellX, cellY, numCells)].Center, new Vector2(x, y)) < 1)
                            color = Color.red;

                        texture.SetPixel(x, y, color);

                    }
                // Apply all SetPixel calls
                texture.Apply();

                // connect texture to material of GameObject this script is attached to
                image.sprite = Sprite.Create(texture, new Rect(new Vector2(), size), new Vector2());
            }





        }

        public int coord(int x, int y, int size)
        {
            x += 2;
            y += 2;
            return x + y * size;
        }

    }
}
