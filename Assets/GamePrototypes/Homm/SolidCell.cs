﻿using UnityEngine;

namespace Assets.Homm
{
    public class SolidCell : MonoBehaviour
    {
        public string Description = "";
        public void Start()
        {
            
        }

        public void Update()
        {
            var grid = HommMain.Instance.Grid;
            var cell = grid.pointToCell(transform.position);
            grid.Get(cell).IsWalkable = false;
            grid.Get(cell).Description = Description;
            enabled = false;
        }
    }
}