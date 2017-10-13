using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Gandalf.Scripts;
using DirectX11;
using UnityEngine;

namespace Assets.Gandalf.Domain
{
    public class Cell : IUISelectable
    {
        private readonly Grid grid;
        public bool IsExplored { get; private set; }
        public ICellEntity Entity { get; private set; }
        public ItemCollection Items { get; private set; }
        public IDictionary<string, string> Values { get { return Entity == null ? new Dictionary<string, string>(): Entity.Values; } }
        public string Name { get { return NewEntity == null ? "Empty" : NewEntity.Name; } }
        public IEnumerable<string> Actions { get { return Enumerable.Empty<string>(); } }
        public void DoAction(string action)
        {
            throw new NotImplementedException();
        }

        public List<IMagicProvider> MagicProviders { get; private set; }

        public bool HasMagic()
        {
            return MagicProviders.Any(k => k.HasMagic());
        }

        public Cell(Point3 coordinate, Grid grid)
        {
            this.grid = grid;
            Coordinate = coordinate;
            Items = new ItemCollection();
            MagicProviders = new List<IMagicProvider>();
        }

        public CellEntityScript NewEntity;

        public bool TakeMagic(int amount)
        {
            foreach (var p in MagicProviders)
                if (p.TakeMagic(amount)) return true;

            return false;
        }

        public Point3 Coordinate { get; private set; }
        public IEnumerable<Cell> Neighbours4 { get { return grid.GetNeighbours4(this); } }

        public Vector3 CenterPosition
        {
            get { return (Coordinate + new Vector3(0.5f, 0, 0.5f)) * grid.GridCellSize; }
        }

        public bool IsWalkable { get{ return NewEntity == null ? true : NewEntity.IsWalkable; }}


        public void MarkExplored()
        {
            IsExplored = true;
        }

        public IEnumerable<Cell> CalculatePath(Cell destination)
        {
            var f = new CellAStarAlgorithm();
            return f.CalculatePath(this, destination);
        }


        public string GetCellInfoString()
        {
            return Entity.GetCellInfo() + "\n" + Items.ToItemString();
        }
        public void Register(ICellEntity o)
        {
            Entity = o;
            //things.Add(o);
        }

        public void UnRegister(ICellEntity o)
        {
            throw new NotImplementedException();
            //things.Remove(o);
        }

     
    }
}