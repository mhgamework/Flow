using Assets.SimpleGame.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.SimpleGame.Wards
{
    public class InvisibilityOverlayScript : MonoBehaviour
    {
        private Image image;
        private PlayerScript playerScript;

        private void OnEnable()
        {
            image = GetComponent<Image>();
            playerScript = PlayerScript.Instance;
        }

        public void Update()
        {
            image.enabled = playerScript.Entity.Invisible;
        }
    }
}