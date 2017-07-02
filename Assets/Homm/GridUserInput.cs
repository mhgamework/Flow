using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Homm.UI;
using DirectX11;
using MHGameWork.TheWizards;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Homm
{
    public class GridUserInput : BaseUIState, IPointerClickHandler
    {
        private Grid grid;

        private PlayerState playerState;


        public void Start()
        {
            playerState = PlayerState.Instance;
        }

        public void Update()
        {
            if (grid == null)
                grid = HommMain.Instance.Grid;

        }



        public void OnPointerClick(PointerEventData eventData)
        {
            if (!gameObject.activeInHierarchy) return;
            if (eventData.button != 0) return;
            Debug.Log("Clicked!");
            var pos = eventData.pointerCurrentRaycast.worldPosition;
            var targetCell = grid.pointToCell(pos);

            if (eventData.clickCount > 1) Debug.Log("igoring dblclick");
            else if (playerState.Wizard.SelectedTargetLocation == targetCell)
            {
                GameMaster.Instance.MoveHero();
            }

            playerState.Wizard.SetTargetLocation(targetCell);

        }

    }
}