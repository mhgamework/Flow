using System.Collections.Generic;
using Assets.Homm.UI;
using UnityEngine;

namespace Assets.Homm
{
    public class ResourceCell : MonoBehaviour, ICellInteractable
    {
        public string HoverDescription = "";
        public string Description;

        private Grid grid;
        private GameMaster gameMaster;

        public ResourceType Type;
        public int Amount;

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
                grid.Get(cell).Description = HoverDescription;
            }
        }

        public IEnumerable<YieldInstruction> Interact()
        {
            foreach (var f in DialogWindow.Instance.ShowCoroutine(Description, Amount + " " + Type))
                yield return f;

            PlayerState.Instance.ChangeResourceAmount(Type, Amount);
            grid.UnRegisterInteractable(this, transform.position);
            Destroy(gameObject);

            yield return null;
        }
    }
}