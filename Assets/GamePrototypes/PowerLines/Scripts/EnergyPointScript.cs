using UnityEngine;

namespace Assets.PowerLines.Scripts
{
    public class EnergyPointScript : MonoBehaviour
    {
        public float Energy;
        public float MaxEnergy = 10;

        public float Pressure;

        public Transform EnergyRenderer;
        public Transform PressureRenderer;

        public void Update()
        {
            PressureRenderer.localScale = Vector3.one * Pressure / 30;//MaxEnergy;
            EnergyRenderer.localScale = Vector3.one * Energy / 30;
        }
    }
}