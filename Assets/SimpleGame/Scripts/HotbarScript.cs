using UnityEngine;

namespace Assets.SimpleGame.Scripts
{
    public class HotbarScript : Singleton<HotbarScript>
    {
        public InteractableInventoryUIScript InteractableInventory;
        public Color SelectedColor;
        public int SelectedSlot = 2;

        private int currentSelectedSlot = -1;

        public void Start()
        {


        }
        public void Update()
        {
            if (currentSelectedSlot != SelectedSlot)
            {
                if (currentSelectedSlot != -1)
                    InteractableInventory.GetSlot(currentSelectedSlot).ResetBackgroundColor();
                if (SelectedSlot != -1)
                    InteractableInventory.GetSlot(SelectedSlot).SetBackgroundColor(SelectedColor);
                currentSelectedSlot = SelectedSlot;
            }
        }

        public void SelectNext()
        {
            SelectedSlot = (SelectedSlot + 1) % InteractableInventory.NumSlots;
        }

        public void SelectPrevious()
        {
            SelectedSlot = (SelectedSlot - 1 + InteractableInventory.NumSlots) % InteractableInventory.NumSlots;

        }

        public InventoryScript.InventoryItem GetSelectedInventoryItem()
        {
            return InteractableInventory.TargetInventory.GetSlot(SelectedSlot);
        }
    }
}