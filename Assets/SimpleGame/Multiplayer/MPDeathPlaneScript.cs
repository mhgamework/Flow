using UnityEngine;

namespace Assets.SimpleGame.Multiplayer
{
    public class MPDeathPlaneScript : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            other.GetComponentInParent<MultiplayerPlayerScript>().OnFallOfWorld();
            Debug.Log("Fell out of map! " + other.gameObject);
        }
    }
}