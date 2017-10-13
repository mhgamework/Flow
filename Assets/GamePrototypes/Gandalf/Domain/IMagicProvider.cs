namespace Assets.Gandalf.Domain
{
    public interface IMagicProvider
    {
        bool HasMagic();
        bool TakeMagic(int amount);
    }
}