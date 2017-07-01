using DirectX11;

namespace Assets.Homm
{
    public class Grid
    {
        private readonly int size;
        private readonly Cell edge;
        private Cell[] cells;

        public Grid(int size, Cell edge)
        {
            this.size = size;
            this.edge = edge;
            cells = new Cell[size * size];
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i] = new Cell();
            }
        }

        public Cell Get(int x, int y)
        {
            if (x < 0 || y < 0 || x >= size || y >= size) return edge;
            return cells[y * size + x];
        }
        public Cell Get(Point3 p)
        {
            return Get(p.X, p.Z);
        }
    }
}