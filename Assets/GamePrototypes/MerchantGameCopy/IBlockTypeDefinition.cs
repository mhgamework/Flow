using Assets.MineMinecraft;

namespace Assets
{
    public interface IBlockTypeDefinition
    {
        string TypeName { get; }
        IBlock CreateBlockInstance();
    }
}