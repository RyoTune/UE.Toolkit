using System.Runtime.InteropServices;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.Factories.UE5_4_4;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

using FProperty = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FProperty;
using FByteProperty = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FByteProperty;
using FBoolProperty = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FBoolProperty;
using FEnumProperty = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FEnumProperty;
using FObjectProperty = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FObjectProperty;
using FClassProperty = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FClassProperty;
using FStructProperty = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FStructProperty;

using FMapProperty = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FMapProperty;
// using FInterfaceProperty = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FInterfaceProperty;
using FArrayProperty = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FArrayProperty;
using FSetProperty = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FSetProperty;
// using FOptionalProperty = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FOptionalProperty;
using FDelegateProperty = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FDelegateProperty;

using FPropertyParamsBase = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FPropertyParamsBase;
using FStructParams = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FStructParams;

namespace UE.Toolkit.Core.Types.Unreal.Factories.UE5_2_1;

public class UnrealFactory : BaseUnrealFactory
{
    
    public override nint SizeOf<T>()
    {
        var TypeName = typeof(T).Name;
        unsafe
        {
            return TypeName switch
            {
                nameof(IUObject) => sizeof(UObjectBase),
                nameof(IUClass) => sizeof(UClass),
                nameof(IUScriptStruct) => sizeof(UScriptStruct),
                nameof(IUEnum) => sizeof(UEnum),
                nameof(IUUserDefinedEnum) => sizeof(UUserDefinedEnum),
                nameof(IFProperty) => sizeof(FProperty),
                nameof(IFByteProperty) => sizeof(FByteProperty),
                nameof(IFBoolProperty) => sizeof(FBoolProperty),
                nameof(IFEnumProperty) => sizeof(FEnumProperty),
                nameof(IFObjectProperty) => sizeof(FObjectProperty),
                nameof(IFSoftClassProperty) => sizeof(FSoftClassProperty),
                nameof(IFClassProperty) => sizeof(FClassProperty),
                nameof(IFStructProperty) => sizeof(FStructProperty),
                nameof(IFMapProperty) => sizeof(FMapProperty),
                nameof(IFInterfaceProperty) => sizeof(FInterfaceProperty),
                nameof(IFArrayProperty) => sizeof(FArrayProperty),
                nameof(IFSetProperty) => sizeof(FSetProperty),
                nameof(IFOptionalProperty) => sizeof(FOptionalProperty),
                nameof(IFDelegateProperty) => sizeof(FDelegateProperty),
                _ => throw new NotSupportedException(TypeName)
            };
        }
    }
    
