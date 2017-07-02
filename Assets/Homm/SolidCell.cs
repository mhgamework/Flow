using UnityEngine;

namespace Assets.Homm
{
    public class SolidCell : MonoBehaviour
    {
        public void Start()
        {
            
        }

        public void Update()
        {
            var grid = HommMain.Instance.Grid;
            var cell = grid.pointToCell(transform.position);
            grid.Get(cell).IsWalkable = false;
            enabled = false;
        }
    }
}