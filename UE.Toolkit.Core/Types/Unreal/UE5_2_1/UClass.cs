using System.Runtime.InteropServices;
using UE.Toolkit.Core.Types.Unreal.UE4_27_2;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;
using UField = UE.Toolkit.Core.Types.Unreal.UE4_27_2.UField;
using UObjectBase = UE.Toolkit.Core.Types.Unreal.UE4_27_2.UObjectBase;
using UScriptStruct = UE.Toolkit.Core.Types.Unreal.UE4_27_2.UScriptStruct;
using UStruct = UE.Toolkit.Core.Types.Unreal.UE4_27_2.UStruct;

namespace UE.Toolkit.Core.Types.Unreal.UE5_2_1;

[StructLayout(LayoutKind.Explicit, Size = 0x220)]
public unsafe struct UClass
{
    [FieldOffset(0x0)] public UStruct _super;
    [FieldOffset(0xb0)] public IntPtr class_ctor; // InternalConstructor<class_UClassName> => UClassName::UClassName
    [FieldOffset(0xb8)] public IntPtr class_vtable_helper_ctor_caller;
    [FieldOffset(0xc0)] public IntPtr class_add_ref_objects;
    [FieldOffset(0xc8)] public uint class_status; // ClassUnique : 31, bCooked : 1
    [FieldOffset(0xcc)] public uint FirstOwnedClassRep;
    [FieldOffset(0xd0)] public bool bCooked;
    [FieldOffset(0xd1)] public bool bLayoutChanging;
    [FieldOffset(0xd4)] public EClassFlags class_flags;
    [FieldOffset(0xd8)] public EClassCastFlags class_cast_flags;
    [FieldOffset(0xe0)] public UClass* class_within; // type of object containing the current object
    [FieldOffset(0xe8)] public FName class_conf_name;
    [FieldOffset(0x100)] public TArray<UField> net_fields;
    [FieldOffset(0x110)] public UObjectBase* class_default_obj; // Default object of type described in UClass instance
    [FieldOffset(0x118)] public nint sparse_class_data;
    [FieldOffset(0x120)] public UScriptStruct* sparse_class_data_struct;
    [FieldOffset(0x128)] public TMap func_map;
    [FieldOffset(0x180)] public TMap super_func_map;
    [FieldOffset(0x1d8)] public TArray<IntPtr> interfaces;
    [FieldOffset(0x210)] public TArray<FNativeFunctionLookup> native_func_lookup;
}