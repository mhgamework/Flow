using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Gameplay
{
    public class InventoryUIItemScript : MonoBehaviour
    {
        public Text Text;




        public void LinkTo(InventoryScript inventory, string type)
        {
            StartCoroutine(UpdateValue(inventory, type).GetEnumerator());
        }

        private IEnumerable<YieldInstruction> UpdateValue(InventoryScript inventory, string type)
        {
            for (;;)
            {
                Text.text = type + ": " + inventory.Get(type);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}