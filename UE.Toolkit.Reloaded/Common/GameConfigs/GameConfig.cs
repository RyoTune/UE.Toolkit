using UE.Toolkit.Reloaded.Common.GameConfigs.Games;

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
    };
    
    public static IGameConfig Instance { get; private set; } = null!;

    public static void SetGame(string appId)
    {
        if (Mod.Config.GameConfig != GameConfigVersion.Auto)
        {
            Instance = _gameConfigs[Mod.Config.GameConfig]();
            return;
        }
        
        Instance = appId switch
        {
            "p3r.exe" or "smt5v-win64-shipping.exe" => new UE4_27_2_P3R(),
            _ => new UE5_4_4_ClairObscur()
        };
        
        Log.Information($"Game config set to: {Instance.Id} (App ID: {appId})");
    }
}
