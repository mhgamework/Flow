using System.Collections.Generic;
using Boo.Lang.Runtime;
using UnityEngine;

namespace Assets.SimpleGame.Chutes
{
    public class ChuteDropperScript : MonoBehaviour
    {
        public List<ChuteTransportPointScript> Nodes = new List<ChuteTransportPointScript>();

        private List<ChuteItemScript> items = new List<ChuteItemScript>();

        public void Update()
        {
            if (!Nodes[0].Chute) return;

            for (int i = 1; i < Nodes.Count; i++)
            {
                var node = Nodes[i];

                //                if (node.IsDownwards)
                //                {
                //                    if (node.IsFree && hasItemAt(i))
                //                    {
                //                        var item = takeItemAt(i);
                //                        node.Insert(item);
                //                    }
                //                }
                //                else
                //                {
                //                    if (node.HasItem() && hasSpaceAt(i))
                //                    {
                //                        var item = node.TakeItem();
                //                        insertAt(i, item);
                //                    }
                //                }



                if (!node.Chute) continue;

                if (Nodes[0].Chute.HasFreeSpace(Nodes[0]))
                {
                    var get = node.Chute.TakeItemAtTransportPoint(node);
                    if (get)
                    {
                        if (!Nodes[0].Chute.TryInsertItem(get, Nodes[0]))
                            throw new RuntimeException("Should do this!");
                    }

                }

            }
        }


    }
}