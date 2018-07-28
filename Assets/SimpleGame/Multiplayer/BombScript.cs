using System.Linq;
using UnityEngine;

namespace Assets.SimpleGame.Multiplayer
{
    public class BombScript : MonoBehaviour
    {
        public float DetonateAfter = 3;
        public bool DetonateOnImpact;

        public float ExplosionRadius = 5;
        public float ExplosionPushForce = 20;
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
                .Select(f => f.GetComponent<MultiplayerPlayerScript>()).Where(m => m != null);
            foreach (var c in colliders) c.ApplyPushAway(transform.position, ExplosionPushForce, ExplosionPushForceY, 1);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (DetonateOnImpact && armed)
                Detonate();
        }


    }
}