using Assets.Gandalf.Domain;
using UnityEngine;

namespace Assets.Gandalf.Scripts
{
    public class SpawnResourcesScript : MonoBehaviour
    {
        public float SpawnRate = 1;
        public int SpawnAmount = 1;
        public string ResourceType = "Wood";

        private ItemType itemType;
        private float nextSpawn = 0;
        private CellEntityScript cellEntityScript;

        public void Start()
        {
            cellEntityScript = GetComponent<CellEntityScript>();
            nextSpawn = SpawnRate;
            itemType = ItemType.Get(ResourceType);

        }

        public void Update()
        {
            nextSpawn -= Time.deltaTime;
            if (nextSpawn > 0) return;

            nextSpawn = SpawnRate;

            if (cellEntityScript.Cell.Items.Get(itemType) > 0) return;
            cellEntityScript.Cell.Items.Add(itemType, SpawnAmount);

        }
    }
}