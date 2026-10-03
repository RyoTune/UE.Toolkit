using UE.Toolkit.Core.Types.Unreal.Factories;
using UE.Toolkit.Core.Types.Unreal.Factories.UE5_5_4;

namespace UE.Toolkit.Reloaded.Common.GameConfigs.Games;

// ReSharper disable once InconsistentNaming
public class UE5_5_4 : UE5_4_4_ClairObscur
{
    public override string Id => "UE5_5_4";
    public override IUnrealFactory Factory { get; } = new UnrealFactory();
}