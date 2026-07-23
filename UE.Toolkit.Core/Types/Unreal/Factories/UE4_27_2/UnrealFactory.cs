using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using UE.Toolkit.Core.Common;
using UE.Toolkit.Core.Types.Interfaces;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;
using EFunctionFlags = UE.Toolkit.Core.Types.Unreal.UE5_4_4.EFunctionFlags;
using EPropertyFlags = UE.Toolkit.Core.Types.Unreal.UE5_4_4.EPropertyFlags;
using EPropertyGenFlags = UE.Toolkit.Core.Types.Unreal.UE5_4_4.EPropertyGenFlags;
using EClassFlags = UE.Toolkit.Core.Types.Unreal.UE5_4_4.EClassFlags;
// using FFieldObjectUnion = UE.Toolkit.Core.Types.Unreal.UE5_4_4.FFieldObjectUnion;
using FUObjectArray_Pack4 = UE.Toolkit.Core.Types.Unreal.UE5_4_4.FUObjectArray_Pack4;
using FName = UE.Toolkit.Core.Types.Unreal.UE5_4_4.FName;
using EStructFlags = UE.Toolkit.Core.Types.Unreal.UE5_4_4.EStructFlags;
using EInternalObjectFlags = UE.Toolkit.Core.Types.Unreal.UE5_4_4.EInternalObjectFlags;
using EObjectFlags = UE.Toolkit.Core.Types.Unreal.UE5_4_4.EObjectFlags;
using FArrayProperty = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FArrayProperty;
using FBoolProperty = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FBoolProperty;
using FByteProperty = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FByteProperty;
using FClassProperty = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FClassProperty;
using FDelegateProperty = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FDelegateProperty;
using FEnumProperty = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FEnumProperty;
using FField = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FField;
using FFieldClass = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FFieldClass;
using FFieldObjectUnion = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FFieldObjectUnion;
using FGenericPropertyParams = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FGenericPropertyParams;
using FMapProperty = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FMapProperty;
using FObjectProperty = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FObjectProperty;
using FProperty = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FProperty;
using FPropertyParamsBase = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FPropertyParamsBase;
using FSetProperty = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FSetProperty;
using FStaticConstructObjectParameters = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FStaticConstructObjectParameters;
using FStructParams = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FStructParams;
using FStructProperty = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FStructProperty;
using FWorldContext = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FWorldContext;
using UClass = UE.Toolkit.Core.Types.Unreal.UE4_27_2.UClass;
using UEngine = UE.Toolkit.Core.Types.Unreal.UE4_27_2.UEngine;
using UEnum = UE.Toolkit.Core.Types.Unreal.UE4_27_2.UEnum;
using UField = UE.Toolkit.Core.Types.Unreal.UE4_27_2.UField;
using UFunction = UE.Toolkit.Core.Types.Unreal.UE4_27_2.UFunction;
using UGameInstance = UE.Toolkit.Core.Types.Unreal.UE4_27_2.UGameInstance;
using UObjectBase = UE.Toolkit.Core.Types.Unreal.UE4_27_2.UObjectBase;
using UScriptStruct = UE.Toolkit.Core.Types.Unreal.UE4_27_2.UScriptStruct;
using UStruct = UE.Toolkit.Core.Types.Unreal.UE4_27_2.UStruct;
using UUserDefinedEnum = UE.Toolkit.Core.Types.Unreal.UE4_27_2.UUserDefinedEnum;

// ReSharper disable InconsistentNaming

