using System.Numerics;
using UE.Toolkit.Core.Types.Unreal.Factories;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE4_27_2;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;
using UE.Toolkit.Interfaces;
// using UE.Toolkit.Reloaded.Common.GameConfigs;

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
        var SpawnParams = Factory.CreateFActorSpawnParameters();
        SpawnParams.SetParams(EObjectFlags.RF_Transactional);
        unsafe
        {
            var pTransform = (FTransform*)Factory.Memory!.Malloc(sizeof(FTransform), 16);
            pTransform->Rotation = Transform.Rotation;
            pTransform->Position = Transform.Position;
            pTransform->Scale3D = Transform.Scale3D;
            var Result = _SpawnActor.Wrapper(World.Ptr, Class.Ptr, (nint)pTransform, SpawnParams.Ptr);
            Factory.Memory!.Free((nint)pTransform);
            return Result != nint.Zero ? Factory.CreateUObject(Result) : null;   
        }
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