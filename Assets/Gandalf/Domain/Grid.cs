using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Gandalf.Domain;
using DirectX11;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.Gandalf
{
    public class Grid
    {
        public int Size { get; private set; }

        public float GridCellSize { get; private set; }

        //private readonly Cell edge;
        private Cell[] cells;

        public Grid(int size, float gridCellSize)//, Cell edge)
        {
            this.Size = size;
            this.GridCellSize = gridCellSize;
            //this.edge = edge;
            cells = new Cell[size * size];
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i] = new Cell(new Point3(i % Size, 0, i / Size),this);
            }
        }

        public Cell Get(int x, int y)
        {
            if (!HasCell(x, y)) throw new Exception("Out of range " + x + " " + y);//return edge;
            return cells[y * Size + x];
        }
        public Cell Get(Point3 p)
        {
            return Get(p.X, p.Z);
        }

        public bool HasCell(int x, int y)
        {
            return !(x < 0 || y < 0 || x >= Size || y >= Size);
        }


        //public Point3 pointToCell(Vector3 worldPos)
        //{
        //    return worldPos.ToFloored();
        //}

        //public Vector3 toCellCenter(Point3 selectedGridcell)
        //{
        //    return selectedGridcell.ToVector3() + new Vector3(0.5f, 0, 0.5f);
        //}

        //public void RegisterInteractable(ICellInteractable interactable, Vector3 worldPos)
        //{
        //    var cell = pointToCell(worldPos);
        //    Get(cell).Interactables.Add(interactable);
        //}

        //public void UnRegisterInteractable(ICellInteractable interactable, Vector3 worldPos)
        //{
        //    var cell = pointToCell(worldPos);
        //    Get(cell).Interactables.Remove(interactable);
        //}
        //public Vector3 GetWorldPositionFromCell(Cell cell)
        //{
        //    return cell.Coordinate.ToVector3() * GridCellSize;
        //}
        public Cell GetCellContainingWorldPosition(Vector3 pos)
        {
            pos *= (1f / GridCellSize);
            return Get(pos.ToFloored());
        }


        private Point3[] neighbours4 = new Point3[]
        {
            new Point3(-1, 0, 0),
            new Point3(0, 0, 1),
            new Point3(1, 0, 0),
            new Point3(0, 0,-1),
        };

        //private Point3[] points2 = new Point3[]
        //{
        //    new Point3(-1, 0, -1),
        //    new Point3(0, 0, -1),
        //    new Point3(1, 0, -1),
        //    new Point3(-1, 0, 0),
        //    new Point3(1, 0, 0),
        //    new Point3(-1, 0, 1),
        //    new Point3(0, 0, 1),
        //    new Point3(1, 0, 1),
        //};
        public IEnumerable<Cell> GetNeighbours4(Cell cell)
        {
            return neighbours4.Select(k => cell.Coordinate + k).Where(k => HasCell(k.X, k.Z)).Select(k => Get(k));
        }
    }
}