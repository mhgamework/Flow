using UnityEngine;

namespace Assets.SimpleGame.Tools
{
    public class DigToolGizmoScript : MonoBehaviour
    {
        public Transform Sphere;
        public ParticleSystem ParticlesBorderIdle;
        public ParticleSystem ParticlesBorderActive;
        public ParticleSystem ParticlesToCenter;

        private void Start()
        {
          
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show(Vector3 pointValue, bool active)
        {
            transform.position = pointValue;
            setActiveSafe(this, true);
            setActiveSafe(Sphere, true);

            setActiveSafe(ParticlesBorderIdle, !active);
            setActiveSafe(ParticlesBorderActive, active);
            setActiveSafe(ParticlesToCenter, active);
        }

        private void setActiveSafe(Component s, bool active)
        {
            if (s.gameObject.activeSelf == active) return;
            
            s.gameObject.SetActive(active);

        }
    }
}