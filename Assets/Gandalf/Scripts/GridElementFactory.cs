using System.Collections.Generic;
using Assets.Gandalf.Domain;
using UnityEngine;

namespace Assets.Gandalf.Scripts
{
    public class GridElementFactory : MonoBehaviour,IGridElementFactory
    {
        public MagicExtenderScript MagicExtenderPrefab;

        private Dictionary<object, GameObject> dict = new Dictionary<object, GameObject>();
        public GoblinScript GoblinPrefab;
        private Grid grid;

        public void Start()
        {
            var di = GandalfDIScript.Instance;

            grid = di.Get<Grid>();
        }

        public Goblin CreateGoblin()
        {
            var goblin = Instantiate(GoblinPrefab, transform);
            goblin.Goblin = new Goblin(grid,this);

            dict.Add(goblin.Goblin, goblin.gameObject);

            return goblin.Goblin;
        }

        public void Remove(Goblin goblin)
        {
            var obj = dict[goblin];
            Destroy(obj);
            dict.Remove(goblin);

            UIControllerScript.Instance.OnGoblinRemoved(goblin);
        }

        public MagicExtenderScript CreateMagicExtender(Cell cell)
        {
            var ret = Instantiate(MagicExtenderPrefab, transform);

            ret.GetComponent<CellEntityScript>().SetCell(cell);

            return ret;

        }


        public void Destroy(MagicExtenderScript magicExtenderScript)
        {
            Object.Destroy(magicExtenderScript);
        }
    }
}