using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using UE.Toolkit.Core.Common;
using UE.Toolkit.Core.Types.Interfaces;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

// ReSharper disable InconsistentNaming

namespace UE.Toolkit.Core.Types.Unreal.Factories.UE5_4_4;

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
    
    public override IFProperty CreateFProperty(nint ptr) => new FProperty_UE5_4_4(ptr, this);
    public override IFByteProperty CreateFByteProperty(nint ptr) => new FByteProperty_UE5_4_4(ptr, this);
    public override IFBoolProperty CreateFBoolProperty(nint ptr) => new FBoolProperty_UE5_4_4(ptr, this);
    public override IFEnumProperty CreateFEnumProperty(nint ptr) => new FEnumProperty_UE5_4_4(ptr, this);
    public override IFObjectProperty CreateFObjectProperty(nint ptr) => new FObjectProperty_UE5_4_4(ptr, this);
    public override IFSoftClassProperty CreateFSoftClassProperty(nint ptr) => new FSoftClassProperty_UE5_4_4(ptr, this);
    public override IFClassProperty CreateFClassProperty(nint ptr) => new FClassProperty_UE5_4_4(ptr, this);
    public override IFStructProperty CreateFStructProperty(nint ptr) => new FStructProperty_UE5_4_4(ptr, this);
    public override IFMapProperty CreateFMapProperty(nint ptr) => new FMapProperty_UE5_4_4(ptr, this);
    public override IFInterfaceProperty CreateFInterfaceProperty(nint ptr) => new FInterfaceProperty_UE5_4_4(ptr, this);
    public override IFArrayProperty CreateFArrayProperty(nint ptr) => new FArrayProperty_UE5_4_4(ptr, this);
    public override IFSetProperty CreateFSetProperty(nint ptr) => new FSetProperty_UE5_4_4(ptr, this);
    public override IFOptionalProperty CreateFOptionalProperty(nint ptr) => new FOptionalProperty_UE5_4_4(ptr, this);
    public override IFDelegateProperty CreateFDelegateProperty(nint ptr) => new FDelegateProperty_UE5_4_4(ptr, this);
    public override IUObjectArray CreateUObjectArray(nint ptr) => new UObjectArray_UE5_4_4(ptr, this);
    public override IUObject CreateUObject(nint ptr) => new UObject_UE5_4_4(ptr, this, Memory);
    public override IUClass CreateUClass(nint ptr) => new UClass_UE5_4_4(ptr, this, Memory);
    public override IUScriptStruct CreateUScriptStruct(nint ptr) => new UScriptStruct_UE5_4_4(ptr, this, Memory);
    public override IUEnum CreateUEnum(nint ptr) => new UEnum_UE5_4_4(ptr, this, Memory);
    public override IUField CreateUField(nint ptr) => new UField_UE5_4_4(ptr, this, Memory);
    public override IUStruct CreateUStruct(nint ptr) => new UStruct_UE5_4_4(ptr, this, Memory);
    public override IUUserDefinedEnum CreateUUserDefinedEnum(nint ptr) => new UUserDefinedEnum_UE5_4_4(ptr, this, Memory);
    public override IUFunction CreateUFunction(nint ptr) => new UFunction_UE5_4_4(ptr, this, Memory);
    public override IFFieldClass CreateFFieldClass(nint ptr) => new FFieldClass_UE5_4_4(ptr, this);
    public override IFField CreateFField(nint ptr) => new FField_UE5_4_4(ptr, this);
    public override IFFieldVariant CreateFFieldVariant(nint ptr) => new FFieldVariantUE5_4_4(ptr, this);
    public override IFStructParams CreateFStructParams(nint ptr) => new FStructParams_UE5_4_4(ptr, this);
    public override IFPropertyParams CreateFPropertyParams(nint ptr) => new FPropertyParams_UE5_4_4(ptr, this);
    public override IFGenericPropertyParams CreateFGenericPropertyParams(nint ptr) => new FGenericPropertyParams_UE5_4_4(ptr, this);
    public override IFWorldContext CreateFWorldContext(nint ptr) => new FWorldContext_UE5_4_4(ptr, this);
    public override IUEngine CreateUEngine(nint ptr) => new UEngine_UE5_4_4(ptr, this, Memory);
    public override IUGameInstance CreateUGameInstance(nint ptr) => new UGameInstance_UE5_4_4(ptr, this, Memory);
    public override IFStaticConstructObjectParameters CreateFStaticConstructObjectParameters()
        => new FStaticConstructObjectParameters_UE5_4_4(this);
    public override IFActorSpawnParameters CreateFActorSpawnParameters()
        => new FActorSpawnParameters_UE5_4_4(this);
}

