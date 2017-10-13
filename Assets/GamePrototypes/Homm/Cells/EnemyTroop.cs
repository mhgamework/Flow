using System;
using System.Collections.Generic;
using Assets.Homm.UI;
using UnityEngine;

namespace Assets.Homm
{
    public class EnemyTroop : MonoBehaviour, ICellInteractable
    {
        public int UnitAmount = 1;
        private Grid grid;


        public void Update()
        {

            if (grid == null)
            {
                grid = HommMain.Instance.Grid;
                var cell = grid.pointToCell(transform.position);

                grid.Get(cell).IsWalkable = true;
                grid.Get(cell).IsOccupied = true;
                grid.RegisterInteractable(this, transform.position);
            }
             
            //enabled = false;

            grid.Get(grid.pointToCell(transform.position)).Description = "Attack enemy troop, " + UnitAmount + " units.";
        }


        public IEnumerable<YieldInstruction> Interact()
        {



            var enemyCount = UnitAmount;
            var heroCount = PlayerState.Instance.GetResourceAmount(ResourceType.Units);


            var heroWins = heroCount > enemyCount;

            var winnerCount = heroWins ? heroCount : enemyCount;
            var loserCount = heroWins ? enemyCount : heroCount;


            var unitLoss = (int)(((float)loserCount / winnerCount) * loserCount);


            if (heroWins)
            {
                PlayerState.Instance.ChangeResourceAmount(ResourceType.Units, -unitLoss);
                foreach (var f in DialogWindow.Instance.ShowCoroutine(
                    "You win the fight. \nThe enemies loses all units. \n You lose " + unitLoss + " units",
                    ""))
                    yield return f;

                var cell = grid.pointToCell(transform.position);

                grid.Get(cell).IsWalkable = true;
                grid.Get(cell).IsOccupied = false;
                grid.Get(cell).Description = "";

                //PlayerState.Instance.ChangeResourceAmount(Type, Amount);
                grid.UnRegisterInteractable(this, transform.position);
                Destroy(gameObject);
            }
            else
            {
                UnitAmount -= unitLoss;
                foreach (var f in DialogWindow.Instance.ShowCoroutine(
                    "You lose the fight!!! \nThe enemies loses " + unitLoss + " units.",
                    ""))
                    yield return f;

                PlayerState.Instance.ChangeResourceAmount(ResourceType.Units, -heroCount);

            }


          

            yield return null;
        }
    }
}