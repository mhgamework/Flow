using System.Collections.Generic;
using Assets.SimpleGame.Scripts;
using UnityEngine;

namespace Assets.SimpleGame.Wards
{
    public class MagicProjectileScript : MonoBehaviour
    {
        public ParticleSystem TrailParticles;
        public ParticleSystem CenterParticles;
        public ParticleSystem HitParticles;

        public float ProjectileDamage = 20;
        public float ProjectileSpeed = 5;
        Rigidbody theRigidbody;
        ParticleSystem particleSystem;

        private void Start()
        {
            if (theRigidbody == null)
                theRigidbody = GetComponent<Rigidbody>();
            if (particleSystem == null)
                particleSystem = GetComponent<ParticleSystem>();
        }

        public void OnCollisionEnter(Collision collision)
        {
            var target = collision.collider.GetComponentInParent<EntityScript>();
            if (target != null)
            {
                target.TakeDamage(ProjectileDamage);
            }
            theRigidbody.isKinematic = true;
            StartCoroutine(die().GetEnumerator());
        }

        IEnumerable<YieldInstruction> die()
        {
            TrailParticles.Stop();
            CenterParticles.Stop();
            HitParticles.Play();
            yield return new WaitForSeconds(5);
            Destroy(gameObject);

        }

        public void Fire(Vector3 pos, Vector3 forward)
        {
            Start();
            transform.position = pos;
            theRigidbody.position = pos;
            theRigidbody.velocity = forward.normalized * ProjectileSpeed;
        }
    }
}