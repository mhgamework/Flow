using UnityEngine;

namespace Assets.Flow
{
    public class TextPopup : MonoBehaviour, ISecondaryInteractable
    {
        public TextPopupUi Prefab;

        private TextPopupUi instance;

        public Vector3 Offset;

        public void OnUnfocus(Player p)
        {
            if (instance == null) return;
            instance.gameObject.SetActive(false);
        }

        public void OnFocus(Player p)
        {
            if (instance == null)
            {
                instance = Instantiate(Prefab);
                instance.transform.parent = transform;
                instance.transform.position += Offset;
            }

            instance.gameObject.SetActive(true);

            

        }
    }
}