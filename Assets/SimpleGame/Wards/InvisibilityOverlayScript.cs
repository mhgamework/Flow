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
        }

        public void Update()
        {
            if (playerScript == null)
                playerScript = LocalPlayerScript.Instance.GetPlayer();
            image.enabled = playerScript.Entity.Invisible;
        }
    }
}