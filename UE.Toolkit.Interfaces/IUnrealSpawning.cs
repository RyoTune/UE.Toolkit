using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;

namespace UE.Toolkit.Interfaces;

/// <summary>
/// API for spawning new objects.
/// </summary>
public interface IUnrealSpawning
{
    /// <summary>
    /// Spawns an object with the given name (and optionally an owner)
    /// </summary>
    /// <param name="Name">Name of the object</param>
    /// <param name="Owner">The object's owner. This will bind the lifetime of this object to it's parent.</param>
    /// <typeparam name="TObject">Object type</typeparam>
    /// <returns>The new object if it was successfully created</returns>
    IUObject? SpawnObject<TObject>(string Name, IUObject? Owner) where TObject : unmanaged;
    
    /// <summary>
    /// Spawns an object with the given name and class (and optionally an owner)
    /// </summary>
    /// <param name="Name">Name of the object</param>
    /// <param name="Class">Object class</param>
    /// <param name="Owner">The object's owner. This will bind the lifetime of this object to it's parent.</param>
    /// <returns>The new object if it was successfully created</returns>
    IUObject? SpawnObject(string Name, IUClass Class, IUObject? Owner);
    
    /*

    IUObject? SpawnActor<TObject>(string Name) where TObject : unmanaged;

    IUObject? SpawnActor(string Name, IUClass Class);
    
    IUObject? SpawnActor<TObject>(string Name, IUObject World) where TObject : unmanaged;

    IUObject? SpawnActor(string Name, IUClass Class, IUObject World);
    
    */
}