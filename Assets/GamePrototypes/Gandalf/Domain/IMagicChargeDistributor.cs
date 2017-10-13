namespace Assets.Gandalf.Domain
{
    public interface IMagicChargeDistributor
    {
        bool HasMagic { get; }
        bool InRange(Cell cell);
    }
}