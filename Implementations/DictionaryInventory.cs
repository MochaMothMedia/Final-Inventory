using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DictionaryInventory : IInventory
{
	public int MaxCount => _maxCount;

	[SerializeField] int _maxCount = 32;
	[SerializeField] Dictionary<int, IStorable> _storage = new Dictionary<int, IStorable>();

	#region IInventory Implementation
	public void Swap(int index1, int index2)
	{
		if (index1 >= MaxCount && index2 >= MaxCount) return;

		if (_storage.ContainsKey(index1) && _storage.ContainsKey(index2))
		{
			IStorable cachedStorable = _storage[index1];
			_storage[index1] = _storage[index2];
			_storage[index2] = cachedStorable;
			return;
		}

		int largeIndex = index1 < index2 ? index2 : index1;
		int smallIndex = index1 < index2 ? index1 : index2;

		_storage[smallIndex] = _storage[largeIndex];
		_storage.Remove(largeIndex);
	}

	public IStorable AddAllOrFail(IStorable storable) => throw new System.NotImplementedException();
	public IStorable AddAndReturnRemainder(IStorable storable) => throw new System.NotImplementedException();
	public IStorable AddAtPosition(IStorable storable, int index) => throw new System.NotImplementedException();
	public IStorable GetSpecific(IStorable storable, int quantity = 1) => throw new System.NotImplementedException();
	public IStorable GetStackAt(int index) => throw new System.NotImplementedException();
	public IStorable ProbeAtPosition(int index) => throw new System.NotImplementedException();

	public int GetQuantity(IStorable storable) => throw new System.NotImplementedException();

	public bool HasAvailableSpaceFor(IStorable storable) => throw new System.NotImplementedException();

	public void ClearInventory() => throw new System.NotImplementedException();
	#endregion
}
