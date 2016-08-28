using UnityEngine;

namespace Assets.Flow
{
    public class MagicCrystals : MonoBehaviour, IInteractable
    {
        public ParticleSystem Particles;

        public string Type;

        public void Start()
        {
            Particles.Stop();
            Particles.startSpeed = 0.5f;
        }

        public void OnUnfocus(Player p)
        {
            Particles.Stop();
        }

        public void OnFocus(Player p)
        {
            Particles.Play();
        }

        public void TryInteract(Player p, Vector3 point)
        {
            var pressed = Input.GetMouseButton(0);
            Particles.startSpeed = pressed ? 3f : 0.5f;

            if (Type == "water")
                p.WaterMagic = Mathf.Min(p.MaxMagic, p.WaterMagic + Time.deltaTime * p.MagicExtractionSpeed);
            else if (Type == "fire")
                p.FireMagic = Mathf.Min(p.MaxMagic, p.FireMagic + Time.deltaTime * p.MagicExtractionSpeed);
            else if (Type == "clay")
                p.Clay = Mathf.Min(p.MaxClay, p.Clay + Time.deltaTime * p.ClayExtractionSpeed);


        }
    }
}