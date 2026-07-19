using UE.Toolkit.Core.Types.Interfaces;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;

namespace UE.Toolkit.Interfaces;

/// <summary>
/// API for functionality related to Unreal classes.
/// </summary>
public interface IUnrealClasses : IUnrealClassesInternal, ITypeReflection
{
    
    #region Class/Struct Getters
    
    /// <summary>
    /// Get the type information for a specified object. If this class has no type info, this will return null.
    /// This method works any type derived from UObject, which will be any type prefixed with U or A.
    /// </summary>
    /// <param name="Value">Type information for the specified object type.</param>
    /// <typeparam name="TObject">Object type.</typeparam>
    /// <returns>If the type information exists.</returns>
    public bool GetClassInfoFromClass<TObject>(out IUClass? Value) where TObject : unmanaged;
    
    /// <summary>
    /// Get the type information for a specified object. If this class has no type info, this will return null.
    /// This method works any type derived from UObject, which will be any type prefixed with U or A.
    /// </summary>
    /// <param name="Name">Name of the object type.</param>
    /// <param name="Value">Type information for the object type with the given name.</param>
    /// <returns>If the type information exists.</returns>
    public bool GetClassInfoFromName(string Name, out IUClass? Value);
    
    /// <summary>
    /// Get the type information for a specified object. If this struct has no type info, this will return null.
    /// This method works on types that are prefixed with F.
    /// </summary>
    /// <param name="Value">Type information for the specified object type.</param>
    /// <typeparam name="TObject">Object type.</typeparam>
    /// <returns>If the type information exists.</returns>
    public bool GetScriptStructInfoFromType<TObject>(out IUScriptStruct? Value) where TObject : unmanaged;
    
    /// <summary>
    /// Get the type information for a specified object. If this struct has no type info, this will return null.
    /// This method works on types that are prefixed with F.
    /// </summary>
    /// <param name="Name">Name of the object type.</param>
    /// <param name="Value">Type information for the object type with the given name.</param>
    /// <returns>If the type information exists.</returns>
    public bool GetScriptStructInfoFromName(string Name, out IUScriptStruct? Value);
    
    /// <summary>
    /// Get the type information for a specified object. If this enum has no type info, this will return null.
    /// This method works on types that are prefixed with E.
    /// </summary>
    /// <param name="Value">Type information for the specified object type.</param>
    /// <typeparam name="TObject">Object type.</typeparam>
    /// <returns>If the type information exists.</returns>
    public bool GetEnumInfoFromType<TObject>(out IUEnum? Value) where TObject : unmanaged;
    
    /// <summary>
    /// Get the type information for a specified object. If this enum has no type info, this will return null.
    /// This method works on types that are prefixed with E.
    /// </summary>
    /// <param name="Name">Name of the object type.</param>
    /// <param name="Value">Type information for the object type with the given name.</param>
    /// <returns>If the type information exists.</returns>
    public bool GetEnumInfoFromName(string Name, out IUEnum? Value);
    
    #endregion
    
    #region Struct Field Extension Methods (parented to object)
    
    /// <summary>
    /// Listen for the creation of an object's class, then extend it's allocation size and call a custom constructor
    /// after the existing constructor has executed.
    /// </summary>
    /// <param name="ExtraSize">Number of bytes to increase the allocation size of object instances by.</param>
    /// <param name="callback">A custom constructor to call after the original constructor</param>
    /// <typeparam name="TObject">Object type.</typeparam>
    /// <remarks>
    /// The extension is only created for the specific object type. If a struct/class inherits from this class, it
    /// won't receive an extension.
    /// </remarks>
    public void AddExtension<TObject>(uint ExtraSize, Action<ToolkitUObject<TObject>> callback) 
        where TObject: unmanaged;
    
