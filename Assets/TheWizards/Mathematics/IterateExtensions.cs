using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MHGameWork.TheWizards
{
    public static class IterateExtensions
    {
        /// <summary>
        /// Iterates over all cells within boundingbox bb. The ints passed to the action are the cellnumbers, wil 0,0,0 being the cell
        /// from 0,0,0 to cellSize,cellSize,cellSize
        /// 
        /// NOTE: Convert to a fold?
        /// </summary>
        /// <param name="bb"></param>
        /// <param name="cellSize"></param>
        /// <param name="action"></param>
        public static void IterateCells(this Bounds bb, int cellSize, Action<int, int, int> action)
        {
            bb.min = bb.min / cellSize;
            bb.max = bb.max / cellSize;

            var min = new int[3];
            var max = new int[3];
            for (int i = 0; i < 3; i++)
            {
                min[i] = (int)Math.Floor(bb.min[i]);
                max[i] = (int)Math.Ceiling(bb.max[i]);
            }
            for (int x = min[0]; x < max[0]; x++)
                for (int y = min[1]; y < max[1]; y++)
                    for (int z = min[2]; z < max[2]; z++)
                    {
                        action(x, y, z);
                    }
        }
    }
}
