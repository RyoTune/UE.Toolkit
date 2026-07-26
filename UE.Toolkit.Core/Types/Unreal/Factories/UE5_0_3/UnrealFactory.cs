using UE.Toolkit.Core.Types.Interfaces;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.Factories.UE5_2_1;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;
using UClass = UE.Toolkit.Core.Types.Unreal.UE5_0_3.UClass;
using UFunction = UE.Toolkit.Core.Types.Unreal.UE4_27_2.UFunction;

namespace UE.Toolkit.Core.Types.Unreal.Factories.UE5_0_3;

public class UnrealFactory : UE.Toolkit.Core.Types.Unreal.Factories.UE5_2_1.UnrealFactory
{
    public override IUClass CreateUClass(nint ptr) => new UClass_UE5_0_3(ptr, this, Memory);
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