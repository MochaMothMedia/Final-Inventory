public interface IInventory
{
    int MaxCount { get; }
    int StorageCount { get; }

    void Swap(int index1, int index2);
    
    IStorable AddAtPosition(IStorable storable, int index);
    IStorable AddAllOrFail(IStorable storable);
    IStorable AddAndReturnRemainder(IStorable storable);
    IStorable ProbeAtPosition(int index);
    IStorable GetStackAt(int index);
    IStorable GetSpecific(IStorable storable, int quantity = 1);

    int GetQuantity(IStorable storable);
    int HasAvailableSpaceFor(IStorable storable);

    void ClearInventory();
}