    public override IFProperty CreateFProperty(nint ptr) => new UE4_27_2.FPropertyUE4_27_2(ptr, this);
    public override IFByteProperty CreateFByteProperty(nint ptr) => new UE4_27_2.FBytePropertyUE4_27_2(ptr, this);
    public override IFBoolProperty CreateFBoolProperty(nint ptr) => new UE4_27_2.FBoolPropertyUE4_27_2(ptr, this);
    public override IFEnumProperty CreateFEnumProperty(nint ptr) => new UE4_27_2.FEnumPropertyUE4_27_2(ptr, this);
    public override IFObjectProperty CreateFObjectProperty(nint ptr) => new UE4_27_2.FObjectPropertyUE4_27_2(ptr, this);
    public override IFSoftClassProperty CreateFSoftClassProperty(nint ptr) => new UE4_27_2.FSoftClassPropertyUE4_27_2(ptr, this);
    public override IFClassProperty CreateFClassProperty(nint ptr) => new UE4_27_2.FClassPropertyUE4_27_2(ptr, this);
    public override IFStructProperty CreateFStructProperty(nint ptr) => new UE4_27_2.FStructPropertyUE4_27_2(ptr, this);
    public override IFMapProperty CreateFMapProperty(nint ptr) => new UE4_27_2.FMapPropertyUE4_27_2(ptr, this);
    public override IFInterfaceProperty CreateFInterfaceProperty(nint ptr) => new UE4_27_2.FInterfacePropertyUE4_27_2(ptr, this);
    public override IFArrayProperty CreateFArrayProperty(nint ptr) => new UE4_27_2.FArrayPropertyUE4_27_2(ptr, this);
    public override IFSetProperty CreateFSetProperty(nint ptr) => new UE4_27_2.FSetPropertyUE4_27_2(ptr, this);
    // public override IFOptionalProperty CreateFOptionalProperty(nint ptr) => new UE4_27_2.FOptionalPropertyUE4_27_2(ptr, this);
    public override IFOptionalProperty CreateFOptionalProperty(nint ptr) => throw new NotSupportedException();
    public override IFDelegateProperty CreateFDelegateProperty(nint ptr) => new UE4_27_2.FDelegatePropertyUE4_27_2(ptr, this);
    public override IUObjectArray CreateUObjectArray(nint ptr) => new UObjectArray_UE5_4_4(ptr, this);
    public override IUObject CreateUObject(nint ptr) => new UObject_UE5_4_4(ptr, this);
    public override IUClass CreateUClass(nint ptr) => new UClass_UE5_2_1(ptr, this);
    public override IUScriptStruct CreateUScriptStruct(nint ptr) => new UScriptStruct_UE5_2_1(ptr, this);
    public override IUEnum CreateUEnum(nint ptr) => new UEnum_UE5_4_4(ptr, this);
    public override IUField CreateUField(nint ptr) => new UField_UE5_4_4(ptr, this);
    public override IUStruct CreateUStruct(nint ptr) => new UStruct_UE5_2_1(ptr, this);
    public override IUUserDefinedEnum CreateUUserDefinedEnum(nint ptr) => new UUserDefinedEnum_UE5_4_4(ptr, this);
    public override IUFunction CreateUFunction(nint ptr) => new UFunction_UE5_4_4(ptr, this);
    public override IFFieldClass CreateFFieldClass(nint ptr) => new FFieldClass_UE5_4_4(ptr, this);
    public override IFField CreateFField(nint ptr) => new FField_UE5_4_4(ptr, this);
    public override IFFieldVariant CreateFFieldVariant(nint ptr) => new UE4_27_2.FFieldVariantUE4_27_2(ptr, this);
    public override IFStructParams CreateFStructParams(nint ptr) => new FStructParams_UE5_2_1(ptr, this);
    public override IFPropertyParams CreateFPropertyParams(nint ptr) => new FPropertyParams_UE5_2_1(ptr, this);
    public override IFGenericPropertyParams CreateFGenericPropertyParams(nint ptr) => new FGenericPropertyParams_UE5_2_1(ptr, this);
    public override IFWorldContext CreateFWorldContext(nint ptr) => new FWorldContext_UE5_4_4(ptr, this);
    public override IUEngine CreateUEngine(nint ptr) => new UEngine_UE5_4_4(ptr, this);
    public override IUGameInstance CreateUGameInstance(nint ptr) => new UGameInstance_UE5_4_4(ptr, this);
    public override IFStaticConstructObjectParameters CreateFStaticConstructObjectParameters()
        => new FStaticConstructObjectParameters_UE5_4_4(this);
    public override IFActorSpawnParameters CreateFActorSpawnParameters()
        => new FActorSpawnParameters_UE5_4_4(this);
}

public unsafe class UScriptStruct_UE5_2_1(nint ptr, IUnrealFactory factory)
    : UStruct_UE5_2_1(ptr, factory), IUScriptStruct
{
    private readonly UScriptStruct* _self = (UScriptStruct*)ptr;
    public EStructFlags StructFlags => _self->StructFlags;
    public bool bPrepareCppStructOpsCompleted => _self->bPrepareCppStructOpsCompleted;
    public nint CppStructOps => _self->CppStructOps;
}

