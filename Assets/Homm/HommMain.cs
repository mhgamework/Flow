using DirectX11;
using UnityEngine;

namespace Assets.Homm
{
    public class HommMain : Singleton<HommMain>
    {
        public Transform Tree;

        public Wizard Wizard = new Wizard();
        public int MapSize = 24;

        public Grid Grid;
        public int numTrees = 20;

        public void Start()
        {
            var edge = new Cell();
            edge.IsWalkable = false;
            Grid = new Homm.Grid(MapSize, edge);

            for (int i = 0; i < numTrees; i++)
            {
                var randPos = new Point3(Random.Range(0, MapSize), 0, Random.Range(0, MapSize));
                Grid.Get(randPos).IsWalkable = false;
                Instantiate(Tree).transform.position = randPos + new Vector3(0.5f, 0, 0.5f);
            }

        }

        public void Update()
        {

        }
    }
}