    /// <summary>
    /// Listen for the creation of an object's class, then call a custom constructor after the existing constructor
    /// has executed.
    /// </summary>
    /// <param name="callback">A custom constructor to call after the original constructor</param>
    /// <typeparam name="TObject">Object type.</typeparam>
    public void AddConstructor<TObject>(Action<ToolkitUObject<TObject>> callback)
        where TObject : unmanaged => AddExtension(0, callback);

    /// <summary>
    /// Add an Int8 property to the object's class with the specified name and offset. This will make the field
    /// exposable to blueprints and Object XML.
    /// </summary>
    /// <param name="Name">Name of the new field.</param>
    /// <param name="Offset">Offset of the new field.</param>
    /// <param name="Out">Return value.</param>
    /// <typeparam name="TObject">Object type.</typeparam>
    public bool AddI8Property<TObject>(string Name, int Offset, out IFProperty? Out) where TObject : unmanaged;
    
    /// <summary>
    /// Add a Int16 property to the object's class with the specified name and offset. This will make the field
    /// exposable to blueprints and Object XML.
    /// </summary>
    /// <param name="Name">Name of the new field.</param>
    /// <param name="Offset">Offset of the new field.</param>
    /// <param name="Out">Return value.</param>
    /// <typeparam name="TObject">Object type.</typeparam>
    public bool AddI16Property<TObject>(string Name, int Offset, out IFProperty? Out) where TObject : unmanaged;
    
    /// <summary>
    /// Add a Int32 property to the object's class with the specified name and offset. This will make the field
    /// exposable to blueprints and Object XML.
    /// </summary>
    /// <param name="Name">Name of the new field.</param>
    /// <param name="Offset">Offset of the new field.</param>
    /// <param name="Out">Return value.</param>
    /// <typeparam name="TObject">Object type.</typeparam>
    public bool AddI32Property<TObject>(string Name, int Offset, out IFProperty? Out) where TObject : unmanaged;
    
    /// <summary>
    /// Add a Int64 property to the object's class with the specified name and offset. This will make the field
    /// exposable to blueprints and Object XML.
    /// </summary>
    /// <param name="Name">Name of the new field.</param>
    /// <param name="Offset">Offset of the new field.</param>
    /// <param name="Out">Return value.</param>
    /// <typeparam name="TObject">Object type.</typeparam>
    public bool AddI64Property<TObject>(string Name, int Offset, out IFProperty? Out) where TObject : unmanaged;
    
    /// <summary>
    /// Add a UInt8 property to the object's class with the specified name and offset. This will make the field
    /// exposable to blueprints and Object XML.
    /// </summary>
    /// <param name="Name">Name of the new field.</param>
    /// <param name="Offset">Offset of the new field.</param>
    /// <param name="Out">Return value.</param>
    /// <typeparam name="TObject">Object type.</typeparam>
    public bool AddU8Property<TObject>(string Name, int Offset, out IFProperty? Out) where TObject : unmanaged;
    
    /// <summary>
    /// Add a UInt16 property to the object's class with the specified name and offset. This will make the field
    /// exposable to blueprints and Object XML.
    /// </summary>
    /// <param name="Name">Name of the new field.</param>
    /// <param name="Offset">Offset of the new field.</param>
    /// <param name="Out">Return value.</param>
    /// <typeparam name="TObject">Object type.</typeparam>
    public bool AddU16Property<TObject>(string Name, int Offset, out IFProperty? Out) where TObject : unmanaged;
    
    /// <summary>
    /// Add a UInt32 property to the object's class with the specified name and offset. This will make the field
    /// exposable to blueprints and Object XML.
    /// </summary>
    /// <param name="Name">Name of the new field.</param>
    /// <param name="Offset">Offset of the new field.</param>
    /// <param name="Out">Return value.</param>
    /// <typeparam name="TObject">Object type.</typeparam>
    public bool AddU32Property<TObject>(string Name, int Offset, out IFProperty? Out) where TObject : unmanaged;
    
