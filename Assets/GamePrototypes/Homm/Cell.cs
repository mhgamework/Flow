using System.Collections.Generic;

namespace Assets.Homm
{
    public class Cell
    {
        public bool IsWalkable = true;
        public bool IsOccupied = false;
        public List<ICellInteractable> Interactables = new List<ICellInteractable>();
        public string Description = "";
    }
}