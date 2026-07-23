using System.Text;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.DumperMod.Definitions;

public class EnumFactory(Context context, ObjectType objectType, IUEnum uenum, string? knownType = null) 
    : BaseObjectFactory(context, objectType)
{
    public override void Register()
    {
        var name = uenum.NamePrivate.ToString();
        if (Context.Registry.Enums.ContainsKey(name) && knownType == null) return;
        
        var entries = new Dictionary<string, long>();
        
        var dispNames = new Dictionary<int, string>();
        if (uenum.IsChildOf<UUserDefinedEnum>())
        {
            var userEnum = Context.Factory.Cast<IUUserDefinedEnum>(uenum);
            for (var i = 0; i < uenum.Names.ArrayNum; i++)
            {
                var dispName = Context.Strings.UEnumGetDisplayNameTextByIndex(userEnum.Ptr, i);
                dispNames[i] = $"{name}::{dispName}";
            }
        }

        long bigEntryValue = 0;
        var bigEntryName = string.Empty;
        for (var i = 0; i < uenum.Names.ArrayNum; i++)
        {
            unsafe
            {
                var entry = &uenum.Names.AllocatorInstance[i];

                if (!dispNames.TryGetValue(i, out var entryName))
                {
                    entryName = entry->Key.ToString();
                }
            
                var entryValue = entry->Value;
                entries[entryName] = entryValue;
            
                if (bigEntryValue < entryValue)
                {
                    bigEntryValue = entryValue;
                    bigEntryName = entryName;
                }   
            }
        }

        var entryConstant = $"{name}::{name}_MAX";
        if (entries.TryGetValue(entryConstant, out var entryConstantValue) && entryConstantValue == bigEntryValue)
        {
            entries.Remove(entryConstant);
        }

        if (bigEntryValue == 256 && bigEntryName.EndsWith("_MAX")) entries.Remove(bigEntryName);

        var underlyingType = knownType ?? bigEntryValue switch
        {
            <= byte.MaxValue => "byte",
            <= short.MaxValue => "short",
            <= int.MaxValue => "int",
            <= long.MaxValue => "long",
        };

        var hasNegativeValue = entries.Any(x => x.Value < 0);
        if (underlyingType == "byte" && hasNegativeValue) underlyingType = "sbyte";
        if (underlyingType != "byte" && !hasNegativeValue && !underlyingType.StartsWith('u')) underlyingType = $"u{underlyingType}";
        
        Context.Registry.Enums[name] = new EnumDefinition(name, underlyingType, entries);
    }
}

public class EnumDefinition(string name, string underlyingType, Dictionary<string, long> entries) : ISerializable
{
    private string Name = name;
    private string UnderlyingType = underlyingType;
    private Dictionary<string, long> Entries = entries;
    
    public string Serialize(Context context)
    {
        var sb = new StringBuilder();
            
        if (Mod.Config.Mode == DumpFileMode.FilePerType)
        {
            context.Builtins.AddHeader(sb);
        }
            
        sb.AppendLine($"public enum {Builtins.SanitizeName(Name)} : {UnderlyingType}\n{{");
        foreach (var entry in Entries)
        {
            sb.AppendLine($"    {SanitizeEntryName(entry.Key)} = {entry.Value},");
        }

        sb.AppendLine("}");
        return sb.ToString();
    }
    
    public ObjectType Type => ObjectType.Enum;
    
    private static string SanitizeEntryName(string name) => Builtins.SanitizeName(name.Split("::").Last());
}