    /// <summary>
    /// Add a UInt64 property to the object's class with the specified name and offset. This will make the field
    /// exposable to blueprints and Object XML.
    /// </summary>
    /// <param name="Name">Name of the new field.</param>
    /// <param name="Offset">Offset of the new field.</param>
    /// <param name="Out">Return value.</param>
    /// <typeparam name="TObject">Object type.</typeparam>
    public bool AddU64Property<TObject>(string Name, int Offset, out IFProperty? Out) where TObject : unmanaged;
    
    /// <summary>
    /// Add a float property to the object's class with the specified name and offset. This will make the field
    /// exposable to blueprints and Object XML.
    /// </summary>
    /// <param name="Name">Name of the new field.</param>
    /// <param name="Offset">Offset of the new field.</param>
    /// <param name="Out">Return value.</param>
    /// <typeparam name="TObject">Object type.</typeparam>
    public bool AddF32Property<TObject>(string Name, int Offset, out IFProperty? Out) where TObject : unmanaged;
    
    /// <summary>
    /// Add a double property to the object's class with the specified name and offset. This will make the field
    /// exposable to blueprints and Object XML.
    /// </summary>
    /// <param name="Name">Name of the new field.</param>
    /// <param name="Offset">Offset of the new field.</param>
    /// <param name="Out">Return value.</param>
    /// <typeparam name="TObject">Object type.</typeparam>
    public bool AddF64Property<TObject>(string Name, int Offset, out IFProperty? Out) where TObject : unmanaged;

    /// <summary>
    /// Add a C-style boolean to the object's class with the specified name and offset. This will make the field
    /// exposable to blueprints and Object XML.
    /// </summary>
    /// <param name="Name">Name of the new field.</param>
    /// <param name="Offset">Offset of the new field.</param>
    /// <param name="Out">Return value.</param>
    /// <typeparam name="TObject">Object type.</typeparam>
    public bool AddCBoolProperty<TObject>(string Name, int Offset, out IFBoolProperty? Out) where TObject : unmanaged;
    
    /// <summary>
    /// Add a bitflag-style boolean to the object's class with the specified name and offset. This will make the field
    /// exposable to blueprints and Object XML.
    /// </summary>
    /// <param name="Name">Name of the new field.</param>
    /// <param name="Offset">Byte offset of the new field.</param>
    /// <param name="Bit">Bit offset of the new field.</param>
    /// <param name="Out">Return value.</param>
    /// <typeparam name="TObject">Object type.</typeparam>
    public bool AddBitBoolProperty<TObject>(string Name, int Offset, int Bit, out IFBoolProperty? Out) where TObject : unmanaged;

    /// <summary>
    /// Add a by-value struct to the object's class with the specified name and offset. This will make the field
    /// exposable to blueprints and Object XML.
    /// </summary>
    /// <param name="Name">Name of the new field.</param>
    /// <param name="Offset">Offset of the new field.</param>
    /// <param name="Out">Return value.</param>
    /// <typeparam name="TObject">Object type.</typeparam>
    /// <typeparam name="TField">Field type.</typeparam>
    public bool AddStructProperty<TObject, TField>(string Name, int Offset, out IFStructProperty? Out)
        where TObject : unmanaged
        where TField : unmanaged;
    
    /// <summary>
    /// Add a by-reference class in the form of a raw pointer to the object's class with the specified name and offset.
    /// This will make the field exposable to blueprints and Object XML.
    /// </summary>
    /// <param name="Name">Name of the new field.</param>
    /// <param name="Offset">Offset of the new field.</param>
    /// <param name="Out">Return value.</param>
    /// <typeparam name="TObject">Object type.</typeparam>
    /// <typeparam name="TField">Field type.</typeparam>
    public bool AddObjectProperty<TObject, TField>(string Name, int Offset, out IFObjectProperty? Out)
        where TObject : unmanaged
        where TField : unmanaged;
    
