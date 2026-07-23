using System.Runtime.CompilerServices;
using System.Text;
using UnrealEssentials.Interfaces;

namespace UE.Toolkit.DumperMod.Definitions;

public class Builtins(IUnrealEssentials essentials)
{
    private IUnrealEssentials Essentials = essentials;

    private List<string> GetDefaultUsings()
    {
        List<string> Usings = [];
        if (Mod.Config.Schema == DumpSchema.Dynamic)
            Usings.Add("System.Runtime.CompilerServices");
        Usings.AddRange([
            "System.Runtime.InteropServices",
            "UE.Toolkit.Core.Types",
            "UE.Toolkit.Core.Types.Unreal.UE5_4_4",
            "UE.Toolkit.Core.Types.Unreal.Factories",
            "UE.Toolkit.Core.Types.Unreal.Factories.Interfaces",
            "UE.Toolkit.Core.Types.Unreal.Common.FunctionParam",
        ]);
        var VerParts = Essentials.GetEngineVersion().Split("-")[^1].Split(".");
        // FText definition is different for versions below UE 5.4
        if (int.Parse(VerParts[0]) < 5 || int.Parse(VerParts[1]) < 4)
            Usings.Add("FText = UE.Toolkit.Core.Types.Unreal.UE4_27_2.FText");
        return Usings;
    }
    
    public void AddHeader(StringBuilder sb)
    {
        sb.AppendLine("""
/* Generated with UE Toolkit: Dumper (1.10.0)    */
/* GitHub: https://github.com/RyoTune/UE.Toolkit */
/* Author: RyoTune and Rirurin                   */
/* Special thanks to UE4SS team whose code was   */
/* used for reference.                           */

""");
        
        foreach (var use in GetDefaultUsings())
            sb.AppendLine($"using {use};");
            
        if (!string.IsNullOrEmpty(Mod.Config.FileUsings))
        {
            var usings = Mod.Config.FileUsings.Split(';',
                    StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Replace("using ", string.Empty));
            foreach (var use in usings) sb.AppendLine($"using {use};");
        }

        sb.AppendLine();
        if (!string.IsNullOrEmpty(Mod.Config.FileNamespace))
        {
            sb.AppendLine($"namespace {Mod.Config.FileNamespace.TrimEnd(';').Replace("namespace ", string.Empty)};");
            sb.AppendLine();
        }
    }

    public void AddBaseObjectDefinition(StringBuilder sb)
    {
sb.AppendLine("""
public interface ITypeRepr<TRepr> where TRepr: unmanaged
{
    unsafe TRepr* Repr { get; }
}

public abstract class ObjectImpl
{
    public IUObject Inner { get; }
    protected static Dictionary<string, int> FieldOffsets;
    
    protected ObjectImpl(IUObject inner) {
        Inner = inner;
        FieldOffsets = Inner.ClassPrivate.PropertyLink
            .Select(x => (x.NamePrivate, x.Offset_Internal)).ToDictionary();
    }
}

""");
    }
    
    public static string SanitizeName(string name)
    {
        var parts = name.Split('_');
        if (parts.Last().Length == 32 && parts.Length > 2)
        {
            name = string.Join(string.Empty, parts[..^2]);
        }

        name = name.Replace(' ', '_');
        name = name.Replace('-', '_');
        name = name.Replace('/', '_');
        name = name.Replace("?", string.Empty);
        name = name.Replace("&", string.Empty);
        name = name.Replace('(', '_');
        name = name.Replace(')', '_');
        name = name.Replace('[', '_');
        name = name.Replace(']', '_');
        //name = name.Replace('>', '_');
        if (name == "object") name = "_object";
        
        if (char.IsDigit(name[0]))
        {
            name = '_' + name;
        }

        return name;
    }
}