public unsafe class FOptionalProperty_UE5_4_4(nint ptr, IUnrealFactory factory)
    : FProperty_UE5_4_4(ptr, factory), IFOptionalProperty
{
    public IFProperty ValueProperty => _factory.CreateFProperty((nint)((FOptionalProperty*)Ptr)->ValueProperty);
}

public unsafe class FSetProperty_UE5_4_4(nint ptr, IUnrealFactory factory)
    : FProperty_UE5_4_4(ptr, factory), IFSetProperty
{
    public IFProperty ElementProp => _factory.CreateFProperty((nint)((FSetProperty*)Ptr)->ElementProp);
}

public unsafe class FArrayProperty_UE5_4_4(nint ptr, IUnrealFactory factory)
    : FProperty_UE5_4_4(ptr, factory), IFArrayProperty
{
    public IFProperty Inner => _factory.CreateFProperty((nint)((FArrayProperty*)Ptr)->Inner);
}

public unsafe class FInterfaceProperty_UE5_4_4(nint ptr, IUnrealFactory factory)
    : FProperty_UE5_4_4(ptr, factory), IFInterfaceProperty
{
    public IUClass InterfaceClass => _factory.CreateUClass((nint)((FInterfaceProperty*)Ptr)->InterfaceClass);
}

public unsafe class FMapProperty_UE5_4_4(nint ptr, IUnrealFactory factory)
    : FProperty_UE5_4_4(ptr, factory), IFMapProperty
{
    public IFProperty KeyProp => _factory.CreateFProperty((nint)((FMapProperty*)Ptr)->KeyProp);
    public IFProperty ValueProp => _factory.CreateFProperty((nint)((FMapProperty*)Ptr)->ValueProp);
}

public unsafe class FStructProperty_UE5_4_4(nint ptr, IUnrealFactory factory)
    : FProperty_UE5_4_4(ptr, factory), IFStructProperty
{
    public IUScriptStruct Struct => _factory.CreateUScriptStruct((nint)((FStructProperty*)Ptr)->Struct);
}

public unsafe class FClassProperty_UE5_4_4(nint ptr, IUnrealFactory factory)
    : FProperty_UE5_4_4(ptr, factory), IFClassProperty
{
    public IUClass PropertyClass => _factory.CreateUClass((nint)((FClassProperty*)Ptr)->Super.PropertyClass);
    public IUClass MetaClass => _factory.CreateUClass((nint)((FClassProperty*)Ptr)->MetaClass);
}

public unsafe class FSoftClassProperty_UE5_4_4(nint ptr, IUnrealFactory factory)
    : FProperty_UE5_4_4(ptr, factory), IFSoftClassProperty
{
    public IUClass MetaClass => _factory.CreateUClass((nint)((FSoftClassProperty*)Ptr)->MetaClass);
}

public unsafe class FObjectProperty_UE5_4_4(nint ptr, IUnrealFactory factory)
    : FProperty_UE5_4_4(ptr, factory), IFObjectProperty
{
    public IUClass PropertyClass => _factory.CreateUClass((nint)((FObjectProperty*)Ptr)->PropertyClass);
}

public unsafe class FEnumProperty_UE5_4_4(nint ptr, IUnrealFactory factory)
    : FProperty_UE5_4_4(ptr, factory), IFEnumProperty
{
    private readonly FEnumProperty* _self = (FEnumProperty*)ptr;

    public IUEnum Enum => _factory.CreateUEnum((nint)_self->Enum);
    public IFProperty UnderlyingProp => _factory.CreateFProperty((nint)_self->UnderlyingProp);
}

