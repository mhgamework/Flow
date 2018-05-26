using UnityEngine;

namespace Assets.SimpleGame.Scripts
{
    public class DeathPlaneScript : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            EntityDamageUtils.DoDamageToSmth(other, t => 9999999);
            Debug.Log("Fell out of map! " + other.gameObject);
        }
    }
}