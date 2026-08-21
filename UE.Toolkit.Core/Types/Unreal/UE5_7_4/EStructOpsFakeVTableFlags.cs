namespace UE.Toolkit.Core.Types.Unreal.UE5_7_4;

public enum EStructOpsFakeVTableFlags
{
    None = 0,
    Construct = 1,
    Destruct = 4,
    SerializeArchive = 8,
    SerializeSlot = 16,
    PostSerialize = 32,
    NetSerialize = 64,
    NetDeltaSerialize = 128,
    PostScriptConstruct = 256,
    GetPreloadDependencies = 512,
    Copy = 1024,
    Identical = 2048,
    ExportTextItem = 4096,
    ImportTextItem = 8192,
    FindInnerPropertyInstance = 16384,
    AddStructReferencedObjects = 32768,
    SerializeFromMismatchedTag = 65536,
    StructuredSerializeFromMismatchedTag = 131072,
    GetStructTypeHash = 262144,
    InitializeIntrusiveUnsetOptionalValue = 524288,
    IsIntrusiveOptionalValueSet = 1048576,
    ClearIntrusiveOptionalValue = 2097152,
    Visit = 8388608,
    ResolveVisitedPathInfo = 16777216,
}