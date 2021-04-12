using NUnit.Framework;
using NSubstitute;
using System.Collections.Generic;
using FedoraDev.FinalInventory.Implementations;

namespace FedoraDev.FinalInventory.Tests
{
	public class DictionaryInventoryTests
	{
		private IInventory _inventory;

		[SetUp]
		public void Init()
		{
			_inventory = new DictionaryInventory();
			_inventory.MaxCount = 32;
		}

		[TearDown]
		public void TearDown()
		{
			_inventory.ClearInventory();
			_inventory.MaxCount = 32;
		}

		[Test]
		public void Can_Add_A_Storable_To_Inventory()
		{
			IStorable storable = Substitute.For<IStorable>();

			_ = _inventory.AddAndReturnRemainder(storable);

			Assert.AreSame(storable, _inventory.ProbeAtPosition(0));
		}

		[Test]
		public void Adding_Two_Similar_Storables_Increments_Quantity_Of_Storable_In_Inventory()
		{
			IStorable storable1 = Substitute.For<IStorable>();
			IStorable storable2 = Substitute.For<IStorable>();
			storable1.StackCount.Returns(1);
			storable2.StackCount.Returns(1);
			storable1.StackMax.Returns(999);
			storable2.StackMax.Returns(999);

			storable1.IsSameAs(storable2).Returns(true);
			storable2.IsSameAs(storable1).Returns(true);

			_ = _inventory.AddAndReturnRemainder(storable1);
			_ = _inventory.AddAndReturnRemainder(storable2);

			Assert.AreEqual(2, _inventory.ProbeAtPosition(0).StackCount);
		}

		[Test]
		public void Adding_Two_Similar_Storables_Exceeding_Stack_Max_Creates_Second_Stack()
		{
			IStorable storable1 = Substitute.For<IStorable>();
			IStorable storable2 = Substitute.For<IStorable>();
			storable1.StackCount.Returns(1);
			storable2.StackCount.Returns(1);
			storable1.StackMax.Returns(1);
			storable2.StackMax.Returns(1);

			storable1.IsSameAs(storable2).Returns(true);
			storable2.IsSameAs(storable1).Returns(true);

			_ = _inventory.AddAndReturnRemainder(storable1);
			_ = _inventory.AddAndReturnRemainder(storable2);

			Assert.AreEqual(1, _inventory.ProbeAtPosition(0).StackCount);
			Assert.AreEqual(1, _inventory.ProbeAtPosition(1).StackCount);
		}

		[Test]
		public void Adding_More_Than_Inventory_Can_Hold_Returns_Leftovers()
		{
			List<IStorable> storables = new List<IStorable>();

			for (int i = 0; i < _inventory.MaxCount + 1; i++)
			{
				IStorable storable = Substitute.For<IStorable>();
				storable.StackCount.Returns(1);
				storable.StackMax.Returns(1);

				for (int j = 0; j < storables.Count; j++)
				{
					storables[j].IsSameAs(storable).Returns(true);
					storable.IsSameAs(storables[j]).Returns(true);
				}

				storables.Add(storable);
			}

			for (int i = 0; i < storables.Count - 1; i++)
			{
				IStorable insertedStorable = _inventory.AddAndReturnRemainder(storables[i]);
				Assert.IsNull(insertedStorable);
			}

			IStorable leftoverStorable = _inventory.AddAndReturnRemainder(storables[storables.Count - 1]);

			Assert.AreEqual(1, leftoverStorable.StackCount, "Returned storable does not have StackCount == 1.");
		}

		[Test]
		public void Can_Swap_Slot_With_Empty_Slot()
		{
			IStorable storable = Substitute.For<IStorable>();
			_ = _inventory.AddAndReturnRemainder(storable);

			Assert.IsNotNull(_inventory.ProbeAtPosition(0));
			Assert.IsNull(_inventory.ProbeAtPosition(1));

			_inventory.Swap(0, 1);

			Assert.IsNull(_inventory.ProbeAtPosition(0));
			Assert.IsNotNull(_inventory.ProbeAtPosition(1));
			Assert.AreSame(storable, _inventory.ProbeAtPosition(1));
		}

