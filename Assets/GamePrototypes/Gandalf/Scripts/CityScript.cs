using Assets.Gandalf.Domain;
using UnityEngine;

namespace Assets.Gandalf.Scripts
{
    public class CityScript : MonoBehaviour, IMagicChargeDistributor
    {
        private City city;
        private Cell cell;
        private TilePlaceHelper tilePlaceHelper;
        private float MagicAreaSize = 3;

        //public Transform Renderable;

        public void Start()
        {
            var di = GandalfDIScript.Instance;
            tilePlaceHelper = di.Get<TilePlaceHelper>();
            var grid = di.Get<Grid>();

            cell = tilePlaceHelper.GetCell(transform);

            city = new City(cell,grid);
            StartCoroutine(city.Simulate().GetEnumerator());

            //wizard.TeleportTo();

        }
        public void Update()
        {
            //Renderable.gameObject.SetActive(cell.IsExplored);
            //var pos = transform.position;
            //pos.y = forest.Items.Get(ItemType.Wood);
            //transform.position = pos;
        }

        bool IMagicChargeDistributor.HasMagic
        {
            get { return true; }
        }
        public bool InRange(Cell cell)
        {
            return (this.cell.Coordinate - cell.Coordinate).ToVector3().magnitude < MagicAreaSize;
        }
    }
}