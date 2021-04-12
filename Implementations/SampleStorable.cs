using UnityEngine;

namespace FedoraDev.FinalInventory.Implementations
{
	public class SampleStorable : IStorable
	{
		[SerializeField] string _name = "New Sample Storable";
		[SerializeField] int _stackMax = 1;
		[SerializeField] int _stackCount = 1;

		#region IStorable Implementation
		public string Name => _name;
		public int StackMax => _stackMax;
		public int StackCount { get => _stackCount; set => _stackCount = value; }

		public bool IsSameAs(IStorable storable)
		{
			SampleStorable sampleStorable = storable as SampleStorable;
			return (sampleStorable != null && sampleStorable.Name == Name);
		}
		#endregion
	}
}