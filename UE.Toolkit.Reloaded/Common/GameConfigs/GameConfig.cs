using UE.Toolkit.Reloaded.Common.GameConfigs.Games;
using UnrealEssentials.Interfaces;

// ReSharper disable InconsistentNaming
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable ConvertToConstant.Global

namespace UE.Toolkit.Reloaded.Common.GameConfigs;

public static class GameConfig
{
    private static readonly Dictionary<GameConfigVersion, Func<IGameConfig>> _gameConfigs = new()
    {
        [GameConfigVersion.UE5_4_4_ClairObscur] = () => new UE5_4_4_ClairObscur(),
        [GameConfigVersion.UE4_27_2_P3R] = () => new UE4_27_2_P3R(),
        [GameConfigVersion.UE_5_0] = () => new UE5_0_3(),
        [GameConfigVersion.UE_5_1] = () => new UE5_2_1(),
        [GameConfigVersion.UE_5_2] = () => new UE5_2_1(),
        [GameConfigVersion.UE_5_3] = () => new UE5_3_2(),
        [GameConfigVersion.UE_5_5] = () => new UE5_4_4_ClairObscur(),
        [GameConfigVersion.UE_5_6] = () => new UE5_6_1(),
        [GameConfigVersion.UE_5_7] = () => new UE5_7_4(),
    };
    
    public static IGameConfig Instance { get; private set; } = null!;

    public static void SetGame(string appId, IUnrealEssentials essentials)
    {
        if (Mod.Config.GameConfig != GameConfigVersion.Auto)
        {
            Instance = _gameConfigs[Mod.Config.GameConfig]();
            return;
        }

        Instance = essentials.GetEngineVersion() switch
        {
            "++UE4+Release-4.27" => new UE4_27_2_P3R(),
            "++UE5+Release-5.0" => new UE5_0_3(),
            "++UE5+Release-5.1" or "++UE5+Release-5.2" => new UE5_2_1(),
            "++UE5+Release-5.3" => new UE5_3_2(),
            "++UE5+Release-5.6" => new UE5_6_1(),
            "++UE5+Release-5.7" => new UE5_7_4(),
            _ => new UE5_4_4_ClairObscur()
        };
        
        Log.Information($"Game config set to: {Instance.Id} (App ID: {appId})");
    }
}