public unsafe class FFieldClass_UE5_4_4(nint ptr, IUnrealFactory factory)
    : IFFieldClass
{
    private readonly FFieldClass* _self = (FFieldClass*)ptr;

    public nint Ptr => ptr;
    public string Name => _self->Name.ToString();
    public ulong Id => _self->Id;
    public ulong CastFlags => _self->CastFlags;
    public EClassFlags ClassFlags => _self->ClassFlags;
    public IFFieldClass SuperClass => factory.CreateFFieldClass((nint)_self->SuperClass);
    public IFField DefaultObject => factory.CreateFField((nint)_self->DefaultObject);
    public nint FieldConstructor => _self->FieldConstructor;
}

public unsafe class FField_UE5_4_4(nint ptr, IUnrealFactory factory)
    : IFField
{
    private readonly FField* _self = (FField*)ptr;
    protected readonly IUnrealFactory _factory = factory;

    public nint Ptr => ptr;
    public nint VTable => _self->VTable;
    public IFFieldClass ClassPrivate => _factory.CreateFFieldClass((nint)_self->ClassPrivate);
    public IFFieldVariant Owner => factory.CreateFFieldVariant((nint)(&_self->Owner));
    public IFField? Next => _self->Next != null ? _factory.CreateFField((nint)_self->Next) : null;
    public string NamePrivate => _self->NamePrivate.ToString();
    public EObjectFlags FlagsPrivate => _self->FlagsPrivate;
    
    public void SetOwnerUObject(IUObject owner) => _self->Owner.Object = (UObjectBase*)(owner.Ptr + 1);

    public void SetOwnerFField(IFField owner) => _self->Owner.Field = (FField*)owner.Ptr;
}

public unsafe class FFieldVariantUE5_4_4(nint ptr, IUnrealFactory factory)
    : IFFieldVariant
{
    private readonly FFieldObjectUnion* _self = (FFieldObjectUnion*)ptr;
    protected readonly IUnrealFactory _factory = factory;

    public nint Ptr => ptr;
    public IFField? Field => _self->Field != null ? _factory.CreateFField((nint)_self->Field) : null;
    public IUObject? Object => _self->Object != null ? _factory.CreateUObject((nint)(_self->Object) + 1) : null;
    public bool IsObject => ((nint)_self->Object & 1) != 0;
}

public unsafe class FProperty_UE5_4_4(nint ptr, IUnrealFactory factory)
    : FField_UE5_4_4(ptr, factory), IFProperty
{
    private readonly FProperty* _self = (FProperty*)ptr;

    public int ArrayDim => _self->ArrayDim;
    public int ElementSize => _self->ElementSize;
    public EPropertyFlags PropertyFlags => _self->PropertyFlags;
    public ushort RepIndex => _self->RepIndex;
    public byte BlueprintReplicationCondition => _self->BlueprintReplicationCondition;
    public int Offset_Internal => _self->Offset_Internal;
    public IEnumerable<IFProperty> PropertyLinkNext
        => new IFPropertyEnumerable(_factory.CreateFProperty((nint)_self->PropertyLinkNext), PropertyType.PropertyLink,
            _factory);
    public IEnumerable<IFProperty> NextRef
        => new IFPropertyEnumerable(_factory.CreateFProperty((nint)_self->NextRef), PropertyType.NextRef,
            _factory);
    public IEnumerable<IFProperty> DestructorLinkNext
        => new IFPropertyEnumerable(_factory.CreateFProperty((nint)_self->DestructorLinkNext), PropertyType.DestructorLink,
            _factory);
    public IEnumerable<IFProperty> PostConstructLinkNext
        => new IFPropertyEnumerable(_factory.CreateFProperty((nint)_self->PostConstructLinkNext), PropertyType.PostConstructLink,
            _factory);
    public string RepNotifyFunc => _self->RepNotifyFunc.ToString();
}

