namespace Assets.Homm
{
    public class Grid
    {
        private readonly int size;
        private Cell[] cells;

        public Grid( int size)
        {
            this.size = size;
            cells = new Cell[size * size];
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i] = new Cell();
            }
        }

        public Cell Get(int x, int y)
        {
            return cells[y * size + x];
        }

    }
}