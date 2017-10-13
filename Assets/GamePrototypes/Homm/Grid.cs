using DirectX11;
using MHGameWork.TheWizards;
using UnityEngine;

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


        public Point3 pointToCell(Vector3 worldPos)
        {
            return worldPos.ToFloored();
        }

        public Vector3 toCellCenter(Point3 selectedGridcell)
        {
            return selectedGridcell.ToVector3() + new Vector3(0.5f, 0, 0.5f);
        }

        public void RegisterInteractable(ICellInteractable interactable, Vector3 worldPos)
        {
            var cell = pointToCell(worldPos);
            Get(cell).Interactables.Add(interactable);
        }

        public void UnRegisterInteractable(ICellInteractable interactable, Vector3 worldPos)
        {
            var cell = pointToCell(worldPos);
            Get(cell).Interactables.Remove(interactable);
        }
    }
}