using Assets.Gandalf.Domain;
using UnityEngine;

namespace Assets.Gandalf.Scripts
{
    public class CellEntityScript : MonoBehaviour
    {
        private Transform Renderable;
        public Cell Cell { get; private set; }


        public string Name;
        public bool IsWalkable = true;

        public void Start()
        {
            var di = GandalfDIScript.Instance;
            var tilePlaceHelper = di.Get<TilePlaceHelper>();
            var grid = di.Get<Grid>();

            SetCell(tilePlaceHelper.GetCell(transform));
            Renderable = transform.FindChild("Renderable");

            var magicChargeDistributor = (IMagicChargeDistributor)GetComponent(typeof(IMagicChargeDistributor));
            if (magicChargeDistributor != null)
                GandalfDIScript.Instance.Get<MagicChargeService>().MagicChargeDistributors.Add(magicChargeDistributor);
        }
        public void SetCell(Cell cell)
        {
            GandalfDIScript.Instance.Get<TilePlaceHelper>().ToTransform(transform, cell);
            Cell = cell;

            cell.NewEntity = this;

        }

        public void Update()
        {
            Renderable.gameObject.SetActive(Cell.IsExplored);

        }

        public void OnDestroy()
        {
            if (Cell != null)
                Cell.NewEntity = null;
        }
    }
}