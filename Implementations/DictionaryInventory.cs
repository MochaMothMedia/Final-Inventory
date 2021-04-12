using System.Collections.Generic;
using UnityEngine;

namespace FedoraDev.FinalInventory.Implementations
{
	public class DictionaryInventory : IInventory
	{
		public int MaxCount { get => _maxCount; set => _maxCount = value; }

		[SerializeField] int _maxCount = 32;
		[SerializeField] Dictionary<int, IStorable> _storage = new Dictionary<int, IStorable>();

		IStorable AddStack(IStorable storable)
		{
			if (_storage.Count >= MaxCount)
				return storable;

			for (int i = 0; i < MaxCount; i++)
			{
				if (!_storage.ContainsKey(i))
				{
					_storage.Add(i, storable);
					return null;
				}
			}

			return storable;
		}

		#region IInventory Implementation
		public void Swap(int index1, int index2)
		{
			if (index1 >= MaxCount && index2 >= MaxCount)
				return;

			if (_storage.ContainsKey(index1) && _storage.ContainsKey(index2))
			{
				IStorable cachedStorable = _storage[index1];
				_storage[index1] = _storage[index2];
				_storage[index2] = cachedStorable;
				return;
			}

			int containedKey = _storage.ContainsKey(index1) ? index1 : index2;
			int lostKey = index1 == containedKey ? index2 : index1;

			_storage.Add(lostKey, _storage[containedKey]);
			_storage.Remove(containedKey);
		}

		public IStorable AddAllOrFail(IStorable storable)
		{
			if (HasAvailableSpaceFor(storable))
				return AddAndReturnRemainder(storable);
			return storable;
		}

		public IStorable AddAndReturnRemainder(IStorable storable)
		{
			if (storable.StackMax <= 1)
				return AddStack(storable);

			foreach (KeyValuePair<int, IStorable> inventorySlot in _storage)
			{
				if (!inventorySlot.Value.IsSameAs(storable))
					continue;
				if (inventorySlot.Value.StackCount >= inventorySlot.Value.StackMax)
					continue;

				if (inventorySlot.Value.StackCount + storable.StackCount > inventorySlot.Value.StackMax)
				{
					storable.StackCount -= inventorySlot.Value.StackMax - inventorySlot.Value.StackCount;
					inventorySlot.Value.StackCount = inventorySlot.Value.StackMax;
					return AddAndReturnRemainder(storable);
				}

				inventorySlot.Value.StackCount += storable.StackCount;
				return null;
			}

			return AddStack(storable);
		}

		public IStorable AddAtPosition(IStorable storable, int index)
		{
			if (_storage.Count >= MaxCount)
				return storable;

			if (_storage.ContainsKey(index))
			{
				IStorable pushedStorable = AddAtPosition(_storage[index], index + 1);
				if (pushedStorable == null)
					return storable;
			}

			_storage.Add(index, storable);
			return null;
		}

		public IStorable GetSpecific(IStorable storable, int quantity = 1)
		{
			if (GetQuantity(storable) < quantity)
				return null;

			foreach (KeyValuePair<int, IStorable> inventorySlot in _storage)
			{
				if (!inventorySlot.Value.IsSameAs(storable))
					continue;

				if (inventorySlot.Value.StackCount >= quantity)
				{
					inventorySlot.Value.StackCount -= quantity;
					if (inventorySlot.Value.StackCount == 0)
						_storage.Remove(inventorySlot.Key);

					storable.StackCount += quantity;
					return storable;
				}

				int count = inventorySlot.Value.StackCount;
				storable.StackCount += count;
				_storage.Remove(inventorySlot.Key);
				return GetSpecific(storable, quantity - count);
			}

			return null;
		}

		public IStorable GetStackAt(int index)
		{
			if (!_storage.ContainsKey(index))
				return null;

			IStorable cachedStorable = _storage[index];
			_storage.Remove(index);
			return cachedStorable;
		}

		public IStorable ProbeAtPosition(int index)
		{
			if (_storage.ContainsKey(index))
				return _storage[index];
			return null;
		}

		public int GetQuantity(IStorable storable)
		{
			int quantity = 0;

			foreach (KeyValuePair<int, IStorable> inventorySlot in _storage)
				if (inventorySlot.Value.IsSameAs(storable))
					quantity += inventorySlot.Value.StackCount;

			return quantity;
		}

		public bool HasAvailableSpaceFor(IStorable storable)
		{
			int space = 0;

			for (int i = 0; i < MaxCount; i++)
			{
				if (!_storage.ContainsKey(i))
				{
					space += storable.StackMax;
					if (space >= storable.StackCount)
						return true;
					continue;
				}

				if (_storage[i].IsSameAs(storable))
					space += _storage[i].StackMax - _storage[i].StackCount;
			}

			return space >= storable.StackCount;
		}

		public void ClearInventory()
		{
			_storage = new Dictionary<int, IStorable>();
		}
		#endregion
	}
}