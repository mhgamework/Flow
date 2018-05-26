using System;
using Assets.SimpleGame.Scripts.Enemies;
using UnityEngine;

namespace Assets.SimpleGame.Scripts
{
    /// <summary>
    /// Temporary static class to hold how to do dmg. Should be generalized
    /// </summary>
    public static class EntityDamageUtils
    {
        public static void DoDamageToSmth(Collider c, Func<Transform, float> calculateDamage)
        {
            var enemy = c.GetComponentInParent<WoodDemonScript>();
            if (enemy != null)
                UnityEngine.Object.Destroy(enemy.gameObject);
            var player = c.GetComponentInChildren<PlayerScript>();
            if (player != null)
            {
                var dmg = calculateDamage(player.transform);
                player.TakeDamage(dmg);
            }
        }
    }
}