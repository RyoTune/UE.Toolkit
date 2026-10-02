using Reloaded.Mod.Interfaces;
using UE.Toolkit.Core.Types.Unreal.Factories;
using UE.Toolkit.DumperMod.Definitions;
using UE.Toolkit.Interfaces;
using UnrealEssentials.Interfaces;

namespace UE.Toolkit.DumperMod;

public class Context
{
    public Context(IUnrealFactory factory, IUnrealObjects uobjs, IUnrealStrings strs, IUnrealClasses classes, 
        string dumpDir, IUnrealEssentials essentials, IModConfig modConfig)
    {
        DumpDirectory = dumpDir;
        Objects = uobjs;
        Strings = strs;
        Factory = factory;
        Classes = classes;
        Essentials = essentials;
        ModConfig = modConfig;
        Builtins = new(essentials, modConfig);
        Registry = new(this);
    }

    public string DumpDirectory { get; }
    
    public IUnrealObjects Objects { get; }
    public IUnrealStrings Strings { get; }
    public IUnrealFactory Factory { get; }
    public IUnrealClasses Classes { get; }
    public IUnrealEssentials Essentials { get; }
    public IModConfig ModConfig { get; }
    public Builtins Builtins { get; }
    public Registry Registry { get; }
}