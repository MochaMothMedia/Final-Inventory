using System.Collections.Generic;
using UnityEngine;

public class ArrayInventory : IInventory
{
	public int MaxCount => _maxCount;

	[SerializeField] int _maxCount = 32;
	[SerializeField] List<IStorable> _storage = new List<IStorable>();

	IStorable AddStack(IStorable storable)
	{
		if (_storage.Count < MaxCount)
		{
			_storage.Add(storable);
			return null;
		}

		return storable;
	}

	#region IInventory Implementation
	public void Swap(int index1, int index2)
	{
		if (index1 >= _storage.Count && index2 >= _storage.Count)
			return;

		IStorable cachedStorable;

		if (index1 < _storage.Count && index2 < _storage.Count)
		{
			cachedStorable = _storage[index1];
			_storage[index1] = _storage[index2];
			_storage[index2] = cachedStorable;
			return;
		}

		int smallIndex = index1 > index2 ? index1 : index2;

		cachedStorable = _storage[smallIndex];
		_storage.RemoveAt(smallIndex);
		_storage.Add(cachedStorable);
	}

	public IStorable AddAllOrFail(IStorable storable)
	{
		if (HasAvailableSpaceFor(storable))
			return AddAndReturnRemainder(storable);
		return storable;
	}

	public IStorable AddAndReturnRemainder(IStorable storable)
	{
		if (storable.StackMax > 1)
		{
			foreach (IStorable unit in _storage)
			{
				if (unit.IsSameAs(storable) && unit.StackCount < unit.StackMax)
				{
					if (unit.StackCount + storable.StackCount > unit.StackMax)
					{
						storable.StackCount -= (unit.StackMax - unit.StackCount);
						unit.StackCount = unit.StackMax;
					}
					else
					{
						unit.StackCount += storable.StackCount;
						storable.StackCount = 0;
					}

					if (storable.StackCount > 0)
						return AddAllOrFail(storable);
					return null;
				}
			}
		}

		return AddStack(storable);
	}

	public IStorable AddAtPosition(IStorable storable, int index)
	{
		if (_storage.Count >= MaxCount) return storable;

		if (index < _storage.Count)
		{
			_storage.Insert(index, storable);
			return null;
		}

		_storage.Add(storable);
		return null;
	}

	public IStorable GetSpecific(IStorable storable, int quantity = 1)
	{
		if (GetQuantity(storable) >= quantity)
		{
			foreach (IStorable unit in _storage)
			{
				if (unit.IsSameAs(storable))
				{
					if (unit.StackCount >= quantity)
					{
						unit.StackCount -= quantity;
						if (unit.StackCount == 0)
							_storage.Remove(unit);

						storable.StackCount += quantity;
						return storable;
					}

					int count = unit.StackCount;
					storable.StackCount += count;
					_storage.Remove(unit);
					return GetSpecific(storable, quantity - count);
				}
			}
		}

		return null;
	}

	public IStorable GetStackAt(int index)
	{
		if (_storage.Count > index)
		{
			IStorable cachedStorable = _storage[index];
			_storage.RemoveAt(index);
			return cachedStorable;
		}

		return null;
	}

	public IStorable ProbeAtPosition(int index)
	{
		if (_storage.Count > index)
			return _storage[index];
		return null;
	}

	public int GetQuantity(IStorable storable)
	{
		int quantity = 0;

		foreach (IStorable unit in _storage)
			if (unit.IsSameAs(storable))
				quantity += unit.StackCount;

		return quantity;
	}

	public bool HasAvailableSpaceFor(IStorable storable)
	{
		int space = 0;

		for (int i = 0; i < MaxCount; i++)
		{
			if (i < _storage.Count)
			{
				if (_storage[i].IsSameAs(storable))
					space += _storage[i].StackMax - _storage[i].StackCount;
			}
			else
			{
				space += storable.StackMax;
				if (space >= storable.StackCount) return true;
			}
		}

		return space >= storable.StackCount;
	}

	public void ClearInventory() => _storage = new List<IStorable>();
	#endregion
}
