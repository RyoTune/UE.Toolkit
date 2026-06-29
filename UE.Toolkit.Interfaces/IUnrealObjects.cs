using UE.Toolkit.Core.Common;
using UE.Toolkit.Core.Types.Interfaces;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

// ReSharper disable InconsistentNaming

namespace UE.Toolkit.Interfaces;

/// <summary>
/// API for functionality related to Unreal objects.
/// </summary>
public unsafe interface IUnrealObjects : IObjectCreator
{
    /// <summary>
    /// Notify on the creation of any object.
    /// </summary>
    Action<ToolkitUObject<UObjectBase>>? OnObjectLoaded { get; set; }
    
    /// <summary>
    /// Notify when an object will be destroyed.
    /// </summary>
    Action<ToolkitUObject<UObjectBase>>? OnObjectBeginDestroy { get; set; }
    
    /// <summary>
    /// Gets the global UObject array.
    /// </summary>
    IUObjectArray GUObjectArray { get; }
    
    /// <summary>
    /// Listen for an object's creation of the given name.
    /// </summary>
    /// <param name="objName">Object name.</param>
    /// <param name="callback">Callback given each object instance.</param>
    /// <typeparam name="TObject">Object type.</typeparam>
    /// <remarks>Implemented as a hook on <c>UObject::PostLoadSubobjects</c>, allowing for editing object data before use.</remarks>
    void OnObjectLoadedByName<TObject>(string objName, Action<ToolkitUObject<TObject>> callback)
        where TObject : unmanaged;

    /// <summary>
    /// Listen for an object's creation of the given name.
    /// </summary>
    /// <param name="callback">Callback given each object instance.</param>
    /// <typeparam name="TObject">Object name and type.</typeparam>
    /// <remarks>Implemented as a hook on <c>UObject::PostLoadSubobjects</c>, allowing for editing object data before use.</remarks>
    void OnObjectLoadedByName<TObject>(Action<ToolkitUObject<TObject>> callback)
        where TObject : unmanaged;
    
    /// <summary>
    /// Listen for an object's creation of the given class.
    /// </summary>
    /// <param name="callback">Callback given each object instance.</param>
    /// <typeparam name="TObject">Object class type.</typeparam>
    /// <remarks>Implemented as a post-hook on <c>UObject::PostLoadSubobjects</c>, allowing for editing object data before use.</remarks>
    void OnObjectLoadedByClass<TObject>(Action<ToolkitUObject<TObject>> callback)
        where TObject : unmanaged;
    
    /// <summary>
    /// Listen for an object's creation of the given class.<br/>
    /// Class name should include the expected type prefix: UObjects = U,  AActors = A, Structs = F
    /// </summary>
    /// <param name="objClass">Class name.</param>
    /// <param name="callback">Callback given each object instance.</param>
    /// <typeparam name="TObject">Object type.</typeparam>
    /// <remarks>Implemented as a post-hook on <c>UObject::PostLoadSubobjects</c>, allowing for editing object data before use.</remarks>
    void OnObjectLoadedByClass<TObject>(string objClass, Action<ToolkitUObject<TObject>> callback)
        where TObject : unmanaged;
}

/// <summary>
/// Simple <see cref="UObjectBase"/> wrapper containing the name and instance.
/// </summary>
public unsafe class ToolkitUObject<TObject>(TObject* self) where TObject : unmanaged
{
    /// <summary>
    /// Object instance.
    /// </summary>
    public TObject* Self { get; } = self;

    /// <summary>
    /// Object name.
    /// </summary>
    public string Name { get; } = ToolkitUtils.GetNativeName((nint)self);
}