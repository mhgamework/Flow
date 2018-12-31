using UnityEngine;

namespace Assets.MHGameWork.FlowGame.PlayerInputting.Interacting
{
    public class FlowGameInteractableScript : MonoBehaviour
    {
        [SerializeField]
        private Transform highlight;

        public void Start()
        {
            HideHighlight();
        }
        public void HideHighlight()
        {
            highlight.gameObject.SetActive(false);
        }

        public void ShowHighlight()
        {
            highlight.gameObject.SetActive(true);
        }
    }
}