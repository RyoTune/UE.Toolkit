using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UE.Toolkit.Core.Types.Unreal.Factories;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Interfaces;
using UE.Toolkit.Reloaded.Reflection.Common;
using ICppStructOps = UE.Toolkit.Core.Types.Unreal.UE5_4_4.ICppStructOps;
using FStructParams = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FStructParams;
using FPropertyParamsBase = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FPropertyParamsBase;
using FGenericPropertyParams = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FGenericPropertyParams;
using EObjectFlags = UE.Toolkit.Core.Types.Unreal.UE5_4_4.EObjectFlags;
using EPropertyGenFlags = UE.Toolkit.Core.Types.Unreal.UE5_4_4.EPropertyGenFlags;
using EStructFlags = UE.Toolkit.Core.Types.Unreal.UE5_4_4.EStructFlags;

namespace UE.Toolkit.Reloaded.Reflection.UE4_27_2;

public class TypeFactory(IUnrealFactory factory, IUnrealMemory memory, 
    IUnrealClasses classes, IPropertyFlagsBuilder flags)
    : BaseTypeFactory(factory, memory, classes, flags)
{

    private static Func<nint, int, nint>? FMemory_Malloc_Static = null;
    private static nint Malloc_Static_Size = 0;

    private static nint CurrentCppStructOpsVtable;
    private static uint CurrentCppStructOpsSize;
    private const uint CPP_STRUCT_OPS_ALIGNMENT = 0x8;
    private const nint CPP_STRUCT_OPS_VTABLE_SIZE = 39;

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
        // 0-38
        ((nint*)CurrentCppStructOpsVtable)[0] = (nint)(delegate* unmanaged[Stdcall]<void>)(&FunctionPointers.Noop_Void); // ~ICppStructOps
        ((nint*)CurrentCppStructOpsVtable)[1] = (nint)(delegate* unmanaged[Stdcall]<byte>)(&FunctionPointers.Noop_Bool_False); // HasNoopConstructor
        ((nint*)CurrentCppStructOpsVtable)[2] = (nint)(delegate* unmanaged[Stdcall]<byte>)(&FunctionPointers.Noop_Bool_True); // HasZeroConstructor
        ((nint*)CurrentCppStructOpsVtable)[3] = (nint)(delegate* unmanaged[Stdcall]<void>)(&FunctionPointers.Noop_Void); // Construct
        ((nint*)CurrentCppStructOpsVtable)[4] = (nint)(delegate* unmanaged[Stdcall]<void>)(&FunctionPointers.Noop_Void); // ConstructForTests
        ((nint*)CurrentCppStructOpsVtable)[5] = (nint)(delegate* unmanaged[Stdcall]<byte>)(&FunctionPointers.Noop_Bool_False); // HasDestructor
        ((nint*)CurrentCppStructOpsVtable)[6] = (nint)(delegate* unmanaged[Stdcall]<void>)(&FunctionPointers.Noop_Void); // Destruct
        ((nint*)CurrentCppStructOpsVtable)[7] = (nint)(delegate* unmanaged[Stdcall]<byte>)(&FunctionPointers.Noop_Bool_False); // HasSerializer
        ((nint*)CurrentCppStructOpsVtable)[8] = (nint)(delegate* unmanaged[Stdcall]<byte>)(&FunctionPointers.Noop_Bool_False); // HasStructuredSerializer
        ((nint*)CurrentCppStructOpsVtable)[9] = (nint)(delegate* unmanaged[Stdcall]<byte>)(&FunctionPointers.Noop_Bool_False); // Serialize(FArchive)
        ((nint*)CurrentCppStructOpsVtable)[10] = (nint)(delegate* unmanaged[Stdcall]<byte>)(&FunctionPointers.Noop_Bool_False); // Serialize(FStructedArchive::FSlot)
        ((nint*)CurrentCppStructOpsVtable)[11] = (nint)(delegate* unmanaged[Stdcall]<byte>)(&FunctionPointers.Noop_Bool_False); // HasPostSerialize
        ((nint*)CurrentCppStructOpsVtable)[12] = (nint)(delegate* unmanaged[Stdcall]<void>)(&FunctionPointers.Noop_Void); // PostSerializer
        ((nint*)CurrentCppStructOpsVtable)[13] = (nint)(delegate* unmanaged[Stdcall]<byte>)(&FunctionPointers.Noop_Bool_False); // HasNetSerializer
        ((nint*)CurrentCppStructOpsVtable)[14] = (nint)(delegate* unmanaged[Stdcall]<byte>)(&FunctionPointers.Noop_Bool_False); // HasNetSharedSerialization
        ((nint*)CurrentCppStructOpsVtable)[15] = (nint)(delegate* unmanaged[Stdcall]<void>)(&FunctionPointers.Noop_Void); // NetSerializer
        ((nint*)CurrentCppStructOpsVtable)[16] = (nint)(delegate* unmanaged[Stdcall]<byte>)(&FunctionPointers.Noop_Bool_False); // HasNetDeltaSerializer
        ((nint*)CurrentCppStructOpsVtable)[17] = (nint)(delegate* unmanaged[Stdcall]<void>)(&FunctionPointers.Noop_Void); // NetDeltaSerialize
        ((nint*)CurrentCppStructOpsVtable)[18] = (nint)(delegate* unmanaged[Stdcall]<byte>)(&FunctionPointers.Noop_Bool_False); // HasPostScriptConstruct
        ((nint*)CurrentCppStructOpsVtable)[19] = (nint)(delegate* unmanaged[Stdcall]<void>)(&FunctionPointers.Noop_Void); // PostScriptConstruct
        ((nint*)CurrentCppStructOpsVtable)[20] = (nint)(delegate* unmanaged[Stdcall]<byte>)(&FunctionPointers.Noop_Bool_True); // IsPlainOldData
        ((nint*)CurrentCppStructOpsVtable)[21] = (nint)(delegate* unmanaged[Stdcall]<byte>)(&FunctionPointers.Noop_Bool_True); // HasCopy
        ((nint*)CurrentCppStructOpsVtable)[22] = (nint)(delegate* unmanaged[Stdcall]<void>)(&FunctionPointers.Noop_Void); // Copy
        ((nint*)CurrentCppStructOpsVtable)[23] = (nint)(delegate* unmanaged[Stdcall]<byte>)(&FunctionPointers.Noop_Bool_False); // HasIdentical
        ((nint*)CurrentCppStructOpsVtable)[24] = (nint)(delegate* unmanaged[Stdcall]<byte>)(&FunctionPointers.Noop_Bool_False); // Identical
        ((nint*)CurrentCppStructOpsVtable)[25] = (nint)(delegate* unmanaged[Stdcall]<byte>)(&FunctionPointers.Noop_Bool_False); // HasExportTextItem
        ((nint*)CurrentCppStructOpsVtable)[26] = (nint)(delegate* unmanaged[Stdcall]<byte>)(&FunctionPointers.Noop_Bool_False); // ExportTextItem
        ((nint*)CurrentCppStructOpsVtable)[27] = (nint)(delegate* unmanaged[Stdcall]<byte>)(&FunctionPointers.Noop_Bool_False); // HasImportTextItem
        ((nint*)CurrentCppStructOpsVtable)[28] = (nint)(delegate* unmanaged[Stdcall]<byte>)(&FunctionPointers.Noop_Bool_False); // ImportTextItem
        ((nint*)CurrentCppStructOpsVtable)[29] = (nint)(delegate* unmanaged[Stdcall]<byte>)(&FunctionPointers.Noop_Bool_False); // HasAddStructReferencedObjects
        ((nint*)CurrentCppStructOpsVtable)[30] = (nint)(delegate* unmanaged[Stdcall]<void>)(&FunctionPointers.Noop_Void); // AddStructReferencedObjects
        ((nint*)CurrentCppStructOpsVtable)[31] = (nint)(delegate* unmanaged[Stdcall]<byte>)(&FunctionPointers.Noop_Bool_False); // HasSerializeFromMismatchTag
        ((nint*)CurrentCppStructOpsVtable)[32] = (nint)(delegate* unmanaged[Stdcall]<byte>)(&FunctionPointers.Noop_Bool_False); // HasStructuredSerializeFromMismatchedTag
        ((nint*)CurrentCppStructOpsVtable)[33] = (nint)(delegate* unmanaged[Stdcall]<byte>)(&FunctionPointers.Noop_Bool_False); // SerializeFroMismatchedTag
        ((nint*)CurrentCppStructOpsVtable)[34] = (nint)(delegate* unmanaged[Stdcall]<byte>)(&FunctionPointers.Noop_Bool_False); // StructuredSerializeFroMismatchedTag
        ((nint*)CurrentCppStructOpsVtable)[35] = (nint)(delegate* unmanaged[Stdcall]<byte>)(&FunctionPointers.Noop_Bool_False); // HasGetTypeHash
        ((nint*)CurrentCppStructOpsVtable)[36] = (nint)(delegate* unmanaged[Stdcall]<uint>)(&FunctionPointers.Noop_U32); // GetStructTypeHash
        ((nint*)CurrentCppStructOpsVtable)[37] = (nint)(delegate* unmanaged[Stdcall]<uint>)(&FunctionPointers.Noop_U32); // GetComputedPropertyFlags
        ((nint*)CurrentCppStructOpsVtable)[38] = (nint)(delegate* unmanaged[Stdcall]<byte>)(&FunctionPointers.Noop_Bool_False); // IsAbstract
    }

    internal override unsafe bool CreateStructParam(string Name, int Size,
        List<IFPropertyParams> Fields, out IFStructParams? Out)
    {
        var pProperties = (FPropertyParamsBase**)Memory.MallocZeroed(Marshal.SizeOf<nint>() * Fields.Count);
        var StructParamStatic = (FStructParams*)Memory.MallocZeroed(Marshal.SizeOf<FStructParams>()); 
        StructParamStatic->OuterFunc = (nint)(delegate* unmanaged[Stdcall]<nint>)(&Unreal.UnrealClasses.FStructProperty_OuterFunc_Callback);
        StructParamStatic->SuperFunc = nint.Zero;
        StructParamStatic->StructOpsFunc = (nint)(delegate* unmanaged[Stdcall]<nint>)(&InitializeCppStructOps);
        StructParamStatic->NameUTF8 = Marshal.StringToHGlobalAnsi(Name);
        StructParamStatic->SizeOf = (ulong)Size;
        StructParamStatic->AlignOf = CPP_STRUCT_OPS_ALIGNMENT; // alignof(nint)
        StructParamStatic->Properties = pProperties;
        StructParamStatic->NumProperties = Fields.Count;
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
        PropParamStatic->Super.Offset = Offset;
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