    /// <summary>
    /// Add a by-reference class in the form of a type-safe pointer to the object's class with the specified name and offset.
    /// This will make the field exposable to blueprints and Object XML.
    /// </summary>
    /// <param name="Name">Name of the new field.</param>
    /// <param name="Offset">Offset of the new field.</param>
    /// <param name="Out">Return value.</param>
    /// <typeparam name="TObject">Object type.</typeparam>
    /// <typeparam name="TField">Field type.</typeparam>
    public bool AddClassProperty<TObject, TField>(string Name, int Offset, out IFClassProperty? Out)
        where TObject : unmanaged
        where TField : unmanaged;
    
    /// <summary>
    /// Add a FName to the object's class with the specified name and offset. This will make the field
    /// exposable to blueprints and Object XML.
    /// </summary>
    /// <param name="Name">Name of the new field.</param>
    /// <param name="Offset">Offset of the new field.</param>
    /// <param name="Out">Return value.</param>
    /// <typeparam name="TObject">Object type.</typeparam>
    public bool AddNameProperty<TObject>(string Name, int Offset, out IFProperty? Out) where TObject : unmanaged;
    
    /// <summary>
    /// Add a FString to the object's class with the specified name and offset. This will make the field
    /// exposable to blueprints and Object XML.
    /// </summary>
    /// <param name="Name">Name of the new field.</param>
    /// <param name="Offset">Offset of the new field.</param>
    /// <param name="Out">Return value.</param>
    /// <typeparam name="TObject">Object type.</typeparam>
    public bool AddStringProperty<TObject>(string Name, int Offset, out IFProperty? Out) where TObject : unmanaged;
    
    /// <summary>
    /// Add a FText to the object's class with the specified name and offset. This will make the field
    /// exposable to blueprints and Object XML.
    /// </summary>
    /// <param name="Name">Name of the new field.</param>
    /// <param name="Offset">Offset of the new field.</param>
    /// <param name="Out">Return value.</param>
    /// <typeparam name="TObject">Object type.</typeparam>
    public bool AddTextProperty<TObject>(string Name, int Offset, out IFProperty? Out) where TObject : unmanaged;
    
    /// <summary>
    /// Add a Array (TArray) containing elements of the property defined in Inner to the object's class with the
    /// specified name and offset. This will make the field exposable to blueprints and Object XML.
    /// </summary>
    /// <param name="Name">Name of the new field.</param>
    /// <param name="Offset">Offset of the new field.</param>
    /// <param name="Inner">The inner property used for each entry in the array.</param>
    /// <param name="Property">Return value.</param>
    /// <typeparam name="TObject">Object type.</typeparam>
    public bool AddArrayProperty<TObject>(string Name, int Offset, IFProperty Inner, out IFArrayProperty? Property) 
        where TObject : unmanaged;
    
    /*
    /// <summary>
    /// Add a map (TMap) with it's key and value types defined in Key and Value, and with the
    /// specified name and offset. This will make the field exposable to blueprints and Object XML.
    /// </summary>
    /// <param name="Name">Name of the new field.</param>
    /// <param name="Offset">Offset of the new field.</param>
    /// <param name="Key">The property used for each key in the map.</param>
    /// <param name="Value">The property used for each value in the map.</param>
    /// <param name="Property">Return value.</param>
    /// <typeparam name="TObject">Object type.</typeparam>
    public bool AddMapProperty<TObject>(string Name, int Offset,
        IFProperty Key, IFProperty Value, out IFArrayProperty? Property) where TObject : unmanaged;
    */
    
    #endregion
    
    #region Struct Field Extension Methods (no parent)
    
    /// <summary>
    /// Add an Int8 property to the object's class with the specified name and offset. This will make the field
    /// exposable to blueprints and Object XML.
    /// </summary>
    /// <param name="Name">Name of the new field.</param>
    /// <param name="Offset">Offset of the new field.</param>
    /// <param name="Out">Return value.</param>
    public bool AddI8Property(string Name, int Offset, out IFProperty? Out) ;
    