namespace UE.Toolkit.Core.Types.Unreal.Factories.UE4_27_2;

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

    public override IFProperty CreateFProperty(nint ptr) => new FPropertyUE4_27_2(ptr, this);

    public override IFByteProperty CreateFByteProperty(nint ptr) => new FBytePropertyUE4_27_2(ptr, this);
    
    public override IFBoolProperty CreateFBoolProperty(IntPtr ptr) => new FBoolPropertyUE4_27_2(ptr, this);

    public override IFEnumProperty CreateFEnumProperty(nint ptr) => new FEnumPropertyUE4_27_2(ptr, this);

    public override IFObjectProperty CreateFObjectProperty(nint ptr) => new FObjectPropertyUE4_27_2(ptr, this);

    public override IFSoftClassProperty CreateFSoftClassProperty(nint ptr) => new FSoftClassPropertyUE4_27_2(ptr, this);

    public override IFClassProperty CreateFClassProperty(nint ptr) => new FClassPropertyUE4_27_2(ptr, this);

    public override IFStructProperty CreateFStructProperty(nint ptr) => new FStructPropertyUE4_27_2(ptr, this);

    public override IFMapProperty CreateFMapProperty(nint ptr) => new FMapPropertyUE4_27_2(ptr, this);

    public override IFInterfaceProperty CreateFInterfaceProperty(nint ptr) => new FInterfacePropertyUE4_27_2(ptr, this);

    public override IFArrayProperty CreateFArrayProperty(nint ptr) => new FArrayPropertyUE4_27_2(ptr, this);

    public override IFSetProperty CreateFSetProperty(nint ptr) => new FSetPropertyUE4_27_2(ptr, this);

    public override IFOptionalProperty CreateFOptionalProperty(nint ptr) => throw new NotSupportedException();

    public override IFDelegateProperty CreateFDelegateProperty(nint ptr) => new FDelegatePropertyUE4_27_2(ptr, this);

    public override IUObjectArray CreateUObjectArray(nint ptr) => new UObjectArrayUE4_27_2(ptr, this);

    public override IUObject CreateUObject(nint ptr) => new UObjectUE4_27_2(ptr, this, Memory);

    public override IUClass CreateUClass(nint ptr) => new UClassUE4_27_2(ptr, this, Memory);

    public override IUScriptStruct CreateUScriptStruct(nint ptr) => new UScriptStructUE4_27_2(ptr, this, Memory);

    public override IUEnum CreateUEnum(nint ptr) => new UEnumUE4_27_2(ptr, this, Memory);

    public override IUField CreateUField(nint ptr) => new UFieldUE4_27_2(ptr, this, Memory);

    public override IUStruct CreateUStruct(nint ptr) => new UStructUE4_27_2(ptr, this, Memory);

    public override IUUserDefinedEnum CreateUUserDefinedEnum(nint ptr) => new UUserDefinedEnumUE4_27_2(ptr, this, Memory);
    
    public override IUFunction CreateUFunction(nint ptr) => new UFunctionUE4_27_2(ptr, this, Memory);

    public override IFFieldClass CreateFFieldClass(nint ptr) => new FFieldClassUE4_27_2(ptr, this);

    public override IFField CreateFField(nint ptr) => new FFieldUE4_27_2(ptr, this);
    
    public override IFFieldVariant CreateFFieldVariant(nint ptr) => new FFieldVariantUE4_27_2(ptr, this);
    
    public override IFStructParams CreateFStructParams(nint ptr) => new FStructParamsUE4_27_2(ptr, this);
    
    public override IFPropertyParams CreateFPropertyParams(nint ptr) => new FPropertyParamsUE4_27_2(ptr, this);
    
    public override IFGenericPropertyParams CreateFGenericPropertyParams(nint ptr) => new FGenericPropertyParamsUE4_27_2(ptr, this);
    
    public override IFWorldContext CreateFWorldContext(nint ptr) => new FWorldContextUE4_27_2(ptr, this);
    
    public override IUEngine CreateUEngine(nint ptr) => new UEngineUE4_27_2(ptr, this, Memory);
    
    public override IUGameInstance CreateUGameInstance(nint ptr) => new UGameInstanceUE4_27_2(ptr, this, Memory);

    public override IFStaticConstructObjectParameters CreateFStaticConstructObjectParameters()
        => new FStaticConstructObjectParametersUE4_27_2(this);
    
    public override IFActorSpawnParameters CreateFActorSpawnParameters()
        => new FActorSpawnParametersUE4_27_2(this);
}

public unsafe class FSetPropertyUE4_27_2(nint ptr, IUnrealFactory factory)
    : FPropertyUE4_27_2(ptr, factory), IFSetProperty
{
    public IFProperty ElementProp => _factory.CreateFProperty((nint)((FSetProperty*)Ptr)->elem_prop);
}

public unsafe class FArrayPropertyUE4_27_2(nint ptr, IUnrealFactory factory)
    : FPropertyUE4_27_2(ptr, factory), IFArrayProperty
{
    public IFProperty Inner => _factory.CreateFProperty((nint)((FArrayProperty*)Ptr)->inner);
}

