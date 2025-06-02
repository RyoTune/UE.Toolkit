using UE.Toolkit.Core.Common;
using UE.Toolkit.Core.Types.Unreal.Factories;
using UE.Toolkit.Core.Types.Unreal.Factories.UE5_4_4;

namespace UE.Toolkit.Reloaded.Common.GameConfigs.Games;

// ReSharper disable once InconsistentNaming
public class UE5_4_4_ClairObscur : IGameConfig
{
    public virtual string Id => "Clair Obscur";
    public virtual string UObject_PostLoadSubobjects => "40 55 53 41 56 48 8D AC 24 ?? ?? ?? ?? 48 81 EC 10 02 00 00 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 85 ?? ?? ?? ?? 48 8B 41";
    public virtual string GUObjectArray => "2B 05 ?? ?? ?? ?? 44 89 05";
    public virtual Func<nint, nint> GUObjectArray_Result { get; } = result => ToolkitUtils.GetGlobalAddress(result + 2);
    public virtual string UStruct_IsChildOf => "48 85 D2 74 ?? 48 63 42 ?? 4C 8D 42";

    public virtual string GFNamePool => "48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 4C 8B C0 C6 05 ?? ?? ?? ?? 01 8B D3 0F B7 C3 89 44 24";
    public virtual string FNameHelper_FindOrStoreString => "48 89 74 24 ?? 57 48 83 EC 60 81 79 ?? 00 04 00 00";

    //public virtual string FMemory_Malloc => "48 89 5C 24 ?? 57 48 83 EC 20 48 8B F9 8B DA 48 8B 0D ?? ?? ?? ?? 48 85 C9 75 ?? E8";
    //public virtual string FMemory_Free => "48 85 C9 74 ?? 53 48 83 EC 20 48 8B D9 48 8B 0D";
    public virtual string GMalloc => "48 8B 0D ?? ?? ?? ?? 48 85 C9 75 ?? E8 ?? ?? ?? ?? 48 8B 0D ?? ?? ?? ?? 48 8B 01 48 8B D3 FF 50 ?? 48 83 C4 20";
    public virtual string UDataTable_HandleDataTableChanged => "40 55 53 57 48 8D 6C 24 ?? 48 81 EC C0 00 00 00 8B 41";
    public virtual string UEnum_GetDisplayNameTextByIndex => "48 89 5C 24 ?? 55 56 57 48 83 EC 30 48 8B FA";
    public virtual IUnrealFactory Factory { get; } = new UnrealFactory();
}