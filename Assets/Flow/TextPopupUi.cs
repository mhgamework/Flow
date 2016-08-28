using UnityEngine;
using UnityEngine.UI;

namespace Assets.Flow
{
    public class TextPopupUi : MonoBehaviour
    {

        [SerializeField]
        private Text text;

        public void SetText(string value)
        {
            text.text = value;
        }

        public void Update()
        {
            var p = Camera.main.transform.position;
            p.y = transform.position.y;
            transform.LookAt(p);
        }
    }
}