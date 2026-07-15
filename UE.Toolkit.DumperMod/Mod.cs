#if DEBUG
using System.Diagnostics;
#endif
using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using UE.Toolkit.Core.Types.Unreal.Factories;
using UE.Toolkit.DumperMod.Template;
using UE.Toolkit.Interfaces;

namespace UE.Toolkit.DumperMod;

public class Mod : ModBase
{
    private readonly IModLoader _modLoader;
    private readonly IReloadedHooks? _hooks;
    private readonly ILogger _log;
    private readonly IMod _owner;

    public static Config Config = null!;
    private readonly IModConfig _modConfig;
    private readonly Dumper _dumper;

    public Mod(ModContext context)
    {
        _modLoader = context.ModLoader;
        _hooks = context.Hooks;
        _log = context.Logger;
        _owner = context.Owner;
        Config = context.Configuration;
        _modConfig = context.ModConfig;
#if DEBUG
        Debugger.Launch();
#endif
        Project.Initialize(_modConfig, _modLoader, _log, true);
        Log.LogLevel = Config.LogLevel;

        _modLoader.GetController<IUnrealObjects>().TryGetTarget(out var objs);
        _modLoader.GetController<IUnrealStrings>().TryGetTarget(out var strs);
        _modLoader.GetController<IUnrealFactory>().TryGetTarget(out var factory);
        _modLoader.GetController<IUnrealClasses>().TryGetTarget(out var classes);

        var dumpDir = Path.Join(_modLoader.GetDirectoryForModId(_modConfig.ModId), "dump",
            _modLoader.GetAppConfig().AppId);
        _dumper = new(factory!, objs!, strs!, classes!, dumpDir);
    }

    #region Standard Overrides

    public override void ConfigurationUpdated(Config configuration)
    {
        // Apply settings from configuration.
        // ... your code here.
        Config = configuration;
        _log.WriteLine($"[{_modConfig.ModId}] Config Updated: Applying");
        Log.LogLevel = Config.LogLevel;
        _dumper.DumpObjects();
    }

    #endregion

    #region For Exports, Serialization etc.

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Mod()
    {
    }
#pragma warning restore CS8618

    #endregion
}