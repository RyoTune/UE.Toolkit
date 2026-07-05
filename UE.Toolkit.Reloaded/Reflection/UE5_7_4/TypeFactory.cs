using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UE.Toolkit.Core.Types.Unreal.Factories;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;
using UE.Toolkit.Interfaces;
using UE.Toolkit.Reloaded.Reflection.Common;

namespace UE.Toolkit.Reloaded.Reflection.UE5_7_4;

public class TypeFactory(IUnrealFactory factory, IUnrealMemory memory, 
    IUnrealClasses classes, IPropertyFlagsBuilder flags) 
    : BaseTypeFactory(factory, memory, classes, flags)
{
    private static Func<nint, int, nint>? FMemory_Malloc_Static = null;
    private static nint Malloc_Static_Size = 0;
    
    private static nint CurrentCppStructOpsVtable;
    private static uint CurrentCppStructOpsSize;
    private const uint CPP_STRUCT_OPS_ALIGNMENT = 0x8;
    private const nint CPP_STRUCT_OPS_VTABLE_SIZE = 20;
    
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static unsafe nint InitializeCppStructOps()
    {
        var Alloc = (ICppStructOps*)FMemory_Malloc_Static!(Marshal.SizeOf<ICppStructOps>(), 0);
        Alloc->VTable = CurrentCppStructOpsVtable;
        Alloc->Size = CurrentCppStructOpsSize;
        Alloc->Alignment = CPP_STRUCT_OPS_ALIGNMENT;
        return (nint)Alloc;   
    }
    
    // UScriptStruct::ICppStructOps
    private unsafe void CreateCppStructOpsVtable()
    {
        ((nint*)CurrentCppStructOpsVtable)[0] = (nint)(delegate* unmanaged[Stdcall]<void>)(&FunctionPointers.Noop_Void); // ~ICppStructOps
        ((nint*)CurrentCppStructOpsVtable)[1] = (nint)(delegate* unmanaged[Stdcall]<nint, FCapabilities*, FCapabilities*>)(&FunctionPointers.Noop_GetCapabilities); // GetCapabilities
        ((nint*)CurrentCppStructOpsVtable)[2] = (nint)(delegate* unmanaged[Stdcall]<void>)(&FunctionPointers.Noop_Void); // Construct
        ((nint*)CurrentCppStructOpsVtable)[3] = (nint)(delegate* unmanaged[Stdcall]<void>)(&FunctionPointers.Noop_Void); // ConstructForTests 
        ((nint*)CurrentCppStructOpsVtable)[4] = (nint)(delegate* unmanaged[Stdcall]<void>)(&FunctionPointers.Noop_Void); // Destruct 
        ((nint*)CurrentCppStructOpsVtable)[5] = (nint)(delegate* unmanaged[Stdcall]<byte>)(&FunctionPointers.Noop_Bool_False); // Serialize(FArchive) 
        ((nint*)CurrentCppStructOpsVtable)[6] = (nint)(delegate* unmanaged[Stdcall]<byte>)(&FunctionPointers.Noop_Bool_False); // Serialize(FStructuredArchive)
        ((nint*)CurrentCppStructOpsVtable)[7] = (nint)(delegate* unmanaged[Stdcall]<void>)(&FunctionPointers.Noop_Void); // PostSerialize 
        ((nint*)CurrentCppStructOpsVtable)[8] = (nint)(delegate* unmanaged[Stdcall]<byte>)(&FunctionPointers.Noop_Bool_False); // NetSerialize
        ((nint*)CurrentCppStructOpsVtable)[9] = (nint)(delegate* unmanaged[Stdcall]<byte>)(&FunctionPointers.Noop_Bool_False); // NetDeltaSerialize
        ((nint*)CurrentCppStructOpsVtable)[10] = (nint)(delegate* unmanaged[Stdcall]<void>)(&FunctionPointers.Noop_Void); // PostScriptConstruct 
        ((nint*)CurrentCppStructOpsVtable)[11] = (nint)(delegate* unmanaged[Stdcall]<void>)(&FunctionPointers.Noop_Void); // GetPreloadDependencies
        ((nint*)CurrentCppStructOpsVtable)[12] = (nint)(delegate* unmanaged[Stdcall]<byte>)(&FunctionPointers.Noop_Bool_False); // Copy
        ((nint*)CurrentCppStructOpsVtable)[13] = (nint)(delegate* unmanaged[Stdcall]<byte>)(&FunctionPointers.Noop_Bool_False); // Identical
        ((nint*)CurrentCppStructOpsVtable)[14] = (nint)(delegate* unmanaged[Stdcall]<byte>)(&FunctionPointers.Noop_Bool_False); // ExportTextItem
        ((nint*)CurrentCppStructOpsVtable)[15] = (nint)(delegate* unmanaged[Stdcall]<byte>)(&FunctionPointers.Noop_Bool_False); // ImportTextItem
        ((nint*)CurrentCppStructOpsVtable)[16] = (nint)(delegate* unmanaged[Stdcall]<void>)(&FunctionPointers.Noop_Void); // AddStructReferencedObjects
        ((nint*)CurrentCppStructOpsVtable)[17] = (nint)(delegate* unmanaged[Stdcall]<byte>)(&FunctionPointers.Noop_Bool_False); // SerializeFromMismatchedTag
        ((nint*)CurrentCppStructOpsVtable)[18] = (nint)(delegate* unmanaged[Stdcall]<byte>)(&FunctionPointers.Noop_Bool_False); // StructuredSerializeFromMismatchedTag
        ((nint*)CurrentCppStructOpsVtable)[19] = (nint)(delegate* unmanaged[Stdcall]<uint>)(&FunctionPointers.Noop_U32); // GetStructTypeHash
    }
    
    internal override unsafe bool CreateStructParam(string Name, int Size,
        List<IFPropertyParams> Fields, out IFStructParams? Out)
    {
        var pProperties = (FPropertyParamsBase**)Memory.MallocZeroed(Marshal.SizeOf<nint>() * Fields.Count);
        var StructParamStatic = (FStructParams*)Memory.MallocZeroed(Marshal.SizeOf<FStructParams>());
        // Retrieve the game package so the reference to it is valid when FStructProperty::OuterFunc is called
        _ = Classes.GetGamePackage();
        StructParamStatic->OuterFunc = (nint)(delegate* unmanaged[Stdcall]<nint>)(&Unreal.UnrealClasses.FStructProperty_OuterFunc_Callback);
        StructParamStatic->SuperFunc = nint.Zero;
        StructParamStatic->StructOpsFunc = (nint)(delegate* unmanaged[Stdcall]<nint>)(&InitializeCppStructOps);
        StructParamStatic->NameUTF8 = Marshal.StringToHGlobalAnsi(Name);
        StructParamStatic->SizeOf = (ushort)Size;
        StructParamStatic->AlignOf = (byte)CPP_STRUCT_OPS_ALIGNMENT; // alignof(nint)
        StructParamStatic->Properties = pProperties;
        StructParamStatic->NumProperties = (ushort)Fields.Count;
        StructParamStatic->ObjectFlags = EObjectFlags.RF_Public | EObjectFlags.RF_MarkAsNative | EObjectFlags.RF_Transient;
        StructParamStatic->StructFlags = EStructFlags.STRUCT_Native;
        foreach (var (i, Field) in Fields.Select((x, i) => (i, x)))
            StructParamStatic->Properties[i] = (FPropertyParamsBase*)Field.Ptr;
        Out = Factory.CreateFStructParams((nint)StructParamStatic);
        FMemory_Malloc_Static ??= (size, align) => Memory.MallocZeroed(size, align);
        // sizeof(UScriptStruct::ICppStructOps::VTable)
        CurrentCppStructOpsVtable = Memory.MallocZeroed(Marshal.SizeOf<nint>() * CPP_STRUCT_OPS_VTABLE_SIZE);
        CurrentCppStructOpsSize = (uint)Size;
        CreateCppStructOpsVtable();
        return true;
    }
    
    private unsafe bool CreateGenericParam(string Name, int Offset,
        EPropertyGenFlags GenFlags, out IFGenericPropertyParams? Out)
    {
        var PropParamStatic = (FGenericPropertyParams*)Memory.Malloc(Marshal.SizeOf<FGenericPropertyParams>()); 
        PropParamStatic->Super.NameUTF8 = Marshal.StringToHGlobalAnsi(Name);
        PropParamStatic->Super.RepNotifyFuncUTF8 = nint.Zero;
        PropParamStatic->Super.PropertyFlags = Flags.CreatePropertyFlags(PropertyVisibility.Public, PropertyBuilderFlags.None);
        PropParamStatic->Super.PropertyGenFlags = GenFlags;
        PropParamStatic->Super.ObjectFlags = EObjectFlags.RF_Public | EObjectFlags.RF_MarkAsNative | 
                                             EObjectFlags.RF_Transient;
        PropParamStatic->Super.ArrayDim = 1;
        PropParamStatic->Super.Offset = (ushort)Offset;
        Out = Factory.CreateFGenericPropertyParams((nint)PropParamStatic);
        return true;
    } 
    
    public override bool CreateI8Param(string Name, int Offset, out IFGenericPropertyParams? Out)
        => CreateGenericParam(Name, Offset, EPropertyGenFlags.Int8, out Out);
    
    public override bool CreateI16Param(string Name, int Offset, out IFGenericPropertyParams? Out)
        => CreateGenericParam(Name, Offset, EPropertyGenFlags.Int16, out Out);
    
    public override bool CreateI32Param(string Name, int Offset, out IFGenericPropertyParams? Out)
        => CreateGenericParam(Name, Offset, EPropertyGenFlags.Int, out Out);
    
    public override bool CreateI64Param(string Name, int Offset, out IFGenericPropertyParams? Out)
        => CreateGenericParam(Name, Offset, EPropertyGenFlags.Int64, out Out);
    
    public override bool CreateU8Param(string Name, int Offset, out IFGenericPropertyParams? Out)
        => CreateGenericParam(Name, Offset, EPropertyGenFlags.Int8, out Out);
    
    public override bool CreateU16Param(string Name, int Offset, out IFGenericPropertyParams? Out)
        => CreateGenericParam(Name, Offset, EPropertyGenFlags.UInt16, out Out);
    
    public override bool CreateU32Param(string Name, int Offset, out IFGenericPropertyParams? Out)
        => CreateGenericParam(Name, Offset, EPropertyGenFlags.UInt32, out Out);
    
    public override bool CreateU64Param(string Name, int Offset, out IFGenericPropertyParams? Out)
        => CreateGenericParam(Name, Offset, EPropertyGenFlags.UInt64, out Out);
    
    public override bool CreateF32Param(string Name, int Offset, out IFGenericPropertyParams? Out)
        => CreateGenericParam(Name, Offset, EPropertyGenFlags.Float, out Out);
    
    public override bool CreateF64Param(string Name, int Offset, out IFGenericPropertyParams? Out)
        => CreateGenericParam(Name, Offset, EPropertyGenFlags.Double, out Out);
}