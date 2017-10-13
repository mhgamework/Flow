using System.Collections.Generic;
using System.Linq;
using Assets.Reusable;
using UnityEngine;

namespace Assets.Gandalf.Domain
{
    public class CellAStarAlgorithm : EuclidianAStarAlgorithm<Cell>
    {
        public override IEnumerable<Cell> GetNeighbours(Cell current, Cell finalGoal)
        {
            return current.Neighbours4.Where(n => n.IsWalkable);
        }

        protected override Vector3 GetPosition(Cell p)
        {
            return p.CenterPosition;
        }
    }
}