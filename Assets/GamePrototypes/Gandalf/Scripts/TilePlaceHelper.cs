using Assets.Gandalf.Domain;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.Gandalf.Scripts
{
    public class TilePlaceHelper
    {
        Grid grid;

        public TilePlaceHelper(Grid grid)
        {
            this.grid = grid;
        }

        public Cell GetCell(Transform transform)
        {
            var pos = transform.position;
            pos *= (1f / grid.GridCellSize);
            var point = pos.ToPoint3Rounded();
            return grid.Get(point);
        }

        public void ToTransform(Transform transform, Cell cell)
        {
            transform.position = cell.Coordinate.ToVector3() * grid.GridCellSize;
        }
    }
}