    /// <summary>
    /// Add a Int16 property to the object's class with the specified name and offset. This will make the field
    /// exposable to blueprints and Object XML.
    /// </summary>
    /// <param name="Name">Name of the new field.</param>
    /// <param name="Offset">Offset of the new field.</param>
    /// <param name="Out">Return value.</param>
    public bool AddI16Property(string Name, int Offset, out IFProperty? Out) ;
    
    /// <summary>
    /// Add a Int32 property to the object's class with the specified name and offset. This will make the field
    /// exposable to blueprints and Object XML.
    /// </summary>
    /// <param name="Name">Name of the new field.</param>
    /// <param name="Offset">Offset of the new field.</param>
    /// <param name="Out">Return value.</param>
    public bool AddI32Property(string Name, int Offset, out IFProperty? Out) ;
    
    /// <summary>
    /// Add a Int64 property to the object's class with the specified name and offset. This will make the field
    /// exposable to blueprints and Object XML.
    /// </summary>
    /// <param name="Name">Name of the new field.</param>
    /// <param name="Offset">Offset of the new field.</param>
    /// <param name="Out">Return value.</param>
    public bool AddI64Property(string Name, int Offset, out IFProperty? Out) ;
    
    /// <summary>
    /// Add a UInt8 property to the object's class with the specified name and offset. This will make the field
    /// exposable to blueprints and Object XML.
    /// </summary>
    /// <param name="Name">Name of the new field.</param>
    /// <param name="Offset">Offset of the new field.</param>
    /// <param name="Out">Return value.</param>
    public bool AddU8Property(string Name, int Offset, out IFProperty? Out) ;
    
    /// <summary>
    /// Add a UInt16 property to the object's class with the specified name and offset. This will make the field
    /// exposable to blueprints and Object XML.
    /// </summary>
    /// <param name="Name">Name of the new field.</param>
    /// <param name="Offset">Offset of the new field.</param>
    /// <param name="Out">Return value.</param>
    public bool AddU16Property(string Name, int Offset, out IFProperty? Out) ;
    
    /// <summary>
    /// Add a UInt32 property to the object's class with the specified name and offset. This will make the field
    /// exposable to blueprints and Object XML.
    /// </summary>
    /// <param name="Name">Name of the new field.</param>
    /// <param name="Offset">Offset of the new field.</param>
    /// <param name="Out">Return value.</param>
    public bool AddU32Property(string Name, int Offset, out IFProperty? Out) ;
    
    /// <summary>
    /// Add a UInt64 property to the object's class with the specified name and offset. This will make the field
    /// exposable to blueprints and Object XML.
    /// </summary>
    /// <param name="Name">Name of the new field.</param>
    /// <param name="Offset">Offset of the new field.</param>
    /// <param name="Out">Return value.</param>
    public bool AddU64Property(string Name, int Offset, out IFProperty? Out) ;
    
    /// <summary>
    /// Add a float property to the object's class with the specified name and offset. This will make the field
    /// exposable to blueprints and Object XML.
    /// </summary>
    /// <param name="Name">Name of the new field.</param>
    /// <param name="Offset">Offset of the new field.</param>
    /// <param name="Out">Return value.</param>
    public bool AddF32Property(string Name, int Offset, out IFProperty? Out) ;
    
    /// <summary>
    /// Add a double property to the object's class with the specified name and offset. This will make the field
    /// exposable to blueprints and Object XML.
    /// </summary>
    /// <param name="Name">Name of the new field.</param>
    /// <param name="Offset">Offset of the new field.</param>
    /// <param name="Out">Return value.</param>
    public bool AddF64Property(string Name, int Offset, out IFProperty? Out) ;
    
