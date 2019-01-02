using System;
using UnityEngine;

namespace Assets.MHGameWork.FlowGame.PlayerInputting.Interacting
{
    public class FlowGameInteractableScript : MonoBehaviour
    {
        [SerializeField]
        private Transform highlight;

        public Action PlayerInteractHandler { get; set; }

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

        public void OnPlayerInteract()
        {
            if (PlayerInteractHandler != null) PlayerInteractHandler();
        }

    }
}