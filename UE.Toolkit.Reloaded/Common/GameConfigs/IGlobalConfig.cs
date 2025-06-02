// ReSharper disable InconsistentNaming

using UE.Toolkit.Core.Types.Unreal.Factories;

namespace UE.Toolkit.Reloaded.Common.GameConfigs;

public interface IGameConfig
{
    string Id { get; }
    string UObject_PostLoadSubobjects { get; }
    string GUObjectArray { get; }
    Func<nint, nint> GUObjectArray_Result { get; }
    string UStruct_IsChildOf { get; }
    string GFNamePool { get; }
    string FNameHelper_FindOrStoreString { get; }
    //string FMemory_Malloc { get; }
    //string FMemory_Free { get; }
    string GMalloc { get; }
    string UDataTable_HandleDataTableChanged { get; }
    string UEnum_GetDisplayNameTextByIndex { get; }
    IUnrealFactory Factory { get; }
}