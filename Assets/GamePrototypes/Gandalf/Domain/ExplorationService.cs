using Assets.Gandalf.Domain.Observable;

namespace Assets.Gandalf.Domain
{
    public class ExplorationService
    {
        public CustomObservable<Cell> NewlyExploredCells { get; set; }

        public ExplorationService()
        {
            NewlyExploredCells = new CustomObservable<Cell>();
        }

        public void Explore(Wizard w,Cell cell)
        {
            cell.MarkExplored();
            NewlyExploredCells.OnNext(cell);
        }
    }
}