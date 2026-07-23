using System.Runtime.InteropServices;
using UE.Toolkit.Core.Common;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;
// ReSharper disable IdentifierTypo

namespace UE.Toolkit.Core.Types.Unreal.UE4_27_2;

/*
 * From Rirurin's Unreal object dumper (P3R/UE4.27.2): https://github.com/RyoTune/Unreal.ObjectDumpToJson
 */

// ====================
// OBJECT DUMPER STRUCTS
// ====================

[StructLayout(LayoutKind.Explicit, Size = 0x50)]
public struct TMap // : TSortableMapBase
{
    // TSparseArray fields (0x0 - 0x38)
    [FieldOffset(0x0)] public TArray<IntPtr> valueOffsets;
    // TBitArray<Allocator::BitArrayAllocator> allocationFlags @ 0x10
    [FieldOffset(0x30)] public int first_free_index;
    [FieldOffset(0x34)] public int num_free_indices;
    // TSet fields (0x38 - 0x50)
}

[StructLayout(LayoutKind.Explicit, Size = 0x230)]
public unsafe struct UClass
{
    [FieldOffset(0x0)] public UStruct _super;
    [FieldOffset(0xb0)] public IntPtr class_ctor; // InternalConstructor<class_UClassName> => UClassName::UClassName
    [FieldOffset(0xb8)] public IntPtr class_vtable_helper_ctor_caller;
    [FieldOffset(0xc0)] public IntPtr class_add_ref_objects;
    [FieldOffset(0xc8)] public uint class_status; // ClassUnique : 31, bCooked : 1
    [FieldOffset(0xcc)] public EClassFlags class_flags;
    [FieldOffset(0xd0)] public EClassCastFlags class_cast_flags;
    [FieldOffset(0xd8)] public UClass* class_within; // type of object containing the current object
    [FieldOffset(0xe0)] public UObjectBase* class_gen_by;
    [FieldOffset(0xe8)] public FName class_conf_name;
    [FieldOffset(0x100)] public TArray<UField> net_fields;
    [FieldOffset(0x118)] public UObjectBase* class_default_obj; // Default object of type described in UClass instance
    [FieldOffset(0x120)] public nint sparse_class_data;
    [FieldOffset(0x128)] public UScriptStruct* sparse_class_data_struct;
    [FieldOffset(0x130)] public TMap func_map;
    [FieldOffset(0x180)] public TMap super_func_map;
    [FieldOffset(0x1d8)] public TArray<IntPtr> interfaces;
    [FieldOffset(0x220)] public TArray<FNativeFunctionLookup> native_func_lookup;
}

[StructLayout(LayoutKind.Sequential, Size = 0x28)]
public unsafe struct UObjectBase
{
    private static readonly Dictionary<string, EClassCastFlags> CastFlagsMap
        = Enum.GetValues<EClassCastFlags>().ToDictionary(x => x.ToString(), x => x);
    
    private static readonly Dictionary<FName, HashSet<string>> StructChainMappings = [];
    
    public IntPtr _vtable; // @ 0x0
    public EObjectFlags ObjectFlags; // @ 0x8
    public uint InternalIndex; // @ 0xc
    public UClass* ClassPrivate; // @ 0x10 Type of this object. Used for reflection
    public FName NamePrivate; // @ 0x18
    public UObjectBase* OuterPrivate; // @ 0x20 Object that is containing this object
    
    public readonly UObjectBase* GetOutermost()
    {
        fixed (UObjectBase* obj = &this)
        {
            var currObj = obj;
            while (currObj->OuterPrivate != null) currObj = currObj->OuterPrivate;
            return currObj;
        }
    }

    public bool IsChildOf(string type)
    {
        var ofClassName = type;
        var hasPrefix = (ofClassName[0] == 'U' || ofClassName[0] == 'A' || ofClassName[0] == 'F') && char.IsUpper(ofClassName[1]); 
        if (hasPrefix) ofClassName = ofClassName[1..];

        if (ofClassName == ToolkitUtils.GetPrivateName((nint)ClassPrivate)) return true;

        // TODO: Optimize (again) by caching the result between types.
        var hasCastFlag = CastFlagsMap.TryGetValue(type, out var ofClassFlag);
        
        // If self is a UClass, first check own cast flags and
        // struct chain.
        if (ClassPrivate->class_cast_flags.HasFlag(EClassCastFlags.UClass))
        {
            fixed (UObjectBase* self = &this)
            {
                var selfClass = (UClass*)self;
                if (hasCastFlag)
                {
                    if ((selfClass->class_cast_flags & ofClassFlag) == ofClassFlag) return true;
                }
                
                // Populate struct chain data.
                // TODO: Possibly optimize it using NamePrivate to Struct* mappings but idc rn!
                InitStructChain((UStruct*)selfClass);
                if (StructChainMappings[NamePrivate].Contains(ofClassName)) return true;
            }
        }

        // Check object's private class cast flags.
        return hasCastFlag && (ClassPrivate->class_cast_flags & ofClassFlag) == ofClassFlag;
    }