public unsafe class IFPropertyEnumerable(IFProperty initial, PropertyType propType, IUnrealFactory factory)
    : IEnumerator<IFProperty>, IEnumerable<IFProperty>
{
    private FProperty* _current = (FProperty*)initial.Ptr;
    private bool isInitial = true;
    
    public bool MoveNext()
    {
        if (isInitial)
        {
            isInitial = false;
            return _current != null;
        }
        
        if (_current == null) return false;
        switch (propType)
        {
            case PropertyType.PropertyLink:
                _current = _current->PropertyLinkNext;
                break;
            case PropertyType.NextRef:
                _current = _current->NextRef;
                break;
            case PropertyType.DestructorLink:
                _current = _current->DestructorLinkNext;
                break;
            case PropertyType.PostConstructLink:
                _current = _current->PostConstructLinkNext;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(propType), propType, null);
        }

        return _current != null;
    }

    public void Reset() => _current = (FProperty*)initial.Ptr;

    public IFProperty Current => factory.CreateFProperty((nint)_current);

    object IEnumerator.Current => Current;

    public void Dispose() { }

    public IEnumerator<IFProperty> GetEnumerator() => this;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public unsafe class FBoolProperty_UE5_4_4(nint ptr, IUnrealFactory factory)
    : FProperty_UE5_4_4(ptr, factory), IFBoolProperty
{
    public byte FieldSize => ((FBoolProperty*)ptr)->FieldSize;
    public byte ByteOffset => ((FBoolProperty*)ptr)->ByteOffset;
    public byte ByteMask => ((FBoolProperty*)ptr)->ByteMask;
    public byte FieldMask => ((FBoolProperty*)ptr)->FieldMask;
}

public unsafe class FByteProperty_UE5_4_4(nint ptr, IUnrealFactory factory)
    : FProperty_UE5_4_4(ptr, factory), IFByteProperty
{
    public IUEnum? Enum => ((FByteProperty*)Ptr)->Enum != null ? _factory.CreateUEnum((nint)((FByteProperty*)Ptr)->Enum) : null;
}

public unsafe class UUserDefinedEnum_UE5_4_4(nint ptr, IUnrealFactory factory, IUnrealMemoryInternal memory)
    : UEnum_UE5_4_4(ptr, factory, memory), IUUserDefinedEnum
{
    public TMap<FName, FText> DisplayNameMap => ((UUserDefinedEnum*)Ptr)->DisplayNameMap;
}

public unsafe class UEnum_UE5_4_4(nint ptr, IUnrealFactory factory, IUnrealMemoryInternal memory)
    : UField_UE5_4_4(ptr, factory, memory), IUEnum
{
    private readonly UEnum* _self = (UEnum*)ptr;
    public string CppType => _self->CppType.ToString();
    public TArray<TPair<FName, long>> Names => _self->Names;
    
    public bool TryParse(string name, bool ignoreCase, [NotNullWhen(true)] out long? value)
    {
        value = null;
        if (ignoreCase)
        {
            name = name.ToLower();
        }
        var NamesArray = new TArrayList<TPair<FName, long>>(&_self->Names, _factory.Memory);
        foreach (var pDiscriminant in NamesArray)
        {
            var Discriminant = pDiscriminant.Value;
            var CheckName = Discriminant->Key.ToString();
            if (ignoreCase)
                CheckName = CheckName.ToLower();
            if (CheckName == name)
            {
                value = Discriminant->Value;
                return true;
            }
        }
        return false;
    }
}

public unsafe class UScriptStruct_UE5_4_4(nint ptr, IUnrealFactory factory, IUnrealMemoryInternal memory)
    : UStruct_UE5_4_4(ptr, factory, memory), IUScriptStruct
{
    private readonly UScriptStruct* _self = (UScriptStruct*)ptr;
    public EStructFlags StructFlags => _self->StructFlags;
    public bool bPrepareCppStructOpsCompleted => _self->bPrepareCppStructOpsCompleted;
    public nint CppStructOps => _self->CppStructOps;
}

public unsafe class UStruct_UE5_4_4(nint ptr, IUnrealFactory factory, IUnrealMemoryInternal memory)
    : UField_UE5_4_4(ptr, factory, memory), IUStruct
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
        => new IFPropertyEnumerable(_factory.CreateFProperty((nint)_self->PropertyLink), PropertyType.PropertyLink,
            _factory);
    
    public IEnumerable<IFProperty> RefLink
        => new IFPropertyEnumerable(_factory.CreateFProperty((nint)_self->RefLink), PropertyType.NextRef,
            _factory);
    public IEnumerable<IFProperty> DestructorLink
        => new IFPropertyEnumerable(_factory.CreateFProperty((nint)_self->DestructorLink), PropertyType.DestructorLink,
            _factory);
    public IEnumerable<IFProperty> PostConstructLink
        => new IFPropertyEnumerable(_factory.CreateFProperty((nint)_self->PostConstructLink), PropertyType.PostConstructLink,
            _factory);
}

