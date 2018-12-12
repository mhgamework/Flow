using System;
using System.Collections;
using Assets.MarchingCubes.World;
using Assets.MHGameWork.FlowEngine.SdfModeling;
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
                EntityDamageUtils.DoDamageToSmth(c, t => Mathf.Lerp(ExplosionDamage, 10, Mathf.Clamp01((t.position - transform.position).magnitude / ExplosionRadius)));
            }
        }

  

        private void DestroyTerrain()
        {
            var world = SimpleGameStartupScript.Instance.RenderingEngine.GetWorld();
            if (world == null) return;

            var scale = SimpleGameStartupScript.Instance.RenderingEngine.RenderScale;

            var s = new Ball(transform.position / scale, ExplosionRadius / scale);

            var b = new Bounds();
            b.SetMinMax((this.transform.position / scale - Vector3.one * ExplosionRadius / scale), (this.transform.position / scale + Vector3.one * ExplosionRadius / scale));

            new SDFWorldEditingService().Subtract(world, s, b);
        }

    }
}