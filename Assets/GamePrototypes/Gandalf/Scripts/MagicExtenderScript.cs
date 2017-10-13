using Assets.Gandalf.Domain;
using UnityEngine;

namespace Assets.Gandalf.Scripts
{
    public class MagicExtenderScript : MonoBehaviour, IMagicChargeDistributor
    {
        public CellEntityScript CellEntityScript;
        public float MagicAreaSize = 2;


        public void Start()
        {
            CellEntityScript = GetComponent<CellEntityScript>();
        }
        public void Update()
        {
            
        }

        bool IMagicChargeDistributor.HasMagic
        {
            get { return true; }
        }

        public bool InRange(Cell cell)
        {
            return (CellEntityScript.Cell.Coordinate - cell.Coordinate).ToVector3().magnitude < MagicAreaSize;
        }
    }
}