    /// <summary>
    /// Add a by-value struct to the object's class with the specified name and offset. This will make the field
    /// exposable to blueprints and Object XML.
    /// </summary>
    /// <param name="Name">Name of the new field.</param>
    /// <param name="Offset">Offset of the new field.</param>
    /// <param name="Out">Return value.</param>
    /// <typeparam name="TField">Field type.</typeparam>
    public bool AddStructProperty<TField>(string Name, int Offset, out IFStructProperty? Out)
        where TField : unmanaged;
    
    /// <summary>
    /// Add a by-value struct to the object's class with the specified name and offset. This will make the field
    /// exposable to blueprints and Object XML.
    /// </summary>
    /// <param name="Name">Name of the new field.</param>
    /// <param name="TypeName">Field type.</param>
    /// <param name="Offset">Offset of the new field.</param>
    /// <param name="Out">Return value.</param>
    public bool AddStructProperty(string Name, string TypeName, int Offset, out IFStructProperty? Out);
    
    /// <summary>
    /// Add a by-reference struct to the object's class with the specified name and offset. This will make the field
    /// exposable to blueprints and Object XML.
    /// This is constructed as an ObjectProperty, but with the object type set to a UScriptStruct instead of a UClass
    /// to make the underlying table in a DataTable viewable by the type reflection system (big hack, but needed for
    /// data table debugging tools).
    /// </summary>
    /// <param name="Name">Name of the new field.</param>
    /// <param name="TypeName">Field type.</param>
    /// <param name="Offset">Offset of the new field.</param>
    /// <param name="Out">Return value.</param>
    public bool AddStructProperty_DataTableSpecial(string Name, string TypeName, int Offset, out IFObjectProperty? Out);
    
    /// <summary>
    /// Add a FName to the object's class with the specified name and offset. This will make the field
    /// exposable to blueprints and Object XML.
    /// </summary>
    /// <param name="Name">Name of the new field.</param>
    /// <param name="Offset">Offset of the new field.</param>
    /// <param name="Out">Return value.</param>
    public bool AddNameProperty(string Name, int Offset, out IFProperty? Out) ;
    
    /// <summary>
    /// Add a FString to the object's class with the specified name and offset. This will make the field
    /// exposable to blueprints and Object XML.
    /// </summary>
    /// <param name="Name">Name of the new field.</param>
    /// <param name="Offset">Offset of the new field.</param>
    /// <param name="Out">Return value.</param>
    public bool AddStringProperty(string Name, int Offset, out IFProperty? Out) ;
    
    /// <summary>
    /// Add a FText to the object's class with the specified name and offset. This will make the field
    /// exposable to blueprints and Object XML.
    /// </summary>
    /// <param name="Name">Name of the new field.</param>
    /// <param name="Offset">Offset of the new field.</param>
    /// <param name="Out">Return value.</param>
    public bool AddTextProperty(string Name, int Offset, out IFProperty? Out) ;
    
    
    /// <summary>
    /// Add a map (TMap) with it's key and value types defined in Key and Value, and with the
    /// specified name and offset. This will make the field exposable to blueprints and Object XML.
    /// </summary>
    /// <param name="Name">Name of the new field.</param>
    /// <param name="Offset">Offset of the new field.</param>
    /// <param name="Key">The property used for each key in the map.</param>
    /// <param name="Value">The property used for each value in the map.</param>
    /// <param name="Property">Return value.</param>
    public bool AddMapProperty(string Name, int Offset,
        IFProperty Key, IFProperty Value, out IFMapProperty? Property);
    #endregion
    
    #region New Struct Construction

    /// <summary>
    /// Get the package object for the engine "/Script/Engine"
    /// </summary>
    /// <returns>The package object for the engine if it's been initialized.</returns>
    public IUObject? GetEnginePackage();
    
    /// <summary>
    /// Get the package object for the game
    /// </summary>
    /// <returns>The package object for the game if it's been initialized.</returns>
    public IUObject? GetGamePackage();

