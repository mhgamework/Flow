using UnityEngine;

namespace Assets.Flow
{
    public class Storage  : MonoBehaviour, IInteractable
    {
        public TextPopup TextPopup;

        public int WaterCrystals;
        public int FireCrystals;
            

        public void Setup()
        {

        }

        public void OnUnfocus(Player p)
        {

        }

        public void OnFocus(Player p)
        {
        }

        public void TryInteract(Player p, Vector3 point)
        {
            TextPopup.SetText("Water: " + WaterCrystals + "\nFire: " + FireCrystals);
           
        }

        public void AddResources(string resourceType, int i)
        {
            if (resourceType == "water")
                WaterCrystals += i;
            else if (resourceType == "fire")
                FireCrystals += i;
        }
    }
}