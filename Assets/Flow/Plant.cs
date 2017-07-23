using UnityEngine;

namespace Assets.Flow
{
    public class Plant : MonoBehaviour
    {
        public MagicType MagicType;
        public float OutputRate;
        private MagicGridCell magicGridCell;

        public void Update()
        {
            if (magicGridCell == null)
                magicGridCell = MagicGrid.Instance.GetCellFromWorldPos(transform.position);


            magicGridCell.ChangeMagic(MagicType, OutputRate * Time.deltaTime);

        }
    }
}