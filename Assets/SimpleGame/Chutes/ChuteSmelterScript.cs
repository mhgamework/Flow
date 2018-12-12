using System.Collections.Generic;
using Assets.UnityAdditions;
using UnityEngine;

namespace Assets.SimpleGame.Chutes
{
    public class ChuteSmelterScript : MonoBehaviour
    {
        public ChuteTransportPointScript TopInput;
        public ChuteTransportPointScript BackInput;
        public ChuteTransportPointScript FrontOutput;

        private bool itemReady = false;
        private ChuteItemType smeltedItem;
            

        public float SmeltInterval = 1;

        public void Start()
        {
            this.StartCoroutine(doStuff);
        }

        public void Update()
        {

        }

        IEnumerable<YieldInstruction> doStuff()
        {
            for (; ; )
            {
                if (TopInput.HasItem(t=> t.Smelted != null) && BackInput.HasItem(t => t.IsFuel) && !itemReady)
                {
                    // Smelt!
                    var sourceItem = TopInput.TakeItem();
                    var fuelItem =BackInput.TakeItem();
                    sourceItem.Destroy();
                    fuelItem.Destroy();
                    itemReady = true;
                    smeltedItem = sourceItem.Type.Smelted;
                }

                if (itemReady && FrontOutput.IsFree)
                {
                    Debug.Log("Free " + FrontOutput.IsFree);
                    FrontOutput.EmitIfFreeSpace( smeltedItem);
                    itemReady = false;
                    smeltedItem = null;
                }

            
                yield return new WaitForSeconds(SmeltInterval);
            }
        }
    }
}