using System.Linq;
using Assets.SimpleGame.Multiplayer.Players;
using UnityEngine;

namespace Assets.SimpleGame.Multiplayer
{
    /// <summary>
    /// Script for an exploding grenade-like object with an arming + detonate after time mechanic
    /// </summary>
    public class BombScript : MonoBehaviour
    {
        public float DetonateAfter = 3;
        public bool DetonateOnImpact;

        public float ExplosionRadius = 5;
        public float ExplosionPushForce = 21;
        public float ExplosionPushForceY = 20;


        private float armTime;
        private bool armed = false;
        public void Arm()
        {
            if (armed) return;
            armed = true;
            armTime = Time.timeSinceLevelLoad;
        }

        public void Update()
        {
            if (!armed) return;

            if (DetonateAfter + armTime < Time.timeSinceLevelLoad)
            {
                Detonate();
            }
        }

        private void Detonate()
        {
            Destroy(gameObject);

            var colliders = Physics.OverlapSphere(transform.position, ExplosionRadius)
                .Select(f => f.GetComponent<MultiplayerScenePlayerScript>()).Where(m => m != null);
            foreach (var c in colliders) c.GetComponent<PlayerMovementScript>().ApplyPushAway(transform.position, ExplosionPushForce, ExplosionPushForceY, 1);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (DetonateOnImpact && armed)
                Detonate();
        }


    }
}