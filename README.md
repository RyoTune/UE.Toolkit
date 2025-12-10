<div align="center">
	<img src="/UE.Toolkit.Reloaded/Preview.png" height="256"/>
	<h1>Unreal Toolkit</h1>
	<p>
		General modding toolkit for Unreal Engine games using Reloaded II.
	</p>
</div>

## Features
- `UObject` and `UDataTable` logging.
- **Edit object data (numbers and text) [with just XML text files](#editing-objects-with-xml)**.
- Simplified `UObject` and `UDataTable` editing at runtime.
- Access to `FMemory` functions ([supported functions](#using-unreals-memory-allocator-fmemory)).
- Mod for [dumping game types to C# types](#ue-toolkit-dumper).
- Methods for creating `FString` and `FText`.
- Get type information from a class or struct.
- Extend the length of a type and add custom constructor code.
- Add new properties to a class/struct's type information.
- Register a new struct into the type information system.

## Supported Engine Version and Games

Object XML requires game-specific support with an extension mod. Supported games [are listed here](#installing-the-ue-toolkit-extension-mod).

| Feature | UE 4.27.2 | UE 5.4.4 |
| - | - | - |
| Object Logging | ✅| ✅
| Object Editing | ✅| ✅
| `FMemory` Functions | ✅| ✅
| Dumper | ✅| ✅
| Property Editing (Object XML) | ✅| ✅
| Add List Entry (Object XML) | ✅| ❔
| Add Map Entry (Object XML) | ✅| ❔
| Type Information | ✅| ✅
| Custom Constructor | ✅| ❔
| Add Properties | ✅| ❌
| Register Struct | ✅| ❌

Features marked with ❔ are currently untested.

## Installation
### Setup Reloaded and Add Your Game
I recommend the P3R guides, since they're the newest and cover GamePass. __You only really need to add your game to Reloaded, you can ignore anything past that.__

[__Beginner's Guide to Modding Persona 3 Reload__](https://gamebanana.com/tuts/17156)

[__Beginner's Guide to Modding P3R (Xbox / Game Pass)__](https://gamebanana.com/tuts/17171)

### Installing UE Toolkit
1. Go to [Releases](https://github.com/RyoTune/UE.Toolkit/releases) and download the latest version of `UE.Toolkit.Reloaded.7z`
2. Drag and drop the `7z` file into Reloaded to install.

### Installing the UE Toolkit Extension Mod
Install the extension mod for your game to be able to use all features (Object XML Editing).
- __Clair Obscur__ - https://github.com/RyoTune/E33.UEToolkit/releases
- __Persona 3 Reload__ - https://github.com/RyoTune/P3R.UEToolkit/releases

## Editing Objects with XML
In supported games, you're able to edit any object with just a simple text file (XML). No Unreal Engine, no file unpacking/repacking/cooking, and no hex 🤮 editing. Just _Notepad++_ and a dream ✨! ~~Or Notepad if you hate yourself...~~

On top of that, your edits __only__ edit what you're editing! Or put non-stupidly, __mod merging__ is built in 🎉! Mods only conflict if they edit the exact same thing, like the price of the same item for example.

### Requirements
- UE Toolkit (Main Mod)
- The UE Toolkit extension mod for your game.
- Enable "View File Extensions" on Windows

### Creating a Mod (Reloaded)
This will be a bit abridged since I expect most people using this mod are experienced with Reloaded and my other mods.

#### Steps
1. Create a Reloaded mod with a __Mod Dependency__ on the extension mod. You can add the main mod too if you like, but it's not required.
2. In your mod's folder, create the following folders: `MOD_FOLDER/ue-toolkit/objects`
3. Inside the `objects` folder, you will place any __Object XML__ files you make. They can be in sub-folders for organization.

### Creating a Mod (Unreal's `~mods` Folder)
Not recommended or really supported, but some people don't like change and/or hate Reloaded. Reloaded is great though 💖!

*Only supported on Clair Obscur's extension mod.*
#### Steps
1. In the game's `~mods` folder, create the following folders: `../~mods/ue-toolkit/objects`
2. Inside the `objects` folder, create a folder for *your* specific mod: `../~mods/ue-toolkit/objects/My Super Cool Mod`
3. Inside your mod's folder, you will place any __Object XML__ files you make. They can be in sub-folders for organization. 

### Creating an Object XML
Now for actually editing an object. You only need two (2) pieces of information to get started: the object's __Name__ and its __Class__ (and __RowStruct__ for _DataTables_).

You can easily get both using [__FModel__](https://github.com/4sval/FModel/releases).

#### Steps
1. Inside your chosen folder from the previous steps, create a new __text file__.
2. Name the file the same as the object's __Name__.
3. Change the file extension from `.txt` to `.obj.xml`. Your file's name should be similar to: `DT_jRPG_CharacterDefinitions.obj.xml`
4. Open your file in a text editor.

### Writing an Object XML
Generally, your XML will match the "shape" of the object, starting with its __Class__. I highly recommend looking at the object in __FModel__ or going to the extension mod's source if you can read code.

The best I can give is a general example, since it'll look very different depending on what you're editing.

#### The Root Element
The first element in your XML, aka the __Root Element__, will be the object's __Class__. In this example, taken from _Clair Obscur_'s `DL_Goblu_00Entry.uasset`, the __Class__ is a `DataLayerAsset`.

```xml
<DataLayerAsset>

</DataLayerAsset>
```

#### Editing Properties
A `DataLayerAsset` has two properties: `DataLayerType` and `DebugColor`.  We'll first edit `DataLayerType`.

```xml
<DataLayerAsset>
	<DataLayerType value="1"/> 
	OR
	<DataLayerType>1</DataLayerType>
</DataLayerAsset>
```

##### Steps
1. Add a new element with the same name as the property, in this case `DataLayerType`.
2. Add a `value` attribute to the element, with the value you want to set it to.
3. OR between the open and closing tags.

#### Editing Properties of Properties
We started with `DataLayerType` since it's value is directly in `DataLayerAsset`, but what if a property has its own properties? For example, `DebugColor` has RGB properties. 

Well, it's not too different than what we already have with the __Root Element__.

```xml
<DataLayerAsset>
	<DataLayerType value="1"/>
	<DebugColor>
		<R value="255"/>
		<G value="255"/>
		<B value="255"/>
	</DebugColor>
</DataLayerAsset>
```
##### Steps
1. Same as before, add a new element with the name of the property __but__ with open and closing tags.
2. Inside this element, add new elements with the names of the properties (like `DataLayerType` earlier.)

If a sub-property has _its_ own property, then just repeat the same process as needed.

#### Editing Items in a List (Arrays)
For editing lists, arrays, or DataTables a bit later, the process a slightly different since we need to set _which_ item we want to edit. For that, we use a special `Item` element.

```xml
<ArmorItemListTable>
	<Data>
		<Item id="2">
		    <EquipID value="100"/>
		</Item>
	</Data>
</ArmorItemListTable>
```

In this example, from _Persona 3 Reload_'s `DatArmorItemListTable` (not a DataTable), `ArmorItemListTable` is the __Class__, which has a `Data` property that's a list of of armor items.

First, we add an `Item` element with an `id` attribute. The ID is which item in the list we want to edit. The first item would have an ID of `1`, the second item `2`, third `3`, and so on.

Inside the `Item` element, we're back to what's already been covered. In this example, items have an `EquipID` property and we're setting the `EquipID` of the __2nd__ item to `100`.

#### Editing Items in a DataTable
Finally,  _DataTables_! There's only two notable differences: the `row-struct` attribute and `id`s are now row names. This example is from _Clair Obscur_'s `DT_jRPG_CharacterDefinitions.uasset`.

```xml
<DataTable row-struct="S_jRPG_CharacterDefinition">
  <Item id="Lune">
      <CharacterDisplayName value="TEST1"/>
  </Item>
  <Item id="Maelle">
      <CharacterDisplayName value="TEST2"/>
  </Item>
  <Item id="Sciel">
      <CharacterDisplayName value="TEST3"/>
  </Item>
</DataTable>
```

A DataTable's __Class__ is... `DataTable`! Who could've guessed? As such, the __Root Element__ in our XML has to be `DataTable`. 

But, we also need to specify the __Class (Struct)__ for the __Items (Rows)__ in the DataTable so we can edit them. You do that by adding a `row-struct` attribute with the DataTable's __RowStruct__ name, `S_jRPG_CharacterDefinition` in this example.

For our `Item` element's ID, instead of a number we use the __Row's name__ that we want to edit. Inside the `Item` element, it's no different than lists from earlier.

That covers everything, congrats on finishing this 🎉🎉🎉!

#### Misc. Stuff (Enums, Type Hints)
##### Enums
For enum properties (stuff like `EDataLayerType::Runtime`, where it starts with an `E`), you can use name values if you know them. You can find them in the extension mod's source or in __FModel__.

## UE Toolkit: Dumper
1. Launch game and get to the main menu or later.
2. In Reloaded, right-click `UE Toolkit: Dumper` and select `Configure`.
3. In the config window, fill in any settings you want, then click `Save` to start dumping objects.
4. Generated files will be located in the mod folder: right-click `UE Toolkit: Dumper` and select `Open Folder`.

Some Unreal types may not be generated and need to be supplied. `UE.Toolkit.Core` includes any types
that were missing in my testing. Add it to your project using **NuGet** and add `UE.Toolkit.Core.Types.Unreal;`
in the `File Usings` config before dumping.

## Code Only Features

### Using Unreal's Memory Allocator `FMemory`

When writing code mods that interact with objects allocated by Unreal, allocation must be handled on the mod's side by the
same allocator that created it in the game's code. `IUnrealMemory` contains the following methods for interacting with
Unreal's memory allocator:

- `nint Malloc(nint count, int alignment = Default)`: Allocates a block of memory with the global `FMemory`.
- `void Free(nint original)`: Deallocates a block of memory allocated with the global `FMemory`.
- `nint Realloc(nint ptr, nint count, int alignment = Default)`: Reallocates a block of memory with a new size using the global `FMemory`
- `bool GetAllocSize(nint ptr, ref nint size)`: Queries the global `FMemory` to get the size of an allocation created using it.
- `nint MallocZeroed(nint count, int alignment = Default)`: Allocates and clears a block of memory with the global `FMemory`
- `nint QuantizeSize(nint count, int alignment = Default)`: For some allocators this will return the actual size that should be requested to eliminate internal fragmentation.

(If alignment is not specified, Default will be 16 bytes for blocks 16 bytes or larger, and 8 bytes if it's smaller)

### Extending an Object's Size and Adding a Custom Constructor

`IUnrealClasses` contains the methods `AddConstructor`, which passes in a custom callback that executes after the original constructor has run. `AddExtension` additionally allows for increasing the size of the object by `ExtraSize` bytes. Do note that this should only be used on types that don't have any subtypes (no type has it at the beginning as a "Super" property).

### Adding New Properties

It's possible to add new properties into the property list for a given object in code using the `Add[Type]Property` methods in `IUnrealClasses`. This allows that field to be readable from Object XML and blueprints:
```c#
// UGlobalWork is P3R's game instance class
_context._toolkitClasses.AddConstructor<UGlobalWork>(obj => 
{
    if (!CreatedFields) 
    {        
		_context._toolkitClasses.AddU32Property<UFldManagerSubsystem>("CurrFieldMajor", 0x54, out _);
        _context._toolkitClasses.AddU32Property<UFldManagerSubsystem>("CurrFieldMinor", 0x58, out _);
        _context._toolkitClasses.AddU32Property<UFldManagerSubsystem>("CurrFieldSub", 0x5c, out _);
        CreatedFields = true;
    }
});
```
When using the dumper, these fields will appear in the generated file:

#### Before
```c#
[StructLayout(LayoutKind.Explicit, Pack = 16, Size = 0x400)]
public unsafe struct UFldManagerSubsystem
{
    [FieldOffset(0x0)] public UGameInstanceSubsystem Super; // Size: 0x30
    [FieldOffset(0x30)] public FMulticastScriptDelegate mOnEventCallField_; // Size: 0x10
    [FieldOffset(0xB8)] public AFldLevelManager* mLevelManager_; // Size: 0x8
    [FieldOffset(0xC8)] public UAppCharacterComp* mPlayerComp_; // Size: 0x8
	// Other fields ...
}
```
#### After
```c#
[StructLayout(LayoutKind.Explicit, Pack = 16, Size = 0x400)]
public unsafe struct UFldManagerSubsystem
{
    [FieldOffset(0x0)] public UGameInstanceSubsystem Super; // Size: 0x30
    [FieldOffset(0x30)] public FMulticastScriptDelegate mOnEventCallField_; // Size: 0x10
    [FieldOffset(0x54)] public uint CurrFieldMajor; // Size: 0x4
    [FieldOffset(0x58)] public uint CurrFieldMinor; // Size: 0x4
    [FieldOffset(0x5C)] public uint CurrFieldSub; // Size: 0x4
    [FieldOffset(0xB8)] public AFldLevelManager* mLevelManager_; // Size: 0x8
    [FieldOffset(0xC8)] public UAppCharacterComp* mPlayerComp_; // Size: 0x8
	// Other fields ...
}
```

### Registering New Struct Types `F[TypeName]`

A new struct can be registered into UE's type reflection. `IUnrealClasses` contains methods `(Create[Type]Param)` to build a list of property params to pass into `CreateScriptStruct`:

```c#
TryCreateScriptStruct("AgePanelSection", 0x30, new List<IFPropertyParams>
{
    _context._toolkitClasses.CreateF32Param("X1", 0),
    _context._toolkitClasses.CreateF32Param("X2", 4),
    _context._toolkitClasses.CreateF32Param("Y1", 8),
    _context._toolkitClasses.CreateF32Param("Y2", 0xc),
    _context._toolkitClasses.CreateF32Param("Field28", 0x28),
});
```

Much like adding new fields, this is viewable in Object XML and blueprints.

## Special Thanks
- UE4SS team, for object dumping reference.
- Rirurin, for object dumping reference.