using UnityEngine;

namespace Assets.PowerLines.Scripts
{
    public class WirePoleScript : MonoBehaviour, IPlayerRemoveable
    {
        public WirePoleScript EditorConnectTo;

        public void Start()
        {
            if (EditorConnectTo != null)
                WireSystemScript.Instance.ConnectPoles(this, EditorConnectTo);
        }

        public void Remove()
        {
            WireSystemScript.Instance.OnRemovePole(this);
            Destroy(gameObject);
        }

        public void tryAdd(float amount)
        {
            var energyPointScript = GetComponent<EnergyPointScript>();
            energyPointScript.Energy = Mathf.Min(energyPointScript.MaxEnergy, energyPointScript.Energy + amount);


        }
    }
}