    /// <summary>
    /// Create a Int8 param to insert into a struct param's property list. This is used when registering new
    /// struct types to the engine at runtime.
    /// </summary>
    /// <param name="Name">Param's name.</param>
    /// <param name="Offset">Param's offset.</param>
    public IFGenericPropertyParams? CreateI8Param(string Name, int Offset);
    
    /// <summary>
    /// Create a Int16 param to insert into a struct param's property list. This is used when registering new
    /// struct types to the engine at runtime.
    /// </summary>
    /// <param name="Name">Param's name.</param>
    /// <param name="Offset">Param's offset.</param>
    public IFGenericPropertyParams? CreateI16Param(string Name, int Offset);
    
    /// <summary>
    /// Create a Int32 param to insert into a struct param's property list. This is used when registering new
    /// struct types to the engine at runtime.
    /// </summary>
    /// <param name="Name">Param's name.</param>
    /// <param name="Offset">Param's offset.</param>
    public IFGenericPropertyParams? CreateI32Param(string Name, int Offset);
    
    /// <summary>
    /// Create a Int64 param to insert into a struct param's property list. This is used when registering new
    /// struct types to the engine at runtime.
    /// </summary>
    /// <param name="Name">Param's name.</param>
    /// <param name="Offset">Param's offset.</param>
    public IFGenericPropertyParams? CreateI64Param(string Name, int Offset);
    
    /// <summary>
    /// Create a UInt8 param to insert into a struct param's property list. This is used when registering new
    /// struct types to the engine at runtime.
    /// </summary>
    /// <param name="Name">Param's name.</param>
    /// <param name="Offset">Param's offset.</param>
    public IFGenericPropertyParams? CreateU8Param(string Name, int Offset);
    
    /// <summary>
    /// Create a UInt16 param to insert into a struct param's property list. This is used when registering new
    /// struct types to the engine at runtime.
    /// </summary>
    /// <param name="Name">Param's name.</param>
    /// <param name="Offset">Param's offset.</param>
    public IFGenericPropertyParams? CreateU16Param(string Name, int Offset);
    
    /// <summary>
    /// Create a UInt32 param to insert into a struct param's property list. This is used when registering new
    /// struct types to the engine at runtime.
    /// </summary>
    /// <param name="Name">Param's name.</param>
    /// <param name="Offset">Param's offset.</param>
    public IFGenericPropertyParams? CreateU32Param(string Name, int Offset);
    
    /// <summary>
    /// Create a UInt64 param to insert into a struct param's property list. This is used when registering new
    /// struct types to the engine at runtime.
    /// </summary>
    /// <param name="Name">Param's name.</param>
    /// <param name="Offset">Param's offset.</param>
    public IFGenericPropertyParams? CreateU64Param(string Name, int Offset);
    
    /// <summary>
    /// Create a float param to insert into a struct param's property list. This is used when registering new
    /// struct types to the engine at runtime.
    /// </summary>
    /// <param name="Name">Param's name.</param>
    /// <param name="Offset">Param's offset.</param>
    public IFGenericPropertyParams? CreateF32Param(string Name, int Offset);
    
    /// <summary>
    /// Create a double param to insert into a struct param's property list. This is used when registering new
    /// struct types to the engine at runtime.
    /// </summary>
    /// <param name="Name">Param's name.</param>
    /// <param name="Offset">Param's offset.</param>
    public IFGenericPropertyParams? CreateF64Param(string Name, int Offset);

    /// <summary>
    /// Create a new UE struct definition at runtime which can be used by Blueprints and Object XML. This can be used
    /// to annotate instances of another struct with an inner type.
    /// </summary>
    /// <param name="Name">Name of the new struct.</param>
    /// <param name="Size">Size of the new struct.</param>
    /// <param name="Fields">A list of exposed fields within this struct.</param>
    /// <param name="Out">New script struct.</param>
    public bool CreateScriptStruct(string Name, int Size, List<IFPropertyParams> Fields, out IUScriptStruct? Out);

    #endregion
}