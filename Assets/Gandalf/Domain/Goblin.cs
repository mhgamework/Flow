using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Gandalf.Domain
{
    public class Goblin : ICellEntity, IUISelectable
    {
        private Grid grid;
        private readonly IGridElementFactory gridElementFactory;

        public Goblin(Grid grid, IGridElementFactory gridElementFactory)
        {
            this.grid = grid;
            this.gridElementFactory = gridElementFactory;
            Items = new ItemCollection();
        }

        public void SetRoute(Cell source, Cell target)
        {
            Position = source;
            Source = source;
            Target = target;
        }

        public IEnumerable<YieldInstruction> SimulateMovement()
        {
            for (; ; )
            {
                yield return null;
                if (LifeUntil < Time.time)
                {
                    if (!Position.TakeMagic(1)) continue;
                    LifeUntil = Time.time + 3;
                }
                if (Items.IsEmpty && Position != Source)
                    foreach (var el in WalkToSingleStep(Source)) yield return el;
                else if (Items.IsEmpty && Position == Source)
                {
                    // Pickup
                    yield return new WaitForSeconds(2);

                    if (Position.Items.Take(ItemType.Wood, 1) == 0) continue;

                    Items.Add(ItemType.Wood, 1);
                }
                else if (!Items.IsEmpty && Position != Target)
                    foreach (var el in WalkToSingleStep(Target)) yield return el;
                else if (!Items.IsEmpty && Position == Target)
                {
                    // Pickup
                    yield return new WaitForSeconds(2);

                    if (Items.Get(ItemType.Wood) == 0) continue;
                    if (Items.Take(ItemType.Wood, 1) == 0) continue;

                    Position.Items.Add(ItemType.Wood, 1);
                }

            }

        }

        private IEnumerable<YieldInstruction> WalkToSingleStep(Cell destination)
        {
            //while (Position != destination)
            //{
                yield return new WaitForSeconds(WalkSpeed);
                var next = Position.CalculatePath(destination).Reverse().Skip(1).First();
                Position = next;
            //}
        }

        public Vector3 CalculateRenderingPosition(float currentTime)
        {
            return Position.CenterPosition - new Vector3(1, 0, 1) * grid.GridCellSize * 0.5f;
        }

        public float LifeUntil = 0;

        public float WalkSpeed = 1;

        public ItemCollection Items { get; private set; }

        public Cell Position { get; private set; }

        public Cell Target { get; private set; }

        public Cell Source { get; private set; }
        public string GetCellInfo()
        {
            return "Goblin " + Items.ToItemString();
        }

        public IDictionary<string, string> Values
        {
            get { return new Dictionary<string, string>() { { "LifeForce", (LifeUntil - Time.time).ToString() } }; }
        }

        public string Name { get { return "Goblin"; } }
        public IEnumerable<string> Actions { get { return new string[] { "Destroy" }; } }
        public void DoAction(string action)
        {
            if (action == "Destroy")
                gridElementFactory.Remove(this);
        }
    }
}