		[Test]
		public void Can_Swap_Slot_With_Other_Slot()
		{
			IStorable storable1 = Substitute.For<IStorable>();
			IStorable storable2 = Substitute.For<IStorable>();

			_ = _inventory.AddAndReturnRemainder(storable1);
			_ = _inventory.AddAndReturnRemainder(storable2);

			Assert.IsNotNull(_inventory.ProbeAtPosition(0));
			Assert.IsNotNull(_inventory.ProbeAtPosition(1));

			_inventory.Swap(0, 1);

			Assert.IsNotNull(_inventory.ProbeAtPosition(0));
			Assert.IsNotNull(_inventory.ProbeAtPosition(1));
			Assert.AreSame(storable1, _inventory.ProbeAtPosition(1));
			Assert.AreSame(storable2, _inventory.ProbeAtPosition(0));
		}

		[Test]
		public void Can_Add_Storable_At_Specific_Index()
		{
			IStorable storable = Substitute.For<IStorable>();

			_ = _inventory.AddAtPosition(storable, 4);

			Assert.IsNull(_inventory.ProbeAtPosition(0));
			Assert.AreSame(storable, _inventory.ProbeAtPosition(4));
		}

		[Test]
		public void Can_Add_All_If_Room_In_Inventory()
		{
			List<IStorable> storables = new List<IStorable>();

			for (int i = 0; i < _inventory.MaxCount; i++)
			{
				IStorable storable = Substitute.For<IStorable>();
				storable.StackCount.Returns(1);
				storable.StackMax.Returns(1);

				for (int j = 0; j < storables.Count - 1; j++)
				{
					storables[j].IsSameAs(storable).Returns(true);
					storable.IsSameAs(storables[j]).Returns(true);
				}

				storables.Add(storable);
			}

			for (int i = 0; i < storables.Count - 1; i++)
			{
				IStorable insertedStorable = _inventory.AddAndReturnRemainder(storables[i]);
				Assert.IsNull(insertedStorable);
			}

			IStorable storableMany1 = Substitute.For<IStorable>();
			IStorable storableMany2 = Substitute.For<IStorable>();
			storableMany1.StackCount.Returns(1);
			storableMany2.StackCount.Returns(4);
			storableMany1.StackMax.Returns(5);
			storableMany2.StackMax.Returns(5);
			storableMany1.IsSameAs(storableMany2).Returns(true);
			storableMany2.IsSameAs(storableMany1).Returns(true);

			_ = _inventory.AddAndReturnRemainder(storableMany1);

			IStorable leftoverStorable = _inventory.AddAllOrFail(storableMany2);

			Assert.IsNull(leftoverStorable, "Leftover returned with some remainder.");
			Assert.AreEqual(5, _inventory.ProbeAtPosition(_inventory.MaxCount - 1).StackCount, "StackCount of item in inventory didn't increase to 5.");
		}

		[Test]
		public void Adds_None_If_Not_Enough_Space_With_Add_All()
		{
			List<IStorable> storables = new List<IStorable>();

			for (int i = 0; i < _inventory.MaxCount; i++)
			{
				IStorable storable = Substitute.For<IStorable>();
				storable.StackCount.Returns(1);
				storable.StackMax.Returns(1);

				for (int j = 0; j < storables.Count - 1; j++)
				{
					storables[j].IsSameAs(storable).Returns(true);
					storable.IsSameAs(storables[j]).Returns(true);
				}

				storables.Add(storable);
			}

			for (int i = 0; i < storables.Count - 1; i++)
			{
				IStorable insertedStorable = _inventory.AddAndReturnRemainder(storables[i]);
				Assert.IsNull(insertedStorable);
			}

			IStorable storableMany1 = Substitute.For<IStorable>();
			IStorable storableMany2 = Substitute.For<IStorable>();
			storableMany1.StackCount.Returns(1);
			storableMany2.StackCount.Returns(5);
			storableMany1.StackMax.Returns(5);
			storableMany2.StackMax.Returns(5);
			storableMany1.IsSameAs(storableMany2).Returns(true);
			storableMany2.IsSameAs(storableMany1).Returns(true);

			_ = _inventory.AddAndReturnRemainder(storableMany1);

			IStorable leftoverStorable = _inventory.AddAllOrFail(storableMany2);

			Assert.AreEqual(5, leftoverStorable.StackCount, "Leftover didn't return with same quantity.");
			Assert.AreEqual(1, _inventory.ProbeAtPosition(_inventory.MaxCount - 1).StackCount, "Inventory didn't reject storable when there wasn't enough space.");
		}

