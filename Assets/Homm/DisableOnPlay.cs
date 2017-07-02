using UnityEngine;

namespace Assets.Homm
{
    public class DisableOnPlay : MonoBehaviour
    {
        public void Start()
        {
            gameObject.SetActive(false);
        }
    }
}