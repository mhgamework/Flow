using System.Collections.Generic;
using DirectX11;
using MHGameWork.TheWizards;
using UnityEngine;

namespace Assets.Flow
{
    public class MagicGrid : Singleton<MagicGrid>
    {
        public float CellSize;
        private Dictionary<Point3, MagicGridCell> cells = new Dictionary<Point3, MagicGridCell>();

        private MagicGridCell nullCell = new MagicGridCell();

        /// <summary>
        /// Warning DO NOT MUTATE return value
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public MagicGridCell GetCellReadOnly(Point3 p)
        {
            MagicGridCell val;
            if (cells.TryGetValue(p, out val)) return val;
            return nullCell;
        }
        public MagicGridCell GetCell(Point3 p)
        {
            MagicGridCell val;
            if (cells.TryGetValue(p, out val)) return val;
            var cell = new MagicGridCell();
            cells[p] = cell;

            return cell;
        }


        /// <summary>
        /// Uses tofloored
        /// </summary>
        /// <param name="transformPosition"></param>
        /// <returns></returns>
        public MagicGridCell GetCellFromWorldPos(Vector3 transformPosition)
        {
            return GetCell((transformPosition * (1f / CellSize)).ToFloored());
        }
    }
}