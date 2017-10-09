using UnityEngine;

namespace Assets.Gandalf.Scripts
{
    public class GridRendererScript : MonoBehaviour
    {
        public CellRendererScript CellRendererPrefab;
        public void Start()
        {
            var grid = GandalfDIScript.Instance.Get<Grid>();
            var tilePlaceHelper = GandalfDIScript.Instance.Get<TilePlaceHelper>();
            for (int x = 0; x < grid.Size; x++)
                for (int y = 0; y < grid.Size; y++)
                {
                    var renderable = Instantiate(CellRendererPrefab, transform);
                    renderable.Cell = grid.Get(x, y);
                    renderable.gameObject.SetActive(true);
                    tilePlaceHelper.ToTransform(renderable.transform, renderable.Cell);

                }
        }

        public void Update()
        {

        }

    }
}