public unsafe class FInterfacePropertyUE4_27_2(nint ptr, IUnrealFactory factory)
    : FPropertyUE4_27_2(ptr, factory), IFInterfaceProperty
{
    public IUClass InterfaceClass => _factory.CreateUClass((nint)((FObjectProperty*)Ptr)->prop_class);
}

public unsafe class FMapPropertyUE4_27_2(nint ptr, IUnrealFactory factory)
    : FPropertyUE4_27_2(ptr, factory), IFMapProperty
{
    public IFProperty KeyProp => _factory.CreateFProperty((nint)((FMapProperty*)Ptr)->key_prop);
    public IFProperty ValueProp => _factory.CreateFProperty((nint)((FMapProperty*)Ptr)->value_prop);
}

public unsafe class FStructPropertyUE4_27_2(nint ptr, IUnrealFactory factory)
    : FPropertyUE4_27_2(ptr, factory), IFStructProperty
{
    public IUScriptStruct Struct => _factory.CreateUScriptStruct((nint)((FStructProperty*)Ptr)->struct_data);
}

public unsafe class FClassPropertyUE4_27_2(nint ptr, IUnrealFactory factory)
    : FPropertyUE4_27_2(ptr, factory), IFClassProperty
{
    public IUClass PropertyClass => _factory.CreateUClass((nint)((FClassProperty*)Ptr)->_super.prop_class);
    public IUClass? MetaClass => _factory.CreateUClass((nint)((FClassProperty*)Ptr)->meta);
}

public unsafe class FSoftClassPropertyUE4_27_2(nint ptr, IUnrealFactory factory)
    : FPropertyUE4_27_2(ptr, factory), IFSoftClassProperty
{
    public IUClass MetaClass => _factory.CreateUClass((nint)((FClassProperty*)Ptr)->meta);
}

public unsafe class FObjectPropertyUE4_27_2(nint ptr, IUnrealFactory factory)
    : FPropertyUE4_27_2(ptr, factory), IFObjectProperty
{
    public IUClass PropertyClass => _factory.CreateUClass((nint)((FObjectProperty*)Ptr)->prop_class);
}

public unsafe class FEnumPropertyUE4_27_2(nint ptr, IUnrealFactory factory)
    : FPropertyUE4_27_2(ptr, factory), IFEnumProperty
{
    private readonly FEnumProperty* _self = (FEnumProperty*)ptr;

    public IUEnum Enum => _factory.CreateUEnum((nint)_self->enum_data);
    public IFProperty UnderlyingProp => _factory.CreateFProperty((nint)_self->underlying_type);
}

public unsafe class FFieldClassUE4_27_2(nint ptr, IUnrealFactory factory)
    : IFFieldClass
{
    private readonly FFieldClass* _self = (FFieldClass*)ptr;
    
    public nint Ptr => ptr;
    public string Name => _self->name.ToString();
    public ulong Id => _self->Id;
    public ulong CastFlags => _self->CastFlags;
    public EClassFlags ClassFlags => _self->ClassFlags;
    public IFFieldClass SuperClass => factory.CreateFFieldClass((nint)_self->super);
    public IFField DefaultObject => factory.CreateFField((nint)_self->default_obj);
    public nint FieldConstructor => _self->ctor;
}

public unsafe class FFieldUE4_27_2(nint ptr, IUnrealFactory factory)
    : IFField
{
    private readonly FField* _self = (FField*)ptr;
    protected readonly IUnrealFactory _factory = factory;

    public nint Ptr => ptr;
    public nint VTable => _self->_vtable;
    public IFFieldClass ClassPrivate => _factory.CreateFFieldClass((nint)_self->class_private);
    public IFFieldVariant Owner => factory.CreateFFieldVariant((nint)(&_self->owner));
    public IFField? Next => _self->next != null ? _factory.CreateFField((nint)_self->next) : null;
    public string NamePrivate => _self->name_private.ToString();
    public EObjectFlags FlagsPrivate => _self->flags_private;

    public void SetOwnerUObject(IUObject owner)
    {
        _self->owner.Object = (UObjectBase*)owner.Ptr;
        _self->owner.bIsUObject = true;
    }

    public void SetOwnerFField(IFField owner)
    {
        _self->owner.Field = (FField*)owner.Ptr;
        _self->owner.bIsUObject = false;
    }
}