    private static void InitStructChain(UStruct* baseStruct)
    {
        if (StructChainMappings.ContainsKey(baseStruct->_super._super.NamePrivate)) return;

        var names = new HashSet<string>();
        for (var tempStruct = baseStruct; tempStruct != null; tempStruct = tempStruct->super_struct)
        {
            names.Add(tempStruct->_super._super.NamePrivate.ToString());
        }

        StructChainMappings[baseStruct->_super._super.NamePrivate] = names;
    }
}
// Class data
[StructLayout(LayoutKind.Explicit, Size = 0x30)]
public unsafe struct UField
{
    [FieldOffset(0x0)] public UObjectBase _super;
    [FieldOffset(0x28)] public UField* next;
}
[StructLayout(LayoutKind.Explicit, Size = 0xb0)]
public unsafe struct UStruct
{
    [FieldOffset(0x0)] public UField _super;
    [FieldOffset(0x40)] public UStruct* super_struct;
    [FieldOffset(0x48)] public UField* children; // anything not a type field (e.g a class method) - beginning of linked list
    [FieldOffset(0x50)] public FField* child_properties; // the data model - beginning of linked list
    [FieldOffset(0x58)] public int properties_size;
    [FieldOffset(0x5c)] public int min_alignment;
    [FieldOffset(0x60)] public TArray<byte> Script;
    [FieldOffset(0x70)] public FProperty* prop_link;
    [FieldOffset(0x78)] public FProperty* ref_link;
    [FieldOffset(0x80)] public FProperty* dtor_link;
    [FieldOffset(0x88)] public FProperty* post_ct_link;
}

[StructLayout(LayoutKind.Explicit, Size = 0xc0)]
public struct UScriptStruct
{
    [FieldOffset(0x0)] public UStruct _super;
    [FieldOffset(0xb0)] public uint flags;
    [FieldOffset(0xb4)] public bool b_prepare_cpp_struct_ops_completed;
    [FieldOffset(0xb8)] public IntPtr cpp_struct_ops;
}

[StructLayout(LayoutKind.Explicit, Size = 0xe0)]
public unsafe struct UFunction
{
    [FieldOffset(0x0)] public UStruct _super;
    [FieldOffset(0xb0)] public EFunctionFlags func_flags;
    [FieldOffset(0xb4)] public byte num_params;
    [FieldOffset(0xb6)] public ushort params_size;
    [FieldOffset(0xb8)] public ushort return_value_offset;
    [FieldOffset(0xba)] public ushort rpc_id;
    [FieldOffset(0xbc)] public ushort rpc_response_id;
    [FieldOffset(0xc0)] public FProperty* first_prop_to_init;
    [FieldOffset(0xc8)] public UFunction* event_graph_func;
    [FieldOffset(0xd8)] public IntPtr exec_func_ptr;
}

[StructLayout(LayoutKind.Explicit, Size = 0x10)]
public struct FNativeFunctionLookup
{
    [FieldOffset(0x0)] FName name;
    [FieldOffset(0x8)] /*FNativeFuncPtr*/ nint Pointer;
}

[StructLayout(LayoutKind.Explicit, Size = 0x60)]
public struct UEnum
{
    [FieldOffset(0x0)] public UField _super;
    [FieldOffset(0x30)] public FString cpp_type;
    [FieldOffset(0x40)] public TArray<TPair<FName, long>> entries;
    [FieldOffset(0x58)] public IntPtr enum_disp_name_fn;
}

[StructLayout(LayoutKind.Explicit, Size = 0x40)]
public unsafe struct FFieldClass
{
    [FieldOffset(0x0)] public FName name;
    [FieldOffset(0x8)] public ulong Id;
    [FieldOffset(0x10)] public ulong CastFlags;
    [FieldOffset(0x18)] public EClassFlags ClassFlags;
    [FieldOffset(0x20)] public FFieldClass* super;
    [FieldOffset(0x28)] public FField* default_obj;
    [FieldOffset(0x30)] public IntPtr ctor; // [PropertyName]::Construct (e..g for ArrayProperty, this would be FArrayProperty::Construct)
}

