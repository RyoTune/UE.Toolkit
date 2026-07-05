using System.Runtime.InteropServices;
using UE.Toolkit.Core.Types.Interfaces;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.Factories.UE5_4_4;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;
using FFieldClass = UE.Toolkit.Core.Types.Unreal.UE5_7_4.FFieldClass;

namespace UE.Toolkit.Core.Types.Unreal.Factories.UE5_7_4;

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
    public override IUObjectArray CreateUObjectArray(nint ptr) => new UObjectArray_UE5_7_4(ptr, this);
    public override IUObject CreateUObject(nint ptr) => new UObject_UE5_4_4(ptr, this);
    public override IUClass CreateUClass(nint ptr) => new UE5_6_1.UClass_UE5_6_1(ptr, this);
    public override IUScriptStruct CreateUScriptStruct(nint ptr) => new UE5_6_1.UScriptStruct_UE5_6_1(ptr, this);
    public override IUEnum CreateUEnum(nint ptr) => new UEnum_UE5_7_4(ptr, this);
    public override IUField CreateUField(nint ptr) => new UField_UE5_4_4(ptr, this);
    public override IUStruct CreateUStruct(nint ptr) => new UE5_6_1.UStruct_UE5_6_1(ptr, this);
    public override IUUserDefinedEnum CreateUUserDefinedEnum(nint ptr) => new UUserDefinedEnum_UE5_4_4(ptr, this);
    public override IUFunction CreateUFunction(nint ptr) => new UFunction_UE5_4_4(ptr, this);
    public override IFFieldClass CreateFFieldClass(nint ptr) => new FFieldClass_UE5_7_4(ptr, this);
    public override IFField CreateFField(nint ptr) => new FField_UE5_4_4(ptr, this);
    public override IFStructParams CreateFStructParams(nint ptr) => new FStructParams_UE5_4_4(ptr, this);
    public override IFPropertyParams CreateFPropertyParams(nint ptr) => new FPropertyParams_UE5_4_4(ptr, this);
    public override IFGenericPropertyParams CreateFGenericPropertyParams(nint ptr) => new FGenericPropertyParams_UE5_4_4(ptr, this);
    public override IFWorldContext CreateFWorldContext(nint ptr) => new FWorldContext_UE5_4_4(ptr, this);
    public override IUEngine CreateUEngine(nint ptr) => new UEngine_UE5_4_4(ptr, this);
    public override IUGameInstance CreateUGameInstance(nint ptr) => new UGameInstance_UE5_4_4(ptr, this);
    public override IFStaticConstructObjectParameters CreateFStaticConstructObjectParameters()
        => new FStaticConstructObjectParameters_UE5_4_4(this);
    public override IFActorSpawnParameters CreateFActorSpawnParameters()
        => new FActorSpawnParameters_UE5_4_4(this);
}

public unsafe class FFieldClass_UE5_7_4(nint ptr, IUnrealFactory factory)
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

public unsafe class UObjectArray_UE5_7_4(nint ptr, IUnrealFactory factory) : IUObjectArray
{
    private readonly FUObjectArray_UE5_7* _self = (FUObjectArray_UE5_7*)ptr;

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

public unsafe class UEnum_UE5_7_4(nint ptr, IUnrealFactory factory)
    : UField_UE5_4_4(ptr, factory), IUEnum, IDisposable
{
    private readonly Unreal.UE5_7_4.UEnum* _self = (Unreal.UE5_7_4.UEnum*)ptr;
    public string CppType => _self->CppType.ToString();

    private TArray<TPair<FName, long>>? CachedNames;
    private bool _isDisposed;

    private const nint MASK_POINTER = ~1;

    public TArray<TPair<FName, long>> Names
    {
        get
        {
            if (CachedNames == null)
            {
                var NamesToCache = new TArray<TPair<FName, long>>
                {
                    ArrayNum = _self->NameData.NumValues,
                    ArrayMax = _self->NameData.NumValues
                };
                NamesToCache.AllocatorInstance = (TPair<FName, long>*)_factory.Memory.Malloc(NamesToCache.ArrayNum * sizeof(TPair<FName, long>));
                // assert: This function should only be used after compiled-in enums have copied their static string names into FNames if mask pointer is off
                // extra tag bit to indicate if the pointer is dynamically allocated or static
                var pTaggedNames = (FName*)((nint)_self->NameData.TaggedNames & MASK_POINTER);
                var pTaggedValues = (long*)((nint)_self->NameData.TaggedValues & MASK_POINTER);
                for (var i = 0; i < NamesToCache.ArrayNum; i++)
                {
                    NamesToCache.AllocatorInstance[i].Key = pTaggedNames[i];
                    NamesToCache.AllocatorInstance[i].Value = pTaggedValues[i];
                }
                CachedNames = NamesToCache;
            }
            return CachedNames.Value;
        }
    }

    ~UEnum_UE5_7_4() => Dispose(false);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing && CachedNames.Value.AllocatorInstance != null)
            _factory.Memory.Free((nint)CachedNames.Value.AllocatorInstance);
    }
}