public unsafe class FFieldVariantUE4_27_2(nint ptr, IUnrealFactory factory)
    : IFFieldVariant
{
    private readonly FFieldObjectUnion* _self = (FFieldObjectUnion*)ptr;
    protected readonly IUnrealFactory _factory = factory;

    public nint Ptr => ptr;
    public IFField? Field => _self->Field != null ? _factory.CreateFField((nint)_self->Field) : null;
    public IUObject? Object => _self->Object != null ? _factory.CreateUObject((nint)_self->Object) : null;
    public bool IsObject => _self->bIsUObject;
}

public unsafe class FPropertyUE4_27_2(nint ptr, IUnrealFactory factory)
    : FFieldUE4_27_2(ptr, factory), IFProperty
{
    private readonly FProperty* _self = (FProperty*)ptr;

    public int ArrayDim => _self->array_dim;
    public int ElementSize => _self->element_size;
    public EPropertyFlags PropertyFlags => _self->property_flags;
    public ushort RepIndex => _self->rep_index;
    public byte BlueprintReplicationCondition => _self->blueprint_rep_cond;
    public int Offset_Internal => _self->offset_internal;
    public IEnumerable<IFProperty> PropertyLinkNext
        => new IFPropertyEnumerable(_factory.CreateFProperty((nint)_self->prop_link_next), PropertyType.PropertyLink,
            _factory);
    public IEnumerable<IFProperty> NextRef
        => new IFPropertyEnumerable(_factory.CreateFProperty((nint)_self->next_ref), PropertyType.NextRef,
            _factory);
    public IEnumerable<IFProperty> DestructorLinkNext
        => new IFPropertyEnumerable(_factory.CreateFProperty((nint)_self->dtor_link_next), PropertyType.DestructorLink,
            _factory);
    public IEnumerable<IFProperty> PostConstructLinkNext
        => new IFPropertyEnumerable(_factory.CreateFProperty((nint)_self->post_ct_link_next), PropertyType.PostConstructLink,
            _factory);
    public string RepNotifyFunc => _self->rep_notify_func.ToString();
}

public unsafe class IFPropertyEnumerable(IFProperty initial, PropertyType propType, IUnrealFactory factory)
    : IEnumerator<IFProperty>, IEnumerable<IFProperty>
{
    private FProperty* _current = (FProperty*)initial.Ptr;
    private bool _isInitial = true;
    
    public bool MoveNext()
    {
        if (_isInitial)
        {
            _isInitial = false;
            return _current != null;
        }
        
        if (_current == null) return false;
        switch (propType)
        {
            case PropertyType.PropertyLink:
                _current = _current->prop_link_next;
                break;
            case PropertyType.NextRef:
                _current = _current->next_ref;
                break;
            case PropertyType.DestructorLink:
                _current = _current->dtor_link_next;
                break;
            case PropertyType.PostConstructLink:
                _current = _current->post_ct_link_next;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(propType), propType, null);
        }

        return _current != null;
    }

    public void Reset() => _current = (FProperty*)initial.Ptr;

    public IFProperty Current => factory.CreateFProperty((nint)_current);

    object? IEnumerator.Current => Current;

    public void Dispose() { }

    public IEnumerator<IFProperty> GetEnumerator() => this;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public unsafe class FBoolPropertyUE4_27_2(nint ptr, IUnrealFactory factory)
    : FPropertyUE4_27_2(ptr, factory), IFBoolProperty
{
    public byte FieldSize => ((FBoolProperty*)Ptr)->field_size;
    public byte ByteOffset => ((FBoolProperty*)Ptr)->byte_offset;
    public byte ByteMask => ((FBoolProperty*)Ptr)->byte_mask;
    public byte FieldMask => ((FBoolProperty*)Ptr)->field_mask;
}

public unsafe class FBytePropertyUE4_27_2(nint ptr, IUnrealFactory factory)
    : FPropertyUE4_27_2(ptr, factory), IFByteProperty
{
    public IUEnum? Enum => ((FByteProperty*)Ptr)->enum_data != null ? _factory.CreateUEnum((nint)((FByteProperty*)Ptr)->enum_data) : null;
}

