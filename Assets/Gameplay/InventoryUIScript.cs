using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Gameplay
{
    public class InventoryUIScript : MonoBehaviour
    {
        public List<string> ItemTypes;
        public InventoryUIItemScript ItemPrefab;

        public InventoryScript Inventory;

        public void Start()
        {
            ItemPrefab.gameObject.SetActive(false);
            foreach (var t in ItemTypes)
            {
                var row = Instantiate(ItemPrefab, transform);
                row.gameObject.SetActive(true);
                row.LinkTo(Inventory, t);
            }
        }
        public void Update()
        {
            
        }
    }
}