		[Test]
		public void Quantity_Check_Returns_Quantity_From_Multiple_Stacks()
		{
			List<IStorable> storables = new List<IStorable>();

			for (int i = 0; i < _inventory.MaxCount; i++)
			{
				IStorable storable = Substitute.For<IStorable>();
				storable.StackCount.Returns(1);
				storable.StackMax.Returns(1);
				storable.IsSameAs(storable).Returns(true);

				for (int j = 0; j < storables.Count; j++)
				{
					storables[j].IsSameAs(storable).Returns(true);
					storable.IsSameAs(storables[j]).Returns(true);
				}

				storables.Add(storable);
			}

			for (int i = 0; i < storables.Count; i++)
			{
				IStorable insertedStorable = _inventory.AddAndReturnRemainder(storables[i]);
				Assert.IsNull(insertedStorable);
			}

			int quantity = _inventory.GetQuantity(storables[0]);

			Assert.AreEqual(_inventory.MaxCount, quantity);
		}

		[Test]
		public void Can_Pull_Storables_From_Inventory()
		{
			IStorable storable1 = Substitute.For<IStorable>();
			IStorable storable2 = Substitute.For<IStorable>();
			storable1.StackCount.Returns(5);
			storable1.IsSameAs(storable2).Returns(true);
			storable2.IsSameAs(storable1).Returns(true);

			_ = _inventory.AddAndReturnRemainder(storable1);
			IStorable returnedStorable = _inventory.GetSpecific(storable2, 1);

			Assert.AreEqual(1, returnedStorable.StackCount);
			Assert.AreEqual(4, _inventory.ProbeAtPosition(0).StackCount);
		}

		[Test]
		public void Pulling_Storables_Without_Enough_Pulls_None()
		{
			IStorable storable1 = Substitute.For<IStorable>();
			IStorable storable2 = Substitute.For<IStorable>();
			storable1.StackCount.Returns(1);
			storable1.IsSameAs(storable2).Returns(true);
			storable2.IsSameAs(storable1).Returns(true);

			_ = _inventory.AddAndReturnRemainder(storable1);
			IStorable returnedStorable = _inventory.GetSpecific(storable2, 5);

			Assert.IsNull(returnedStorable);
			Assert.AreEqual(1, _inventory.ProbeAtPosition(0).StackCount);
		}

		[Test]
		public void Can_Pull_From_Multiple_Stacks()
		{
			IStorable storable1 = Substitute.For<IStorable>();
			IStorable storable2 = Substitute.For<IStorable>();
			IStorable storable3 = Substitute.For<IStorable>();

			storable1.StackMax.Returns(1);
			storable2.StackMax.Returns(1);

			storable1.StackCount.Returns(1);
			storable2.StackCount.Returns(1);
			storable3.StackCount.Returns(0);

			storable1.IsSameAs(storable2).Returns(true);
			storable1.IsSameAs(storable3).Returns(true);
			storable2.IsSameAs(storable1).Returns(true);
			storable2.IsSameAs(storable3).Returns(true);
			storable3.IsSameAs(storable1).Returns(true);
			storable3.IsSameAs(storable2).Returns(true);

			_ = _inventory.AddAndReturnRemainder(storable1);
			_ = _inventory.AddAndReturnRemainder(storable2);

			IStorable returnedStorable = _inventory.GetSpecific(storable3, 2);

			Assert.AreEqual(2, returnedStorable.StackCount);
		}

		[Test]
		public void Can_Pull_At_Specific_Index()
		{
			IStorable storable1 = Substitute.For<IStorable>();
			IStorable storable2 = Substitute.For<IStorable>();
			IStorable storable3 = Substitute.For<IStorable>();

			_ = _inventory.AddAndReturnRemainder(storable1);
			_ = _inventory.AddAndReturnRemainder(storable2);
			_ = _inventory.AddAndReturnRemainder(storable3);

			IStorable returnedStorable = _inventory.GetStackAt(1);

			Assert.AreSame(storable2, returnedStorable);
		}

		[Test]
		public void Can_Clear_Inventory()
		{
			IStorable storable1 = Substitute.For<IStorable>();
			_ = _inventory.AddAndReturnRemainder(storable1);

			Assert.AreEqual(storable1, _inventory.ProbeAtPosition(0));

			_inventory.ClearInventory();

			Assert.IsNull(_inventory.ProbeAtPosition(0));
		}
	}
}