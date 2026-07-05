using System.Runtime.InteropServices;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Core.Types.Unreal.UE5_7_4;

[StructLayout(LayoutKind.Explicit, Size = 0xb0, Pack = 8)]
public unsafe struct UStruct
{
    [FieldOffset(0x0)] public UField _super;
    [FieldOffset(0x40)] public UStruct* SuperStruct; // ObjectPtr_Private::TNonAccessTrackedObjectPtr<UStruct> (sizeof = 0x8)
    [FieldOffset(0x48)] public UField* Children; // anything not a type field (e.g a class method) - beginning of linked list
    [FieldOffset(0x50)] public FField* ChildProperties; // the data model - beginning of linked list
    [FieldOffset(0x58)] public int PropertiesSize;
    [FieldOffset(0x5c)] public short MinAlignment;
    [FieldOffset(0x5e)] public short StructStateFlags; // std::atomic<ushort>
    [FieldOffset(0x60)] public TArray<byte> Script;
    [FieldOffset(0x70)] public FProperty* PropertyLink;
    [FieldOffset(0x78)] public FProperty* RefLink;
    [FieldOffset(0x80)] public FProperty* DestructorLink;
    [FieldOffset(0x88)] public FProperty* PostConstructLink;
}