using System.Collections.Generic;
using DirectX11;
using UnityEngine;

namespace Assets.Gandalf.Domain
{
    public class Cell
    {
        private readonly Grid grid;

        public Cell(Point3 coordinate,Grid grid)
        {
            this.grid = grid;
            Coordinate = coordinate;
        }

        public Point3 Coordinate { get; private set; }
        public IEnumerable<Cell> Neighbours4 { get { return grid.GetNeighbours4(this); } }

        public Vector3 CenterPosition
        {
            get { return (Coordinate + new Vector3(0.5f, 0, 0.5f)) * grid.GridCellSize; }
        }

        public IEnumerable<T> Get<T>()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Cell> CalculatePath(Cell destination)
        {
            var f = new CellAStarAlgorithm();
            return f.CalculatePath(this, destination);
        }
    }
}