using Assets.Gandalf.Domain;
using UnityEngine;

namespace Assets.Gandalf.Scripts
{
    public class ForestScript : MonoBehaviour
    {
        private Forest forest;
        private Cell cell;
        private TilePlaceHelper tilePlaceHelper;
        public float GrowInterval = 1;

        public Transform Renderable;

        public void Start()
        {
            var di = GandalfDIScript.Instance;
            forest = new Forest();
            tilePlaceHelper = di.Get<TilePlaceHelper>();
            forest.GrowInterval = GrowInterval;
            StartCoroutine(forest.Simulate().GetEnumerator());

            cell = tilePlaceHelper.GetCell(transform);
            //wizard.TeleportTo();

        }
        public void Update()
        {
            Renderable.gameObject.SetActive(cell.IsExplored);
            //var pos = transform.position;
            //pos.y = forest.Items.Get(ItemType.Wood);
            //transform.position = pos;
        }
    }
}