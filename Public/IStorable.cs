namespace FedoraDev.FinalInventory
{
    public interface IStorable
    {
        string Name { get; }
        int StackMax { get; }
        int StackCount { get; set; }

        bool IsSameAs(IStorable storable);
    }
}