using UnityEngine;

namespace Assets.SimpleGame.Scripts.UI
{
    public class HealthBarUI : MonoBehaviour
    {
        public RectTransform RedBar;
        private float startWidth;

        private PlayerScript player;

        public void Start()
        {
            startWidth = RedBar.sizeDelta.x;
            player = PlayerScript.Instance;
        }

        public void Update()
        {
            RedBar.sizeDelta = new Vector2((player.Health / player.MaxHealth) * startWidth, RedBar.sizeDelta.y);

        }
    }
}