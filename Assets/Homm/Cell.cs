using System.Collections.Generic;

namespace Assets.Homm
{
    public class Cell
    {
        public bool IsWalkable = true;
        public List<ICellInteractable> Interactables = new List<ICellInteractable>();
    }
}