public unsafe class UStruct_UE5_2_1(nint ptr, IUnrealFactory factory)
    : UField_UE5_4_4(ptr, factory), IUStruct
{
    private readonly UStruct* _self = (UStruct*)ptr;

    public IUStruct? SuperStruct
        => _self->SuperStruct != null ? _factory.CreateUStruct((nint)_self->SuperStruct) : null;
    public IEnumerable<IUField> Children
        => new IUFieldEnumerable(_self->Children != null ? _factory.CreateUField((nint)_self->Children) : null);
    public IEnumerable<IFField> ChildProperties =>
        new IFFieldEnumerable(_self->ChildProperties != null ? _factory.CreateFField((nint)_self->ChildProperties) : null);
    public int PropertiesSize => _self->PropertiesSize;
    public int MinAlignment => _self->MinAlignment;
    public TArray<byte> Script { get; } = new();

    public IEnumerable<IFProperty> PropertyLink
        => new UE4_27_2.IFPropertyEnumerable(_factory.CreateFProperty((nint)_self->PropertyLink), PropertyType.PropertyLink,
            _factory);
    
    public IEnumerable<IFProperty> RefLink
        => new UE4_27_2.IFPropertyEnumerable(_factory.CreateFProperty((nint)_self->RefLink), PropertyType.NextRef,
            _factory);
    public IEnumerable<IFProperty> DestructorLink
        => new UE4_27_2.IFPropertyEnumerable(_factory.CreateFProperty((nint)_self->DestructorLink), PropertyType.DestructorLink,
            _factory);
    public IEnumerable<IFProperty> PostConstructLink
        => new UE4_27_2.IFPropertyEnumerable(_factory.CreateFProperty((nint)_self->PostConstructLink), PropertyType.PostConstructLink,
            _factory);
}

public unsafe class UClass_UE5_2_1(nint ptr, IUnrealFactory factory)
    : UStruct_UE5_2_1(ptr, factory), IUClass
{
    private readonly UClass* _self = (UClass*)ptr;

    public IUClass? GetSuperClass()
        => _self->GetSuperClass() != null ? _factory.CreateUClass((nint)_self->GetSuperClass()) : null;
    
    public IUFunction? GetFunction(string Name)
    {
        var FuncMapDict = new TMapDictionary<FName, UFunction>(
            (TMap<FName, UFunction>*)(&_self->FuncMap), factory.Memory
        );
        return FuncMapDict.TryGetValue(new(Name), out var Function)
            ? factory.CreateUFunction((nint)Function.Value)
            : null;
    }
    
    public IUObject? ClassDefaultObject 
        => _self->ClassDefaultObject != null ? factory.CreateUObject((nint)_self->ClassDefaultObject ) : null;

    public nint Constructor => _self->ClassConstructor;
    public EClassFlags ClassFlags => _self->ClassFlags;
}

public unsafe class FStructParams_UE5_2_1(nint ptr, IUnrealFactory factory) : IFStructParams
{
    private readonly FStructParams* _self = (FStructParams*)ptr;
    protected readonly IUnrealFactory _factory = factory;

    public nint Ptr => (nint)_self;

    public nint OuterFunc => _self->OuterFunc;
    public nint SuperFunc => _self->SuperFunc;
    public nint StructOpsFunc => _self->StructOpsFunc;
    public string Name => Marshal.PtrToStringUTF8(_self->NameUTF8)!;
    public ulong Size => _self->SizeOf;
    public ulong Alignment => _self->AlignOf;
    public EObjectFlags ObjectFlags => _self->ObjectFlags;
    public EStructFlags StructFlags => _self->StructFlags;
    public int PropertyCount => _self->NumProperties;
    public IFPropertyParams? GetProperty(int Index)
    {
        var Result = ((FStructParams*)Ptr)->GetProperty(Index);
        return Result != null ? _factory.CreateFPropertyParams((nint)Result) : null;
    }
    
    public IEnumerable<IFPropertyParams> Properties => new FPropertyParamEnumerator(this);
}

public unsafe class FPropertyParams_UE5_2_1(nint ptr, IUnrealFactory factory) : IFPropertyParams
{
    private readonly FPropertyParamsBase* _self = (FPropertyParamsBase*)ptr;
    protected readonly IUnrealFactory _factory = factory;   
    
    public nint Ptr => (nint)_self;
    public string Name => Marshal.PtrToStringUTF8(_self->NameUTF8)!;
    public EPropertyFlags PropertyFlags => _self->PropertyFlags;
    public EPropertyGenFlags GenFlags => _self->PropertyGenFlags;
    public EObjectFlags ObjectFlags => _self->ObjectFlags;
}

public unsafe class FGenericPropertyParams_UE5_2_1(nint ptr, IUnrealFactory factory) 
    : FPropertyParams_UE5_2_1(ptr, factory), IFGenericPropertyParams
{
    private readonly FGenericPropertyParams* _self = (FGenericPropertyParams*)ptr;

    public int ArrayDim => _self->Super.ArrayDim;
    public int Offset => _self->Super.Offset;
}