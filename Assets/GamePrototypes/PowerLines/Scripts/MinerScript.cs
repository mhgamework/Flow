using UnityEngine;

namespace Assets.PowerLines.Scripts
{
    public class MinerScript : MonoBehaviour, IPlayerRemoveable
    {
        public WirePoleScript WirePolePrefab;
        public Transform OutputWirePolePosition;
        public Transform PowerWirePolePosition;

        public float GenerationSpeed = 1;


        private WirePoleScript outputPole;

        public void Start()
        {
            outputPole = Instantiate(WirePolePrefab, OutputWirePolePosition, false);
        }

        public void Update()
        {
            outputPole.tryAdd(GenerationSpeed * Time.deltaTime);
        }

        public void Remove()
        {
            outputPole.Remove();
            Destroy(gameObject);
        }
    }
}