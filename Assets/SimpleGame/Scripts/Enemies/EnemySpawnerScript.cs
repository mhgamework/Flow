using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.SimpleGame.Scripts.Enemies
{
    /// <summary>
    /// Spawns enemies every x time
    /// </summary>
    public class EnemySpawnerScript : MonoBehaviour
    {
        private List<WoodDemonScript> enemies = new List<WoodDemonScript>();
        private float lastSpawn = -9999;
        public float SpawnInterval;
        public int MaxEnemies;

        public WoodDemonScript EnemyPrefab;

        public void Update()
        {
            if (DayNightCycleScript.Instance.IsDay()) return;

            if (enemies.Count(f => f != null) < MaxEnemies)
            {
                trySpawn();
            }
        }

        private void trySpawn()
        {
            if (lastSpawn + SpawnInterval > Time.realtimeSinceStartup) return;

            var enemy = Instantiate(EnemyPrefab);
            enemies.Add(enemy);
            enemy.transform.position = transform.position;
                 
            lastSpawn = Time.realtimeSinceStartup;

        }
    }
}