[StructLayout(LayoutKind.Sequential, Size = 0x38)]
public unsafe struct FField
{
    public IntPtr _vtable; // @ 0x0
    public FFieldClass* class_private; // @ 0x8
    public FFieldObjectUnion owner; // @ 0x10
    public FField* next; // @ 0x20
    public FName name_private; // @ 0x28
    public EObjectFlags flags_private; // @ 0x30
}

[StructLayout(LayoutKind.Explicit, Size = 0x10)]
public unsafe struct FFieldObjectUnion
{
    [FieldOffset(0x0)] public FField* Field;
    [FieldOffset(0x0)] public UObjectBase* Object;
    [FieldOffset(0x8)] public bool bIsUObject;
}

[StructLayout(LayoutKind.Sequential, Size = 0x78)]
public unsafe struct FProperty
    // FInt8Property, FInt16Property, FIntProperty, FInt64Property
    // FUint8Property, FUint16Property, FUintProperty, FUint64Property
    // FFloatProperty, FDoubleProperty, FNameProperty, FStrProperty
{
    public FField _super; // @ 0x0
    public int array_dim; // @ 0x38
    public int element_size; // @ 0x3c
    public EPropertyFlags property_flags; // @ 0x40
    public ushort rep_index; // @ 0x48
    public byte blueprint_rep_cond; // @ 0x4a
    public byte Field4B; // @ 0x4b
    public int offset_internal; // @ 0x4c
    public FName rep_notify_func; // @ 0x50
    public FProperty* prop_link_next; // @ 0x58
    public FProperty* next_ref; // @ 0x60
    public FProperty* dtor_link_next; // @ 0x68
    public FProperty* post_ct_link_next; // @ 0x70
}

[StructLayout(LayoutKind.Sequential, Size = 0x80)]
public unsafe struct FByteProperty
{
    public FProperty _super; // @ 0x0
    public UEnum* enum_data; // @ 0x78 // TEnumAsByte<EEnum>
}

[StructLayout(LayoutKind.Sequential, Size = 0x80)]
public struct FBoolProperty
{
    public FProperty _super; // @ 0x0
    public byte field_size; // @ 0x78
    public byte byte_offset; // @ 0x79
    public byte byte_mask; // @ 0x7a
    public byte field_mask; // @ 0x7b
}

[StructLayout(LayoutKind.Sequential, Size = 0x80)]
public unsafe struct FObjectProperty
    // FObjectPtrProperty, FWeakObjectProperty, FLazyObjectProperty, FSoftObjectProperty, FInterfaceProperty
{
    // Defines a reference variable to another object
    public FProperty _super; // @ 0x0
    public UClass* prop_class; // @ 0x78
}

[StructLayout(LayoutKind.Sequential, Size = 0x88)]
public unsafe struct FClassProperty
    // FClassPtrProperty, FSoftClassProperty
{
    public FObjectProperty _super; // @ 0x0
    public UClass* meta; // @ 0x80
}

[StructLayout(LayoutKind.Sequential, Size = 0x88)]
public unsafe struct FArrayProperty
{
    public FProperty _super; // @ 0x0
    public FProperty* inner; // @ 0x78
    public uint flags; // @ 0x80
}

[StructLayout(LayoutKind.Sequential, Size = 0xa8)]
public unsafe struct FMapProperty
{
    public FProperty _super; // @ 0x0
    public FProperty* key_prop; // @ 0x78
    public FProperty* value_prop; // @ 0x80
}

[StructLayout(LayoutKind.Sequential, Size = 0x98)]
public unsafe struct FSetProperty
{
    public FProperty _super; // @ 0x0
    public FProperty* elem_prop; // @ 0x78
}

[StructLayout(LayoutKind.Sequential, Size = 0x80)]
public unsafe struct FStructProperty
{
    // Structure embedded inside an object
    public FProperty _super; // @ 0x0
    public UScriptStruct* struct_data; // @ 0x78
}

[StructLayout(LayoutKind.Sequential, Size = 0x80)]
public unsafe struct FDelegateProperty
    // FMulticastDelegateProperty, FMulticastInlineDelegateProperty, FMulticastSparseDelegateProperty
{
    public FProperty _super;
    public UFunction* func;
}

[StructLayout(LayoutKind.Sequential, Size = 0x88)]
public unsafe struct FEnumProperty
{
    public FProperty _super; // @ 0x0
    public FProperty* underlying_type; // @ 0x78
    public UEnum* enum_data; // @ 0x80
}

[StructLayout(LayoutKind.Sequential)]
public struct UUserDefinedEnum
{
    public UEnum Super;
    public TMap<FName, FText> DisplayNameMap;
}
