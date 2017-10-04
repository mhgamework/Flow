namespace Assets.Gandalf.Domain
{
    public struct ItemAmount
    {
        public ItemType Type;
        public int Amount;

        public ItemAmount(ItemType type, int amount)
        {
            Type = type;
            Amount = amount;
        }
    }
}