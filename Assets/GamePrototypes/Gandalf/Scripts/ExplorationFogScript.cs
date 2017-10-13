using System.Collections.Generic;
using Assets.Gandalf.Domain;
using Assets.Gandalf.Domain.Observable;
using UnityEngine;

namespace Assets.Gandalf.Scripts
{
    public class ExplorationFogScript : MonoBehaviour
    {
        public Transform FogPrefab;
        private CustomSubscriber<Cell> newlyExploredCells;
        private TilePlaceHelper tilePlaceHelper;

        private Dictionary<Cell, Transform> fogsDictionary = new Dictionary<Cell, Transform>();
        public void Start()
        {
            var di = GandalfDIScript.Instance;
            var explorationService = di.Get<ExplorationService>();
            newlyExploredCells = explorationService.NewlyExploredCells.Subscribe();

            tilePlaceHelper = di.Get<TilePlaceHelper>();

            var grid = di.Get<Grid>();
            for (int x = 0; x < grid.Size; x++)
            {
                for (int y = 0; y < grid.Size; y++)
                {
                    updateCell(grid.Get(x, y));
                }
            }

        }

        public void Update()
        {
            foreach (var cell in newlyExploredCells.ConsumeBuffer())
            {
                updateCell(cell);
            }

        }

        private void updateCell(Cell cell)
        {
            var isRendered = fogsDictionary.ContainsKey(cell);
            var shouldRender = !cell.IsExplored;
            if (isRendered == shouldRender) return;

            if (shouldRender)
            {
                var newFog = Instantiate(FogPrefab, transform);
                fogsDictionary[cell] = newFog;
                tilePlaceHelper.ToTransform(newFog, cell);
            }
            else
            {
                Destroy(fogsDictionary[cell].gameObject);
                fogsDictionary.Remove(cell);
            }
        }
    }
}