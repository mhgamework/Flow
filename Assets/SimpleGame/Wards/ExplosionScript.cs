using System.Collections;
using Assets.MarchingCubes.SdfModeling;
using Assets.MarchingCubes.World;
using Assets.SimpleGame.Scripts;
using Assets.SimpleGame.Scripts.Enemies;
using Assets.SimpleGame.WardDrawing;
using UnityEngine;

namespace Assets.SimpleGame.Wards
{
    public class ExplosionScript : MonoBehaviour
    {
        public float ExplodeAfter = 3;
        public float ExplosionRadius = 3;
        public float ExplosionDamage = 100;

        public AudioClip ChargeClip;
        public AudioClip ExplosionClip;

        private AudioSource AudioSource;

        public ParticleSystem ChargeParticles;
        public ParticleSystem ExplodeParticles;

        public void Start()
        {
            AudioSource = GetComponent<AudioSource>();

            StartCoroutine(Trigger().GetEnumerator());
        }



        IEnumerable Trigger()
        {
            AudioSource.clip = ChargeClip;
            AudioSource.Play();
            ChargeParticles.Play();
            yield return new WaitForSeconds(ExplodeAfter);
            ExplodeParticles.Play();
            AudioSource.Stop();
            AudioSource.clip = ExplosionClip;
            AudioSource.Play();
            DestroyTerrain();
            DoDamage();
            GetComponentInChildren<MeshWardViewScript>().gameObject.SetActive(false);
            yield return new WaitForSeconds(5);

            Destroy(gameObject);
        }

        private void DoDamage()
        {
            var colliders = Physics.OverlapSphere(transform.position, ExplosionRadius);
            foreach (var c in colliders)
            {
                var enemy = c.GetComponentInParent<WoodDemonScript>();
                if (enemy != null)
                    Destroy(enemy.gameObject);
                var player = c.GetComponentInChildren<PlayerScript>();
                if (player != null)
                {
                    var dmg = Mathf.Lerp(ExplosionDamage, 10,
                        Mathf.Clamp01((player.transform.position - transform.position).magnitude / ExplosionRadius));
                    player.TakeDamage(dmg);
                }
            }
        }

        private void DestroyTerrain()
        {
            var world = SimpleGameStartupScript.Instance.RenderingEngine.GetWorld();
            if (world == null) return;

            var s = new Ball(transform.position, ExplosionRadius);

            var b = new Bounds();
            b.SetMinMax((this.transform.position - Vector3.one * ExplosionRadius), (this.transform.position + Vector3.one * ExplosionRadius));

            new SDFWorldEditingService() .Subtract(world, s, b);
        }

    }
}