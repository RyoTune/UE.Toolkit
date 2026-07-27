using UE.Toolkit.Core.Types.Unreal.Factories;
using UE.Toolkit.DumperMod.Definitions;
using UE.Toolkit.Interfaces;
using UnrealEssentials.Interfaces;

namespace UE.Toolkit.DumperMod;

public class Context
{
    public Context(IUnrealFactory factory, IUnrealObjects uobjs, IUnrealStrings strs, IUnrealClasses classes, 
        string dumpDir, IUnrealEssentials essentials)
    {
        DumpDirectory = dumpDir;
        Objects = uobjs;
        Strings = strs;
        Factory = factory;
        Classes = classes;
        Essentials = essentials;
        Builtins = new(essentials);
        Registry = new(this);
    }

    public string DumpDirectory { get; }
    
    public IUnrealObjects Objects { get; }
    public IUnrealStrings Strings { get; }
    public IUnrealFactory Factory { get; }
    public IUnrealClasses Classes { get; }
    public IUnrealEssentials Essentials { get; }
    public Builtins Builtins { get; }
    public Registry Registry { get; }
}