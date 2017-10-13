using System.Linq;
using Assets.Gandalf.Domain;
using UnityEngine;

namespace Assets.Gandalf.Scripts
{
    public class UIControllerScript : Singleton<UIControllerScript>
    {
        private Wizard wizard;

        public IUISelectable Selected { get; private set; }

        public void Start()
        {
            wizard = GandalfDIScript.Instance.Get<Wizard>();
        }
        public void Update()
        {
           
          
        }

        public enum InputState
        {
            None,
            GoblinPlacement,
        }

        public InputState State { get; private set; }

        public void OnStartWalk()
        {
            State = InputState.None;
        }

        public void OnStartGoblinPlacement()
        {
            State = InputState.GoblinPlacement;
        }

        public void OnGridLeftClick(Cell cell)
        {
            if (State == InputState. None)
            {
                if (wizard.GetMovementOptions().Contains(cell))
                {
                    wizard.MoveTo(cell);
                }
            }
            else if (State == InputState.GoblinPlacement)
            {
                wizard.SetGoblin(wizard.CurrentCell, cell);
                State = InputState.None;
            }
        }
        public void OnGridRightClick(Cell cell)
        {
            Selected = cell;
        }

        public void OnGoblinRightClick(GoblinScript goblinScript)
        {
            Selected = goblinScript.Goblin;
        }

        public void OnGoblinRemoved(Goblin goblin)
        {
            if (goblin == Selected) Selected = null;
        }
        public void OnSpawnExtenderClick()
        {
            wizard.CreateMagicExtender();
        }
    }
}