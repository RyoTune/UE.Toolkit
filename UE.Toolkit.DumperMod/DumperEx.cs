using System.Diagnostics;
using System.Text;
using UE.Toolkit.Core.Types.Unreal.Factories;
using UE.Toolkit.DumperMod.Definitions;
using UE.Toolkit.Interfaces;
using UnrealEssentials.Interfaces;

namespace UE.Toolkit.DumperMod;

public class DumperEx(
    IUnrealFactory factory,
    IUnrealObjects uobjs,
    IUnrealStrings strs,
    IUnrealClasses classes,
    string dumpDir,
    IUnrealEssentials essentials)
{
    private Context Context = new(factory, uobjs, strs, classes, dumpDir, essentials);

    public void DumpObjects()
    {
        Log.Information("Dumping objects...");

        var sw = new Stopwatch();
        sw.Start();
        Context.Registry.Register();
        
        StringBuilder? sb = null;
        if (Mod.Config.Mode == DumpFileMode.SingleFile)
        {
            sb = new();
            Context.Builtins.AddHeader(sb);
            Context.Builtins.AddBaseObjectDefinition(sb);
        }
        else
        {
            var outputFile = Path.Join(Context.DumpDirectory, "UObjectImpl.cs");
            var implWriter = new StringBuilder();
            Context.Builtins.AddHeader(implWriter);
            Context.Builtins.AddBaseObjectDefinition(implWriter);
            File.WriteAllText(outputFile, implWriter.ToString());
        }
        
        var numDumped = 0;
        
        foreach (var (_, Definition) in Context.Registry.Structs)
        {
            if (Mod.Config.Mode == DumpFileMode.FilePerType)
            {
                var outputFile = Path.Join(Context.DumpDirectory, $"{Definition.DisplayName}.cs");
                File.WriteAllText(outputFile, Definition.Serialize(Context));
            }
            else sb?.AppendLine(Definition.Serialize(Context));
            numDumped++;
        }
        
        foreach (var (Name, Definition) in Context.Registry.Enums)
        {
            if (Mod.Config.Mode == DumpFileMode.FilePerType)
            {
                var outputFile = Path.Join(Context.DumpDirectory, $"{Name}");
                File.WriteAllText(outputFile, Definition.Serialize(Context));
            }
            else sb?.AppendLine(Definition.Serialize(Context));
            numDumped++;
        }
        
        sw.Stop();
        if (Mod.Config.Mode == DumpFileMode.SingleFile)
        {
            var singleFileOutput = string.IsNullOrEmpty(Mod.Config.SingleFileOutputName)
                ? Path.Join(Context.DumpDirectory, "Types.cs")
                : Path.Join(Context.DumpDirectory, $"{Mod.Config.SingleFileOutputName.Replace(".cs", string.Empty)}.cs");
            
            File.WriteAllText(singleFileOutput, sb!.ToString());
            Log.Information($"{numDumped} objects dumped in {sw.ElapsedMilliseconds}ms.\nOutput File: {singleFileOutput}");
        }
        else
        {
            Log.Information($"{numDumped} objects dumped in {sw.ElapsedMilliseconds}ms.\nOutput Folder: {Context.DumpDirectory}");
        }
    }
}