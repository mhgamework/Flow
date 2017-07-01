using UnityEngine;

namespace Assets.Homm
{
    public class HommMain : Singleton<HommMain>
    {
        public Wizard Wizard = new Wizard();
        public int MapSize = 24;

        public Grid Grid;

        public void Start()
        {
            Grid = new Homm.Grid(MapSize);
        }

        public void Update()
        {
            
        }
    }
}