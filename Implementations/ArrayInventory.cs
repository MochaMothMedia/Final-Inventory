using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrayInventory : IInventory
{
	public int MaxCount => throw new System.NotImplementedException();
	public int StorageCount => throw new System.NotImplementedException();

	public void Swap(int index1, int index2) => throw new System.NotImplementedException();

	public IStorable AddAllOrFail(IStorable storable) => throw new System.NotImplementedException();
	public IStorable AddAndReturnRemainder(IStorable storable) => throw new System.NotImplementedException();
	public IStorable AddAtPosition(IStorable storable, int index) => throw new System.NotImplementedException();
	public IStorable GetSpecific(IStorable storable, int quantity = 1) => throw new System.NotImplementedException();
	public IStorable GetStackAt(int index) => throw new System.NotImplementedException();
	public IStorable ProbeAtPosition(int index) => throw new System.NotImplementedException();

	public int GetQuantity(IStorable storable) => throw new System.NotImplementedException();
	public int HasAvailableSpaceFor(IStorable storable) => throw new System.NotImplementedException();

	public void ClearInventory() => throw new System.NotImplementedException();
}
