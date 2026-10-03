using System.Collections;
using UE.Toolkit.Core.Types.Interfaces;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.Factories.UE4_27_2;
using UE.Toolkit.Core.Types.Unreal.Factories.UE5_2_1;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;
using FWorldContext = UE.Toolkit.Core.Types.Unreal.UE5_0_3.FWorldContext;
using UClass = UE.Toolkit.Core.Types.Unreal.UE5_0_3.UClass;
using UFunction = UE.Toolkit.Core.Types.Unreal.UE4_27_2.UFunction;
using UEngine = UE.Toolkit.Core.Types.Unreal.UE5_0_3.UEngine;

namespace UE.Toolkit.Core.Types.Unreal.Factories.UE5_0_3;

public class UnrealFactory : UE.Toolkit.Core.Types.Unreal.Factories.UE5_2_1.UnrealFactory
{
    public override IFWorldContext CreateFWorldContext(nint ptr) => new FWorldContext_UE5_0_3(ptr, this);
    public override IUClass CreateUClass(nint ptr) => new UClass_UE5_0_3(ptr, this, Memory);
    public override IICppStructOps CreateICppStructOps(nint ptr) => new ICppStructOpsUE4_27_2(ptr, Memory);
    public override IUEngine CreateUEngine(nint ptr) => new UEngine_UE5_0_3(ptr, this, Memory);
}

public unsafe class UClass_UE5_0_3(nint ptr, IUnrealFactory factory, IUnrealMemoryInternal memory)
    : UStruct_UE5_2_1(ptr, factory, memory), IUClass
{
    private readonly UClass* _self = (UClass*)ptr;

    public IUClass? GetSuperClass()
        => _self->_super.super_struct != null ? _factory.CreateUClass((nint)_self->_super.super_struct) : null;
    
    public IUFunction? GetFunction(string Name)
    {
        var FuncMapDict = new TMapDictionary<FName, Ptr<UFunction>>(
            (TMap<FName, Ptr<UFunction>>*)(&_self->func_map), factory.Memory
        );
        return FuncMapDict.TryGetValue(new(Name), out var Function)
            ? factory.CreateUFunction((nint)Function.Value->Value)
            : null;
    }
    
    public IEnumerable<IUFunction> GetFunctions()
    {
        var FuncMapDict = new TMapDictionary<FName, Ptr<UFunction>>(
            (TMap<FName, Ptr<UFunction>>*)(&_self->func_map), _factory.Memory
        );
        return FuncMapDict.Values.Select(x => _factory.CreateUFunction((nint)x.Value->Value));
    }
    
    public IUObject? ClassDefaultObject 
        => _self->class_default_obj != null ? factory.CreateUObject((nint)_self->class_default_obj) : null;

    public nint Constructor => _self->class_ctor;
    public EClassFlags ClassFlags => _self->class_flags;
    public EClassCastFlags ClassCastFlags => _self->class_cast_flags;
}

public unsafe class FWorldContext_UE5_0_3(nint ptr, IUnrealFactory factory) : IFWorldContext
{
    protected readonly IUnrealFactory _factory = factory;
    private readonly FWorldContext* _self = (FWorldContext*)ptr;
    public nint Ptr => (nint)_self;
    public WorldType GetWorldType() => _self->WorldType;
    public nint GetWorld() => (nint)_self->ThisCurrentWorld;
}

public unsafe class FWorldContextEnumerator(UEngine_UE5_0_3 owner, IUnrealFactory factory) 
    : IEnumerator<IFWorldContext>, IEnumerable<IFWorldContext>
{
    private int CurrentIndex = -1;
    
    #region impl IEnumerator 
    
    public bool MoveNext() => ++CurrentIndex < owner.GetWorldListInner()->ArrayNum;

    public void Reset() => CurrentIndex = -1;

    public IFWorldContext Current => factory.CreateFWorldContext((nint)owner.GetWorldListInner()->AllocatorInstance[CurrentIndex].Value);

    object? IEnumerator.Current => Current;

    public void Dispose() {}
    
    #endregion
    
    #region impl IEnumerable
    
    public IEnumerator<IFWorldContext> GetEnumerator() => this;
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    #endregion
}

public unsafe class UEngine_UE5_0_3(nint ptr, IUnrealFactory factory, IUnrealMemoryInternal memory) 
    : UObjectUE4_27_2(ptr, factory, memory), IUEngine
{
    private readonly UEngine* _self = (UEngine*)ptr;

    internal TArray<Ptr<FWorldContext>>* GetWorldListInner() => &_self->WorldList;
    
    public IEnumerable<IFWorldContext> GetWorldList() => new FWorldContextEnumerator(this, factory);
}