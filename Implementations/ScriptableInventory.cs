using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory")]
public class ScriptableInventory : SerializedScriptableObject, IInventory
{
	public int MaxCount => _inventory.MaxCount;

	public int StorageCount => _inventory.StorageCount;

	public void Swap(int index1, int index2) => _inventory.Swap(index1, index2);

	public IStorable AddAllOrFail(IStorable storable) => _inventory.AddAllOrFail(storable);
	public IStorable AddAndReturnRemainder(IStorable storable) => _inventory.AddAndReturnRemainder(storable);
	public IStorable AddAtPosition(IStorable storable, int index) => _inventory.AddAtPosition(storable, index);
	public IStorable GetSpecific(IStorable storable, int quantity = 1) => _inventory.GetSpecific(storable, quantity = 1);
	public IStorable GetStackAt(int index) => _inventory.GetStackAt(index);
	public IStorable ProbeAtPosition(int index) => _inventory.ProbeAtPosition(index);

	public int GetQuantity(IStorable storable) => _inventory.GetQuantity(storable);
	public int HasAvailableSpaceFor(IStorable storable) => _inventory.HasAvailableSpaceFor(storable);

	public void ClearInventory() => _inventory.ClearInventory();

	[SerializeField]
	[BoxGroup("Inventory")]
	[HideLabel]
	IInventory _inventory;
}
