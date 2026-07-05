using System.Runtime.InteropServices;

namespace UE.Toolkit.Core.Types.Unreal.UE5_4_4;

public enum CapabilityFlags : uint
{
    HasNoopConstructor = 1 << 0,
    HasZeroConstructor = 1 << 1,
    HasDestructor = 1 << 2,
    HasSerializer = 1 << 3,
    HasStructuredSerializer = 1 << 4,
    HasPostSerialize = 1 << 5,
    HasNetSerializer = 1 << 6,
    HasNetSharedSerialization = 1 << 7,
    HasNetDeltaSerializer = 1 << 8,
    HasPostScriptConstruct = 1 << 9,
    IsPlainOldData = 1 << 10,
    IsUECoreType = 1 << 11,
    IsUECoreVariant = 1 << 12,
    HasCopy = 1 << 13,
    HasIdentical = 1 << 14,
    HasExportTextItem = 1 << 15,
    HasImportTextItem = 1 << 16,
    HasAddStructReferencedObjects = 1 << 17,
    HasSerializeFromMismatchedTag = 1 << 18,
    HasStructuredSerializeFromMismatchedTag = 1 << 19,
    HasGetTypeHash = 1 << 20,
    IsAbstract = 1 << 21,
}

[StructLayout(LayoutKind.Sequential)]
public struct FCapabilities
{
    public EPropertyFlags ComputedPropertyFlags;
    public CapabilityFlags CapabilityFlags;
}