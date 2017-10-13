using System.Collections.Generic;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.Gandalf.Domain
{
    public class City : ICellEntity, IMagicProvider, IMagicChargeDistributor
    {
        public Cell Position { get; private set; }
        public float MagicAreaSize = 3;

        private int Magic = 0;
        private int MaxMagic = 10;
        private int NumInhabitants = 1;

        public City(Cell position, Grid grid)
        {
            this.Position = position;
            position.Register(this);

            for (int x = position.Coordinate.X - 2; x <= position.Coordinate.X + 2; x++)
                for (int y = position.Coordinate.Z - 2; y <= position.Coordinate.Z + 2; y++)
                {
                    if (grid.HasCell(x, y))
                        grid.Get(x, y).MagicProviders.Add(this);
                }


        }

        public IEnumerable<YieldInstruction> Simulate()
        {
            for (; ; )
            {
                yield return new WaitForSeconds(3);
                 var numTaken = Position.Items.Take(ItemType.Wood, NumInhabitants);
                if (numTaken < NumInhabitants)
                    NumInhabitants--;
                if (Position.Items.Get(ItemType.Wood) > 3)
                    NumInhabitants++;
                if (NumInhabitants < 1) NumInhabitants = 1;
                Magic += NumInhabitants * 3;
                if (Magic > MaxMagic) Magic = MaxMagic;
            }
        }

        public string GetCellInfo()
        {
            return "City ";
        }

        public IDictionary<string, string> Values
        {
            get { return new Dictionary<string, string>() { { "Magic", Magic.ToString() }, { "NumInhabitants", NumInhabitants.ToString() } }; }
        }

        public bool HasMagic()
        {
            return Magic > 0;
        }

        public bool TakeMagic(int amount)
        {
            if (Magic < amount) return false;
            Magic -= amount;
            return true;
        }

        bool IMagicChargeDistributor.HasMagic
        {
            get { return true; }
        }

        public bool InRange(Cell cell)
        {
            return (Position.Coordinate - cell.Coordinate).ToVector3().magnitude < MagicAreaSize;
        }
    }
}