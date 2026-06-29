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
            "iostoretest_50-win64-shipping.exe" or "iostoretest_51-win64-shipping.exe" 
                or "iostoretest_52-win64-shipping.exe" or "chronos-win64-shipping.exe" => new UE5_2_1(),
            "iostoretest_53-win64-shipping.exe" => new UE5_3_2(),
            "iostoretest_56-win64-shipping.exe" => new UE5_6_1(),
            "iostoretest_57-win64-shipping.exe" => new UE5_7_4(),
            _ => new UE5_4_4_ClairObscur()
        };
        
        Log.Information($"Game config set to: {Instance.Id} (App ID: {appId})");
    }

    // 
    public static Type GetFText()
    {
        return Instance.Id switch
        {
            "P3R" or "UE5_2_1" or "UE5_3_2" => typeof(UE.Toolkit.Core.Types.Unreal.UE4_27_2.FText),
            _ => typeof(UE.Toolkit.Core.Types.Unreal.UE5_4_4.FText)
        };
    }

    public static int GetFTextSize()
    {
        unsafe
        {
            return Instance.Id switch
            {
                "P3R" or "UE5_2_1" or "UE5_3_2" => sizeof(UE.Toolkit.Core.Types.Unreal.UE4_27_2.FText),
                _ => sizeof(UE.Toolkit.Core.Types.Unreal.UE5_4_4.FText)
            };   
        }
    }
}
