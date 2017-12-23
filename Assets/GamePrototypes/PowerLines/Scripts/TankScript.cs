using UnityEngine;

namespace Assets.PowerLines.Scripts
{
    public class TankScript : MonoBehaviour, IPlayerRemoveable
    {
        public float Capacity = 100;
        public WirePoleScript WirePolePrefab;
        public Transform WirePolePos;

        private WirePoleScript pole;

        public void Start()
        {
            pole=Instantiate(WirePolePrefab, WirePolePos, false);
            pole.GetComponent<EnergyPointScript>().MaxEnergy = Capacity;
        }

        public void Update()
        {

        }

        public void Remove()
        {
            Destroy(gameObject);
        }
    }
}