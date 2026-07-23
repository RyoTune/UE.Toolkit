using System.Text;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.DumperMod.Definitions;

[Flags]
public enum ObjectType
{
    None = 0,
    Class = 1 << 0, // inherits from UClass
    Struct = 1 << 1, // inherits from UScriptStruct
    Enum = 1 << 2, // inherits from UEnum
    Interface = 1 << 3, // inherits from UInterface
    Function = 1 << 5, // inherits from UFunction
}

public class Registry(Context context)
{
    private Context Context = context;

    public Dictionary<string, StructDefinition> Structs = [];
    public Dictionary<string, EnumDefinition> Enums = [];

    private IObjectFactory? GetObjectFactory(IUObject obj)
    {
        if (obj.IsChildOf<UClass>())
        {
            var objectType = ObjectType.Class;
            if (obj.IsChildOf<UInterface>()) objectType |= ObjectType.Interface;
            return Mod.Config.Schema switch
            {
                DumpSchema.Static => new ClassFactoryStatic(Context, objectType, Context.Factory.Cast<IUClass>(obj)),
                DumpSchema.Dynamic => new ClassFactory(Context, objectType, Context.Factory.Cast<IUClass>(obj)),
            };
        }
        if (obj.IsChildOf<UScriptStruct>())
            return new StructFactory(Context, ObjectType.Struct, Context.Factory.Cast<IUScriptStruct>(obj));
        if (obj.IsChildOf<UEnum>())
            return new EnumFactory(Context, ObjectType.Enum, Context.Factory.Cast<IUEnum>(obj));
        return null;
    }

    public void Register()
    {
        var objectArray = Context.Objects.GUObjectArray;
        for (var i = 0; i < objectArray.NumElements; i++)
        {
            var obj = objectArray.IndexToObject(i);
            if (obj == null) continue;
            // Interfaces? (Interface)
            GetObjectFactory(obj)?.Register();
        }
    }

    private void SerializeDefinition(string Name, ISerializable ser, StringBuilder? sb, ref int numDumped)
    {
        if (Mod.Config.Mode == DumpFileMode.FilePerType)
        {
            var outputFile = Path.Join(Context.DumpDirectory, $"{Name}.cs");
            File.WriteAllText(outputFile, ser.Serialize(Context));
        }
        else sb?.AppendLine(ser.Serialize(Context));
        numDumped++;       
    }

    public void Serialize(StringBuilder? sb, ref int numDumped)
    {
        foreach (var (_, Definition) in Context.Registry.Structs)
            SerializeDefinition(Definition.DisplayName, Definition, sb, ref numDumped);
        foreach (var (Name, Definition) in Context.Registry.Enums)
            SerializeDefinition(Name, Definition, sb, ref numDumped);
    }
    
    private static string GetModuleNameForPackage(IUObject package)
    {
        if (package.OuterPrivate != null)
        {
            throw new("Encountered a package with an outer object set");
        }

        var packageName = package.NamePrivate.ToString();
        if (!packageName.StartsWith("/Script/"))
        {
            return string.Empty;
        }

        return packageName["/Script/".Length..];
    }
}