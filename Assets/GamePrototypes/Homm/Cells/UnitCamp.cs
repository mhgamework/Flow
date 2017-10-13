using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Homm.UI;
using UnityEngine;

namespace Assets.Homm
{
    public class UnitCamp : MonoBehaviour,ICellInteractable
    {
        public int AvailableUnits = 1;
        public int UnitCrystalPrice = 3;
        public int UnitGrowth = 5;
        public string HoverDescription = "";

        private Grid grid;
        private GameMaster gameMaster;

        public void Start()
        {
            gameMaster = GameMaster.Instance;
        }
        public void Update()
        {
            if (grid == null)
            {
                grid = HommMain.Instance.Grid;
                grid.RegisterInteractable(this, transform.position);
                var cell = grid.pointToCell(transform.position);
                grid.Get(cell).Description = HoverDescription;
            }
        }

        public IEnumerable<YieldInstruction> Interact()
        {
            foreach (var f in BuyUnitsDialog.Instance.ShowCoroutine(this))
                yield return f;

            yield return null;
        }
        public void BuyUnits(int amount)
        {
            if (AvailableUnits < amount) throw new Exception("Not possible");
            if(PlayerState.Instance.GetResourceAmount(ResourceType.Crystals) < amount * UnitCrystalPrice) throw new Exception("Not possible");

            AvailableUnits -= amount;
            PlayerState.Instance.ChangeResourceAmount(ResourceType.Crystals, -amount * UnitCrystalPrice);
            PlayerState.Instance.ChangeResourceAmount(ResourceType.Units, amount);

        }

        public int GetMaxUnitBuyCount()
        {
            return Math.Min(PlayerState.Instance.GetResourceAmount(ResourceType.Crystals) / UnitCrystalPrice,
                AvailableUnits);
        }
    }
}
