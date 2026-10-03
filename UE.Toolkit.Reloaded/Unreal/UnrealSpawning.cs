using System.Numerics;
using UE.Toolkit.Core.Types.Unreal.Factories;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;
using UE.Toolkit.Interfaces;
using UE.Toolkit.Reloaded.Common.GameConfigs;
// using UE.Toolkit.Reloaded.Common.GameConfigs;
using FTransform = UE.Toolkit.Core.Types.Unreal.UE5_4_4.FTransform;

namespace UE.Toolkit.Reloaded.Unreal;

public class UnrealSpawning : IUnrealSpawning
{
    private IUnrealClasses Classes;
    private IUnrealFactory Factory;
    private IUnrealState State;

    private delegate nint StaticConstructObject_Internal(nint pParams);

    private SHFunction<StaticConstructObject_Internal> _StaticConstructObjectInternal;

    private delegate nint UWorld_SpawnActor(nint pSelf, nint pActorClass, nint pTransform, nint spawnParams);

    private SHFunction<UWorld_SpawnActor> _SpawnActor; 
    
    public IUObject? SpawnObject<TObject>(string Name, IUObject? Owner) where TObject : unmanaged
        => Classes.GetClassInfoFromClass<TObject>(out var Class) ? SpawnObject(Name, Class, Owner) : null;

    public IUObject? SpawnObject(string Name, IUClass Class, IUObject? Owner)
    {
        var Params = Factory.CreateFStaticConstructObjectParameters();
        Params.SetParams(Class, Owner, new FName(Name));
        return Factory.CreateUObject(_StaticConstructObjectInternal.Wrapper(Params.Ptr));
    }
    
    public IUObject? SpawnActor<TObject>(string Name) where TObject : unmanaged
        => Classes.GetClassInfoFromClass<TObject>(out var Class) ? SpawnActor(Name, Class) : null;

    public IUObject? SpawnActor(string Name, IUClass Class)
        => State.GetCurrentPlayWorld(out var World) ? SpawnActor(Name, Class, World) : null;
    
    public IUObject? SpawnActor<TObject>(string Name, FTransform Transform) where TObject : unmanaged
        => Classes.GetClassInfoFromClass<TObject>(out var Class) ? SpawnActor(Name, Class, Transform) : null;

    public IUObject? SpawnActor(string Name, IUClass Class, FTransform Transform)
        => State.GetCurrentPlayWorld(out var World) ? SpawnActor(Name, Class, World, Transform) : null;

    public IUObject? SpawnActor<TObject>(string Name, IUObject World) where TObject : unmanaged
        => Classes.GetClassInfoFromClass<TObject>(out var Class) ? SpawnActor(Name, Class, World) : null;

    public IUObject? SpawnActor(string Name, IUClass Class, IUObject World)
        => SpawnActor(Name, Class, World, new());
    
    public IUObject? SpawnActor<TObject>(string Name, IUObject World, FTransform Transform) where TObject : unmanaged
        => Classes.GetClassInfoFromClass<TObject>(out var Class) ? SpawnActor(Name, Class, World, Transform) : null;

    public IUObject? SpawnActor(string Name, IUClass Class, IUObject World, FTransform Transform)
    {
        var spawnParams = Factory.CreateFActorSpawnParameters();
        spawnParams.SetParams(new(Name), EObjectFlags.RF_Transactional);
        var pTransform = GameConfig.Instance.CreateTransform(Transform);
        var result = _SpawnActor.Wrapper(World.Ptr, Class.Ptr, pTransform.Ptr, spawnParams.Ptr);
        Factory.Memory!.Free(pTransform.Ptr);
        return result != nint.Zero ? Factory.CreateUObject(result) : null;   
    }

    public UnrealSpawning(IUnrealClasses classes, IUnrealFactory factory, IUnrealState state)
    {
        Classes = classes;
        Factory = factory;
        State = state;

        _SpawnActor = new();
        _StaticConstructObjectInternal = new();
    }
}