namespace Assets.Gandalf.Domain
{
    public interface IGridElementFactory
    {
        Goblin CreateGoblin();
        void Remove(Goblin goblin);
    }
}