using UnityEngine;
using UnityEngine.UI;

namespace Assets.SimpleGame.Scripts.UI
{
    public class HitDamageOverlayScript : Singleton<HitDamageOverlayScript>
    {
        public float StartAlpha = 0.5f;
        public float OverlayTime = 1;

        public Image image;

        public void Update()
        {
            var c = image.color;
            c.a = Mathf.Max(0, c.a - Time.deltaTime / OverlayTime * StartAlpha);
            image.color = c;

        }

        public void OnHit()
        {
            var c = image.color;
            c.a = StartAlpha;
            image.color = c;
        }
    }
}