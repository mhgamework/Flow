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

            }
        }

        public IEnumerable<YieldInstruction> Interact()
        {
            claimed = true;
            yield return null;
        }
    }
}