public unsafe class UUserDefinedEnumUE4_27_2(nint ptr, IUnrealFactory factory, IUnrealMemoryInternal memory)
    : UEnumUE4_27_2(ptr, factory, memory), IUUserDefinedEnum
{
    public TMap<FName, FText> DisplayNameMap => *(TMap<FName, FText>*)(&((UUserDefinedEnum*)ptr)->DisplayNameMap);
}

public unsafe class UEnumUE4_27_2(nint ptr, IUnrealFactory factory, IUnrealMemoryInternal memory)
    : UFieldUE4_27_2(ptr, factory, memory), IUEnum
{
    private readonly UEnum* _self = (UEnum*)ptr;
    public string CppType => _self->cpp_type.ToString();
    public UE.Toolkit.Core.Types.Unreal.UE5_4_4.TArray<UE.Toolkit.Core.Types.Unreal.UE5_4_4.TPair<FName, long>> Names => _self->entries;

    public bool TryParse(string name, bool ignoreCase, [NotNullWhen(true)] out long? value)
    {
        value = null;
        if (ignoreCase)
        {
            name = name.ToLower();
        }
        var NamesArray = new TArrayList<TPair<FName, long>>(&_self->entries, _factory.Memory);
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

public unsafe class UScriptStructUE4_27_2(nint ptr, IUnrealFactory factory, IUnrealMemoryInternal memory)
    : UStructUE4_27_2(ptr, factory, memory), IUScriptStruct
{
    private readonly UScriptStruct* _self = (UScriptStruct*)ptr;
    public EStructFlags StructFlags => (EStructFlags)_self->flags;
    public bool bPrepareCppStructOpsCompleted => _self->b_prepare_cpp_struct_ops_completed;
    public nint CppStructOps => _self->cpp_struct_ops;
}

public unsafe class UStructUE4_27_2(nint ptr, IUnrealFactory factory, IUnrealMemoryInternal memory)
    : UFieldUE4_27_2(ptr, factory, memory), IUStruct
{
    private readonly UStruct* _self = (UStruct*)ptr;

    public IUStruct? SuperStruct
        => _self->super_struct != null ? _factory.CreateUStruct((nint)_self->super_struct) : null;
    public IEnumerable<IUField> Children
        => new IUFieldEnumerable(_self->children != null ? _factory.CreateUField((nint)_self->children) : null);
    public IEnumerable<IFField> ChildProperties =>
        new IFFieldEnumerable(_self->child_properties != null ? _factory.CreateFField((nint)_self->child_properties) : null);
    public int PropertiesSize => _self->properties_size;
    public int MinAlignment => _self->min_alignment;
    public UE.Toolkit.Core.Types.Unreal.UE5_4_4.TArray<byte> Script { get; } = new();

    public IEnumerable<IFProperty> PropertyLink
        => new IFPropertyEnumerable(_factory.CreateFProperty((nint)_self->prop_link), PropertyType.PropertyLink,
            _factory);
    
    public IEnumerable<IFProperty> RefLink
        => new IFPropertyEnumerable(_factory.CreateFProperty((nint)_self->ref_link), PropertyType.NextRef,
            _factory);
    public IEnumerable<IFProperty> DestructorLink
        => new IFPropertyEnumerable(_factory.CreateFProperty((nint)_self->dtor_link), PropertyType.DestructorLink,
            _factory);
    public IEnumerable<IFProperty> PostConstructLink
        => new IFPropertyEnumerable(_factory.CreateFProperty((nint)_self->post_ct_link), PropertyType.PostConstructLink,
            _factory);
}

public class IFFieldEnumerable(IFField? initial)
    : IEnumerator<IFField>, IEnumerable<IFField>
{
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

    public void Reset() => _current = initial;

    public IFField Current => _current!;

    object? IEnumerator.Current => Current;

    public void Dispose() { }

    public IEnumerator<IFField> GetEnumerator() => this;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class IUFieldEnumerable(IUField? initial)
    : IEnumerator<IUField>, IEnumerable<IUField>
{
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

    public void Reset() => _current = initial;

    public IUField Current => _current!;

    object? IEnumerator.Current => Current;

    public void Dispose() { }
    
    public IEnumerator<IUField> GetEnumerator() => this;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public unsafe class UFieldUE4_27_2(nint ptr, IUnrealFactory factory, IUnrealMemoryInternal memory)
    : UObjectUE4_27_2(ptr, factory, memory), IUField
{
    private readonly UField* _self = (UField*)ptr;

    public IUField? Next
        => _self->next != null ? _factory.CreateUField((nint)_self->next) : null;
}

public unsafe class UObjectUE4_27_2(nint ptr, IUnrealFactory factory, IUnrealMemoryInternal memory) 
    : BaseUObject<UObjectBase>(ptr, factory, memory)
{
    public override nint VTable => _self->_vtable;

    public override EObjectFlags ObjectFlags => _self->ObjectFlags;

    public override int InternalIndex => (int)_self->InternalIndex;

    public override IUClass ClassPrivate => _factory.CreateUClass((nint)_self->ClassPrivate);

    public override FName NamePrivate => _self->NamePrivate;

    public override IUObject? OuterPrivate => _self->OuterPrivate != null ? _factory.CreateUObject((nint)_self->OuterPrivate) : null;

    public override bool IsChildOf(string type) => _self->IsChildOf(type);

    public override IUObject GetOutermost() => _factory.CreateUObject((nint)_self->GetOutermost());

    public override string GetNativeName() => ToolkitUtils.GetNativeName(this);

    public override string GetPathName() => ToolkitUtils.GetPathName(this);
}

public unsafe class UClassUE4_27_2(nint ptr, IUnrealFactory factory, IUnrealMemoryInternal memory)
    : UStructUE4_27_2(ptr, factory, memory), IUClass
{
    private readonly UClass* _self = (UClass*)ptr;

    public IUClass? GetSuperClass()
        => _self->_super.super_struct != null ? _factory.CreateUClass((nint)_self->_super.super_struct) : null;

    public IUFunction? GetFunction(string Name)
    {
        var FuncMapDict = new TMapDictionary<FName, Ptr<UFunction>>(
            (TMap<FName, Ptr<UFunction>>*)(&_self->func_map),
            _factory.Memory
        );
        return FuncMapDict.TryGetValue(new(Name), out var Function)
            ? factory.CreateUFunction((nint)Function.Value->Value)
            : null;
    }

    public IEnumerable<IUFunction> GetFunctions()
    {
        var FuncMapDict = new TMapDictionary<FName, Ptr<UFunction>>(
            (TMap<FName, Ptr<UFunction>>*)(&_self->func_map),
            _factory.Memory
        );
        return FuncMapDict.Values.Select(x => _factory.CreateUFunction((nint)x.Value->Value));
    }

    public IUObject? ClassDefaultObject 
        => _self->class_default_obj != null ? factory.CreateUObject((nint)_self->class_default_obj) : null;

    public nint Constructor => _self->class_ctor;
    
    public EClassFlags ClassFlags => _self->class_flags;
    
    public EClassCastFlags ClassCastFlags => _self->class_cast_flags;
}

public unsafe class UFunctionUE4_27_2(nint ptr, IUnrealFactory factory, IUnrealMemoryInternal memory)
    : UStructUE4_27_2(ptr, factory, memory), IUFunction
{
    private readonly UFunction* _self = (UFunction*)ptr;

    public EFunctionFlags FunctionFlags => _self->func_flags;
    public int ParamCount => _self->num_params;
    public int ParamSize => _self->params_size;
    public int ReturnValueOffset => _self->return_value_offset;

    public int GetTotalParameterSize()
    {
        var LastProperty = ChildProperties.Any() ? _factory.CreateFProperty(ChildProperties.Last().Ptr) : null;
        return LastProperty != null ? LastProperty.Offset_Internal + LastProperty.ElementSize : 0;
    }

    public nint FunctionPtr => _self->exec_func_ptr;
}

public unsafe class UObjectArrayUE4_27_2(nint ptr, IUnrealFactory factory) : IUObjectArray
{
    private readonly FUObjectArray_Pack4* _self = (FUObjectArray_Pack4*)ptr;

    public int ObjLastNonGCIndex => _self->ObjLastNonGCIndex;

    public IUObject? IndexToObject(int idx)
    {
        var objItem = _self->ObjObjects.GetItem(idx);
        if (objItem == null || objItem->Object == null || objItem->Flags.HasFlag(EInternalObjectFlags.Unreachable)) return null;
        return factory.CreateUObject((nint)objItem->Object);
    }

    public int NumElements => _self->ObjObjects.NumElements;

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

public unsafe class FStructParamsUE4_27_2(nint ptr, IUnrealFactory factory) : IFStructParams
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

public unsafe class FPropertyParamsUE4_27_2(nint ptr, IUnrealFactory factory) : IFPropertyParams
{
    private readonly FPropertyParamsBase* _self = (FPropertyParamsBase*)ptr;
    protected readonly IUnrealFactory _factory = factory;   
    
    public nint Ptr => (nint)_self;
    public string Name => Marshal.PtrToStringUTF8(_self->NameUTF8)!;
    public EPropertyFlags PropertyFlags => _self->PropertyFlags;
    public EPropertyGenFlags GenFlags => _self->PropertyGenFlags;
    public EObjectFlags ObjectFlags => _self->ObjectFlags;
}

public unsafe class FGenericPropertyParamsUE4_27_2(nint ptr, IUnrealFactory factory) 
    : FPropertyParamsUE4_27_2(ptr, factory), IFGenericPropertyParams
{
    private readonly FGenericPropertyParams* _self = (FGenericPropertyParams*)ptr;

    public int ArrayDim => _self->Super.ArrayDim;
    public int Offset => _self->Super.Offset;
}

public unsafe class FWorldContextUE4_27_2(nint ptr, IUnrealFactory factory) : IFWorldContext
{
    protected readonly IUnrealFactory _factory = factory;
    private readonly FWorldContext* _self = (FWorldContext*)ptr;
    public nint Ptr => (nint)_self;
    public WorldType GetWorldType() => _self->WorldType;
    public nint GetWorld() => (nint)_self->ThisCurrentWorld;
}

public unsafe class FWorldContextEnumerator(UEngineUE4_27_2 owner, IUnrealFactory factory) 
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

public unsafe class UEngineUE4_27_2(nint ptr, IUnrealFactory factory, IUnrealMemoryInternal memory) 
    : UObjectUE4_27_2(ptr, factory, memory), IUEngine
{
    private readonly UEngine* _self = (UEngine*)ptr;

    internal TArray<Ptr<FWorldContext>>* GetWorldListInner() => &_self->WorldList;
    
    public IEnumerable<IFWorldContext> GetWorldList() => new FWorldContextEnumerator(this, factory);
}

public unsafe class FStaticConstructObjectParametersUE4_27_2
    : IFStaticConstructObjectParameters, IDisposable
{
    private readonly FStaticConstructObjectParameters* _self;
    public nint Ptr => (nint)_self;
    protected readonly IUnrealFactory _factory;
    private bool Disposed = false;

    public FStaticConstructObjectParametersUE4_27_2(IUnrealFactory factory)
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

    ~FStaticConstructObjectParametersUE4_27_2() => Disposing();
    
    #endregion
}

public unsafe class FActorSpawnParametersUE4_27_2
    : IFActorSpawnParameters, IDisposable
{
    private readonly FActorSpawnParameters* _self;
    public nint Ptr => (nint)_self;
    protected readonly IUnrealFactory _factory;
    private bool Disposed = false;

    public FActorSpawnParametersUE4_27_2(IUnrealFactory factory)
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

    ~FActorSpawnParametersUE4_27_2() => Disposing();
    
    #endregion
}

public unsafe class UGameInstanceUE4_27_2(nint ptr, IUnrealFactory factory, IUnrealMemoryInternal memory)
    : UObjectUE4_27_2(ptr, factory, memory), IUGameInstance
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

public unsafe class FDelegatePropertyUE4_27_2(nint ptr, IUnrealFactory factory)
    : FPropertyUE4_27_2(ptr, factory), IFDelegateProperty 
{
    public IUFunction? Function => ((FDelegateProperty*)Ptr)->func != null ? _factory.CreateUFunction((nint)((FDelegateProperty*)Ptr)->func) : null;
}