using Assets.Gandalf.Scripts;

namespace Assets.Gandalf.Domain
{
    public interface IGridElementFactory
    {
        Goblin CreateGoblin();
        void Remove(Goblin goblin);
        MagicExtenderScript CreateMagicExtender(Cell cell);
        void Destroy(MagicExtenderScript magicExtenderScript);
    }
}