using UnityEngine;

namespace Assets
{
    public class ToggleOnKey : MonoBehaviour
    {
        public KeyCode Key;
        public GameObject Target;

        public void Update()
        {
            if (Input.GetKeyDown(Key))
                Target.SetActive(!Target.activeSelf);
        }
    }
}