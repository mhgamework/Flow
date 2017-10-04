﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Gandalf.Domain;
using Assets.Gandalf.Scripts;
using Assets.Homm.UI;
using DirectX11;
using MHGameWork.TheWizards;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Gandalf
{
    public class GridUserInputScript : Singleton<GridUserInputScript>, IPointerClickHandler
    {
        private Grid grid;

        //private PlayerState playerState;

        public Cell ClickedCell { get; private set; }
        public Cell HoveredCell { get; private set; }


        public void Start()
        {
            grid = GandalfDIScript.Instance.Get<Grid>();
            //playerState = PlayerState.Instance;
        }

        public void Update()
        {
            //if (grid == null)
            //    grid = HommMain.Instance.Grid;
        }

        private void LateUpdate()
        {
            ClickedCell = null;

        }




        public void OnPointerClick(PointerEventData eventData)
        {
            if (!gameObject.activeInHierarchy) return;
            if (eventData.button != 0) return;
            Debug.Log("Clicked!");
            var pos = eventData.pointerCurrentRaycast.worldPosition;
            var targetCell = grid.GetCellContainingWorldPosition(pos);

            if (eventData.clickCount > 1) Debug.Log("ignoring dblclick");
            else 
            {
                ClickedCell = targetCell;
            }


        }

        //private void OnMouseOver()
        //{
        //    if (grid == null) return;
        //    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    //ray.origin = transform.localToWorldMatrix.MultiplyPoint(ray.origin);
        //    //ray.direction = transform.localToWorldMatrix.MultiplyVector(ray.direction);
        //    // Assume groundplane for now
        //    var plane = new Plane(Vector3.up, 0);
        //    float enter;
        //    if (!plane.Raycast(ray, out enter))
        //    {
        //        HoveredCell = null;
        //        return;
        //    }

        //    var hit = ray.GetPoint(enter);

        //    HoveredCell= grid.pointToCell(hit);

        //}



        //public void OnMove(AxisEventData eventData)
        //{
        //    throw new NotImplementedException();
        //}
    }
}