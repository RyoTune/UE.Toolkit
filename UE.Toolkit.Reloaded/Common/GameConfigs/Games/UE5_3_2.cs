using UE.Toolkit.Core.Types.Unreal.Factories;
using UE.Toolkit.Core.Types.Unreal.Factories.UE5_4_4;
using UE.Toolkit.Interfaces;
using UE.Toolkit.Reloaded.Unreal;

namespace UE.Toolkit.Reloaded.Common.GameConfigs.Games;

// ReSharper disable once InconsistentNaming
public class UE5_3_2 : IGameConfig
{
    public virtual string Id => "UE5_3_2";
    public virtual IUnrealFactory Factory { get; } = new UnrealFactory();
    public virtual IUnrealMemory Memory { get; } = new UnrealMemory();
}