public class IFFieldEnumerable(IFField? initial)
    : IEnumerator<IFField>, IEnumerable<IFField>
{
    private readonly IFField _initial = initial;
    private IFField? _current = initial;
    private bool isInitial = true;

    public bool MoveNext()
    {
        if (isInitial)
        {
            isInitial = false;
            return _current != null;
        }
        
        _current = _current?.Next;
        return _current != null;
    }

    public void Reset() => _current = _initial;

    public IFField Current => _current!;

    object IEnumerator.Current => Current;

    public void Dispose() { }

    public IEnumerator<IFField> GetEnumerator() => this;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class IUFieldEnumerable(IUField? initial)
    : IEnumerator<IUField>, IEnumerable<IUField>
{
    private readonly IUField _initial = initial;
    private IUField? _current = initial;
    private bool isInitial = true;

    public bool MoveNext()
    {
        if (isInitial)
        {
            isInitial = false;
            return _current != null;
        }
        
        _current = _current?.Next;
        return _current != null;
    }

    public void Reset() => _current = _initial;

    public IUField Current => _current!;

    object IEnumerator.Current => Current;

    public void Dispose() { }
    
    public IEnumerator<IUField> GetEnumerator() => this;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public unsafe class UField_UE5_4_4(nint ptr, IUnrealFactory factory, IUnrealMemoryInternal memory)
    : UObject_UE5_4_4(ptr, factory, memory), IUField
{
    private readonly UField* _self = (UField*)ptr;

    public IUField? Next
        => _self->Next != null ? _factory.CreateUField((nint)_self->Next) : null;
}

public unsafe class UObject_UE5_4_4(nint ptr, IUnrealFactory factory, IUnrealMemoryInternal memory) 
    : BaseUObject<UObjectBase>(ptr, factory, memory)
{
    public override nint VTable => _self->VTable;

    public override EObjectFlags ObjectFlags => _self->ObjectFlags;

    public override int InternalIndex => _self->InternalIndex;

    public override IUClass ClassPrivate => _factory.CreateUClass((nint)_self->ClassPrivate);

    public override FName NamePrivate => _self->NamePrivate;

    public override IUObject? OuterPrivate => _self->OuterPrivate != null ? _factory.CreateUObject((nint)_self->OuterPrivate) : null;

    public override bool IsChildOf(string type) => _self->IsChildOf(type);

    public override IUObject GetOutermost() => _factory.CreateUObject((nint)_self->GetOutermost());

    public override string GetNativeName() => ToolkitUtils.GetNativeName(this);
    
    public override string GetPathName() => ToolkitUtils.GetPathName(this);
}

public unsafe class UClass_UE5_4_4(nint ptr, IUnrealFactory factory, IUnrealMemoryInternal memory)
    : UStruct_UE5_4_4(ptr, factory, memory), IUClass
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
    
    public IEnumerable<IUFunction> GetFunctions()
    {
        var FuncMapDict = new TMapDictionary<FName, Ptr<UFunction>>(
            (TMap<FName, Ptr<UFunction>>*)(&_self->FuncMap),
            _factory.Memory
        );
        return FuncMapDict.Values.Select(x => _factory.CreateUFunction((nint)x.Value->Value));
    }
    
    public IUObject? ClassDefaultObject 
        => _self->ClassDefaultObject != null ? factory.CreateUObject((nint)_self->ClassDefaultObject ) : null;

    public nint Constructor => _self->ClassConstructor;
    public EClassFlags ClassFlags => _self->ClassFlags;
}

public unsafe class UFunction_UE5_4_4(nint ptr, IUnrealFactory factory, IUnrealMemoryInternal memory)
    : UStruct_UE5_4_4(ptr, factory, memory), IUFunction
{
    private readonly UFunction* _self = (UFunction*)ptr;

    public EFunctionFlags FunctionFlags => _self->FunctionFlags;
    public int ParamCount => _self->NumParms;
    public int ParamSize => _self->ParmsSize;
    public int ReturnValueOffset => _self->ReturnValueOffset;
    
    public int GetTotalParameterSize()
    {
        var LastProperty = ChildProperties.Any() ? _factory.CreateFProperty(ChildProperties.Last().Ptr) : null;
        return LastProperty != null ? LastProperty.Offset_Internal + LastProperty.ElementSize : 0;
    }

    public nint FunctionPtr => (nint)_self->Func;
}

public unsafe class UObjectArray_UE5_4_4(nint ptr, IUnrealFactory factory) : IUObjectArray
{
    private readonly FUObjectArray* _self = (FUObjectArray*)ptr;

    public int ObjLastNonGCIndex => _self->ObjLastNonGCIndex;
    public int NumElements => _self->ObjObjects.NumElements;

    public IUObject? IndexToObject(int idx)
    {
        var objItem = _self->ObjObjects.GetItem(idx);
        if (objItem == null || objItem->Object == null || objItem->Flags.HasFlag(EInternalObjectFlags.Unreachable)) return null;
        
        return factory.CreateUObject((nint)objItem->Object);
    }

    public void AddToRootSet(int idx)
    {
        var objItem = _self->ObjObjects.GetItem(idx);
        if (objItem == null || objItem->Object == null) return;
        objItem->Flags |= EInternalObjectFlags.RootSet;
        objItem->Object->ObjectFlags |= EObjectFlags.RF_MarkAsRootSet;
    }

    public void RemoveFromRootSet(int idx)
    {
        var objItem = _self->ObjObjects.GetItem(idx);
        if (objItem == null || objItem->Object == null) return;
        objItem->Flags &= ~EInternalObjectFlags.RootSet;
        objItem->Object->ObjectFlags &= ~EObjectFlags.RF_MarkAsRootSet;
    }
}

public class FPropertyParamEnumerator(IFStructParams owner) 
    : IEnumerator<IFPropertyParams>, IEnumerable<IFPropertyParams>
{
    private int CurrentIndex = -1;
    
    #region impl IEnumerator
    
    public bool MoveNext() => ++CurrentIndex < owner.PropertyCount;

    public void Reset() => CurrentIndex = -1;

    public IFPropertyParams Current => owner.GetProperty(CurrentIndex);

    object? IEnumerator.Current => Current;

    public void Dispose() {}
    
    #endregion
    
    #region impl IEnumerable
    
    public IEnumerator<IFPropertyParams> GetEnumerator() => this;
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    #endregion
}

public unsafe class FStructParams_UE5_4_4(nint ptr, IUnrealFactory factory) : IFStructParams
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

public unsafe class FPropertyParams_UE5_4_4(nint ptr, IUnrealFactory factory) : IFPropertyParams
{
    private readonly FPropertyParamsBase* _self = (FPropertyParamsBase*)ptr;
    protected readonly IUnrealFactory _factory = factory;   
    
    public nint Ptr => (nint)_self;
    public string Name => Marshal.PtrToStringUTF8(_self->NameUTF8)!;
    public EPropertyFlags PropertyFlags => _self->PropertyFlags;
    public EPropertyGenFlags GenFlags => _self->PropertyGenFlags;
    public EObjectFlags ObjectFlags => _self->ObjectFlags;
}

public unsafe class FGenericPropertyParams_UE5_4_4(nint ptr, IUnrealFactory factory) 
    : FPropertyParams_UE5_4_4(ptr, factory), IFGenericPropertyParams
{
    private readonly FGenericPropertyParams* _self = (FGenericPropertyParams*)ptr;

    public int ArrayDim => _self->Super.ArrayDim;
    public int Offset => _self->Super.Offset;
}

public unsafe class FWorldContext_UE5_4_4(nint ptr, IUnrealFactory factory) : IFWorldContext
{
    protected readonly IUnrealFactory _factory = factory;
    private readonly FWorldContext* _self = (FWorldContext*)ptr;
    public nint Ptr => (nint)_self;
    public WorldType GetWorldType() => _self->WorldType;
    public nint GetWorld() => (nint)_self->ThisCurrentWorld;
}

public unsafe class FWorldContextEnumerator(UEngine_UE5_4_4 owner, IUnrealFactory factory) 
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

public unsafe class UEngine_UE5_4_4(nint ptr, IUnrealFactory factory, IUnrealMemoryInternal memory) 
    : UObject_UE5_4_4(ptr, factory, memory), IUEngine
{
    private readonly UEngine* _self = (UEngine*)ptr;

    internal TArray<Ptr<FWorldContext>>* GetWorldListInner() => &_self->WorldList;
    
    public IEnumerable<IFWorldContext> GetWorldList() => new FWorldContextEnumerator(this, factory);
}

public unsafe class FStaticConstructObjectParameters_UE5_4_4
    : IFStaticConstructObjectParameters, IDisposable
{
    private readonly FStaticConstructObjectParameters* _self;
    public nint Ptr => (nint)_self;
    protected readonly IUnrealFactory _factory;
    private bool Disposed = false;

    public FStaticConstructObjectParameters_UE5_4_4(IUnrealFactory factory)
    {
        _factory = factory;
        _self = (FStaticConstructObjectParameters*)_factory.Memory.MallocZeroed(sizeof(FStaticConstructObjectParameters));
    }

    public void SetParams(IUClass Class, IUObject? Owner, FName Name)
    {
        _self->Class = (UClass*)Class.Ptr;
        _self->Outer = (UObjectBase*)Owner?.Ptr;
        _self->Name = Name;
    }
    
    #region DISPOSE INTERFACE
    
    public void Dispose()
    {
        Disposing();
        GC.SuppressFinalize(this);
    }
    
    protected virtual void Disposing()
    {
        if (Disposed) return;
        _factory.Memory.Free(Ptr);
        Disposed = true;
    }

    ~FStaticConstructObjectParameters_UE5_4_4() => Disposing();
    
    #endregion
}

public unsafe class FActorSpawnParameters_UE5_4_4
    : IFActorSpawnParameters, IDisposable
{
    private readonly FActorSpawnParameters* _self;
    public nint Ptr => (nint)_self;
    protected readonly IUnrealFactory _factory;
    private bool Disposed = false;

    public FActorSpawnParameters_UE5_4_4(IUnrealFactory factory)
    {
        _factory = factory;
        _self = (FActorSpawnParameters*)_factory.Memory.MallocZeroed(sizeof(FActorSpawnParameters));
    }
    
    public void SetParams(EObjectFlags Flags)
    {
        _self->ObjectFlags = Flags;
    }
    
    #region DISPOSE INTERFACE
    
    public void Dispose()
    {
        Disposing();
        GC.SuppressFinalize(this);
    }
    
    protected virtual void Disposing()
    {
        if (Disposed) return;
        _factory.Memory.Free(Ptr);
        Disposed = true;
    }

    ~FActorSpawnParameters_UE5_4_4() => Disposing();
    
    #endregion
}

public unsafe class UGameInstance_UE5_4_4(nint ptr, IUnrealFactory factory, IUnrealMemoryInternal memory)
    : UObject_UE5_4_4(ptr, factory, memory), IUGameInstance
{
    private readonly UGameInstance* _self = (UGameInstance*)ptr;

    public bool TryGetSubsystem(IUClass Class, out IUObject? Subsystem)
    {
        var Subsystems = new TMapDictionary<HashablePtr<UClass>, HashablePtr<UObjectBase>>(
           (TMap<HashablePtr<UClass>, HashablePtr<UObjectBase>>*)(&_self->SubsystemCollection.Subsystems),
            factory.Memory);
        Subsystem = Subsystems.TryGetValue(new HashablePtr<UClass>(new Ptr<UClass>((UClass*)Class.Ptr)),
            out var pSubsystem)
            ? _factory.CreateUObject((nint)pSubsystem.Value->Ptr.Value)
            : null;
        return Subsystem != null;
    }
}

public unsafe class FDelegateProperty_UE5_4_4(nint ptr, IUnrealFactory factory)
    : FProperty_UE5_4_4(ptr, factory), IFDelegateProperty 
{
    public IUFunction? Function => ((FDelegateProperty*)Ptr)->SignatureFunction != null ? _factory.CreateUFunction((nint)((FDelegateProperty*)Ptr)->SignatureFunction) : null;
}