# Final Inventory

A lightweight and extendible inventory system for Unity.

## Installation

###### Prerequisites
This project uses [Odin Inspector](https://assetstore.unity.com/packages/tools/utilities/odin-inspector-and-serializer-89041), which I cannot redistribute. If you don't own Odin Inspector, I would highly recommend purchasing it otherwise you won't be able to serialize interface instances as members which completely breaks this solution.

Grab [NSubstitute from NuGet](https://www.nuget.org/packages/NSubstitute). Extract the zip and grab `NSubstitute.dll` and add it to your project `Assets/Plugins` folder.

###### Package Manager
In Unity's Package Manager, you can add the repo as a git package. I have been able to get this to work by using the `https` method, however you can't grab a specific version this way and will always be stuck using the latest release. In Github, select `Code > HTTPS` and click on the clip board. Back in Unity, open Package Manager and hit the plus button and select `Add package from git url`. Paste the link there and the package will be added automatically.

###### Manual Installation
Unity's Package Manager seems to have issues with git repositories, however this can be added as a dependency to your Unity project manually.

Next, you need to add a reference to this repo to your project's `Packages/manifest.json` file.

```
{
  "dependencies": {
    ...,
    "com.fedoradev.finalinventory": "https://github.com/FedoraDevStudios/FinalInventory.git#1.0.2"
  }
}
```

After Unity reloads, you can open the TestRunner window `Window > General > Test Runner` and `Run All` to ensure everything works properly.

###### Manual Upgrade
After installing manually, you have to change both `Packages/manifest.json` and `Packages/packages-lock.json`. In the former, simply update the dependency with the version you wish to pull. In the lock file, you need to remove the entry for the package. This entry is a few lines long and everything needs to be deleted, including the curly braces. After this is successfully completed, moving back to Unity will force the application to download the desired version.

## Usage
### Inventory
###### Scriptable Object Asset
Using the asset create menu, use `Create/Inventory` to create an asset. You can then assign the type of inventory and fill it in the inspector. This asset will store anything that implements `IInventory` so you can create your own implementations and the asset can be switched to them.

###### Adding references

If you're using `asmdef` files, you should only need to reference `_FedoraDev.FinalInventory`. You can reference `_FedoraDev.FinalInventory.Implementations` as well, however this will create hard dependencies and isn't the intended way to use this system.

Additionally, you can reference `IInventory` anywhere in your own code, so you don't have to use the scriptable objects. For instance, if you want an inventory on a Game Object, you can assign as below.

```c#
using Sirenix.OdinInspector;
using FedoraDev.FinalInventory;
using UnityEngine;

public class MyGameObject : SerializedMonoBehaviour
{
  [SerializeField] private IInventory _inventory;
}
```

###### Custom IInventory Implementations
If you're looking to create your own `IInventory` implementations, your class should implement the following. After creating this implementation, all instances of `IInventory`, including the default `Scriptable Object` asset can be assigned your implementation. Note that functions that take an `IStorable` with no parameter for quantity will use the quantity in the `IStorable`. This is useful depending on how your `IStorable` implements `IsSameAs()`. You can create an instance of `IStorable` at runtime with a desired quantity in `StackCount` and pass that object into these functions.

```c#
public class MyInventory : IInventory
{
  //Max slots in the inventory
  int MaxCount { get; set; }
  
  //Swaps two indexes
  void Swap(int index1, int index2) { ... }
  
  //Attempt to insert storable at the index
  IStorable AddAtPosition(IStorable storable, int index) { ... }
  
  //Add entire stack, otherwise adds nothing
  IStorable AddAllOrFail(IStorable storable) { ... }
  
  //Add what will fit and return the rest
  IStorable AddAndReturnRemainder(IStorable storable) { ... }
  
  //Returns the IStorable at the index in the inventory
  IStorable ProbeAtPosition(int index) { ... }
  
  //Removes and returns entire IStorable at location
  IStorable GetStackAt(int index) { ... }
  
  //Removes and returns quantity of IStorable from inventory
  IStorable GetSpecific(IStorable storable, int quantity = 1) { ... }
  
  //Returns the quantity of a specific IStorable found in inventory
  int GetQuantity(IStorable storable) { ... }
  
  //Checks if the IStorable will fit in the inventory
  bool HasAvailableSpaceFor(IStorable storable) { ... }
  
  //Wipes the inventory
  void ClearInventory() { ... }
}
```

### Storable
Anything in your game that can be serialized can also be a storable by implementing `IStorable`. This is intended to be implemented along with other system interfaces to work in your game. For instance, if you have a GUI to show your inventory, you should also have some interface like `IShowInInventoryGUI` with some icon as an example. Then, to create your item, simple make it implement both interfaces. This system is not responsible for displaying an inventory, so I'll be omitting any implementation details pertaining that.

The power for this system really shines when you have multiple types that can be stored in an inventory. For instance, you can make your NPCs storable and the inventory really doesn't mind. Just make sure you can serialize any required data.

```c#
using Sirenix.OdinInspector;
using FedoraDev.FinalInventory;
using UnityEngine;

public class CoolItem : SerializedScriptableObject, IStorable
{
  [SerializeField] private int _stackMax;
  [SerializeField] private int _stackCount;
  
  public string Name => name;
  public int StackMax => _stackMax;
  public int StackCount { get => _stackCount; set => _stackCount = value; }
  
  public bool IsSameAs(IStorable storable)
  {
  	CoolItem item = storable as CoolItem;
  	return (item != null && item.Name == Name);
  }
}
```
