using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Gandalf.Domain
{
    public class Goblin
    {
        public void SetRoute(Cell source, Cell target)
        {
            Source = source;
            Target = target;
        }

        public IEnumerable<YieldInstruction> SimulateMovement()
        {
            for (;;)
            {
                yield return null;
                if (Position == Source)
                {
                    // Pickup
                    yield return new WaitForSeconds(2000);

                    var forest = Position.Get<Forest>().FirstOrDefault();
                    if (forest == null) continue;
                    if (forest.Items.Take(ItemType.Wood, 1) == 0) continue;

                    Items.Add(ItemType.Wood, 1);

                }
                else if (Position == Target)
                {
                    // Pickup
                    yield return new WaitForSeconds(2000);

                    var city = Position.Get<City>().FirstOrDefault();
                    if (city == null) continue;
                    if (Items.Get(ItemType.Wood) == 0) continue;
                    if (Items.Take(ItemType.Wood, 1) == 0) continue;

                    city.Items.Add(ItemType.Wood, 1);
                }
                else if (Items.IsEmpty)
                    foreach (var el in WalkTo(Source)) yield return el;
                else
                    foreach (var el in WalkTo(Target)) yield return el;

            }
          
        }

        private IEnumerable<YieldInstruction> WalkTo(Cell destination)
        {
            while (Position != destination)
            {
                yield return new WaitForSeconds(WalkSpeed);
                var next = Position.CalculatePath(destination).First();
                Position = next;
            }
        }

        public Vector3 CalculateRenderingPosition(float currentTime)
        {
            return Position.CenterPosition;
        }

        public float WalkSpeed = 1;

        public ItemCollection Items;

        public Cell Position { get; private set; }

        public Cell Target { get; private set; }

        public Cell Source { get; private set; }
    }
}