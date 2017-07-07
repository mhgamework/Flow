using System.Collections.Generic;
using UnityEngine;

namespace Assets.Homm
{
    public class MineCell : MonoBehaviour, ICellInteractable
    {
        private Grid grid;
        private GameMaster gameMaster;

        public bool claimed = false;

        public void Start()
        {
            gameMaster = GameMaster.Instance;
        }
        public void Update()
        {
            if (grid == null)
            {
                grid = HommMain.Instance.Grid;
                grid.RegisterInteractable(this,transform.position);
                var cell = grid.pointToCell(transform.position);
                grid.Get(cell).Description = "Visit Mine " + (claimed ? "- Claimed" : "- Unclaimed");

            }
        }

        public IEnumerable<YieldInstruction> Interact()
        {
            claimed = true;
            yield return null;
        }
    }
}