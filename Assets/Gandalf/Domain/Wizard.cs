﻿using System.Collections.Generic;
using System.Linq;

namespace Assets.Gandalf.Domain
{
    public class Wizard
    {
        private readonly GridElementFactory elementFactory;
        private readonly ExplorationService explorationService;
        public Cell CurrentCell { get; private set; }

        public Wizard(GridElementFactory elementFactory,ExplorationService explorationService)
        {
            this.elementFactory = elementFactory;
            this.explorationService = explorationService;
        }

        public void MoveTo(Cell cell)
        {
            if (!GetMovementOptions().Contains(cell)) throw new System.Exception();

            TeleportTo(cell);
        }

        public IEnumerable<Cell> GetMovementOptions()
        {
            if (CurrentCell == null) return Enumerable.Empty<Cell>();
            return CurrentCell.Neighbours4.ToArray();
        }

        public Goblin SetGoblin(Cell from, Cell to)
        {
            var goblin = elementFactory.CreateGoblin();
            goblin.SetRoute(from, to);
            return goblin;
        }

        public void RemoveGoblin(Goblin goblin)
        {
            if (!CurrentCell.Get<Goblin>().Contains(goblin))
                throw new System.Exception("Wizard cannot remove goblins that are not in the wizards cell");

            elementFactory.Remove(goblin);
        }

        public void TeleportTo(Cell cell)
        {
            CurrentCell = cell;
            explorationService.Explore(this, cell);
        }
    }
}