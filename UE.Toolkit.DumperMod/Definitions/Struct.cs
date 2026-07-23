using System.Text;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;

namespace UE.Toolkit.DumperMod.Definitions;

public class StructFactory(Context context, ObjectType objectType, IUScriptStruct scriptStruct) : BaseObjectFactory(context, objectType)
{
    public override void Register()
    {
        var structName = scriptStruct.NamePrivate.ToString();
        var structNativeName = "F" + structName;
        var size = scriptStruct.PropertiesSize;
        var alignment = scriptStruct.MinAlignment;
        var super = scriptStruct.SuperStruct;
        var superSize = super?.PropertiesSize ?? 0;
        var superName = super?.NamePrivate.ToString();
        if ((superName ?? string.Empty) == structName)
        {
            Log.Warning($"{nameof(StructFactory)} || '{structNativeName}' is recursive");
            return;
        }
        var props = new PropertyStructFactory(Context).ResolveProperties(scriptStruct.PropertyLink, superSize);
        Context.Registry.Structs[structName] = new StructDefinition(structName, structNativeName, size, alignment, props, superName);
    }
}

public abstract class BaseStructDefinition(
    string internalName,
    string displayName,
    int size,
    int alignment,
    List<BasePropertyDefintion> properties,
    string? superInternalName) : ISerializable
{
    public string InternalName { get; } = internalName;
    public string DisplayName { get; } = displayName;
    public int Size { get; } = size;
    public int Alignment { get; } = alignment;
    public List<BasePropertyDefintion> Properties { get; } = properties;
    public string? SuperInternalName { get; } = superInternalName;
    
    public abstract string Serialize(Context context);
}

public class StructDefinition(
    string internalName,
    string displayName,
    int size,
    int alignment,
    List<BasePropertyDefintion> propClass,
    string? superInternalName) : BaseStructDefinition(
        internalName, displayName, size, alignment, propClass, superInternalName)
{

    protected void WriteStructBase(StringBuilder sb, string StructName, string? SuperName, int SuperSize, Context context)
    {
        sb.AppendLine($"[StructLayout(LayoutKind.Explicit, Pack = {Alignment}, Size = 0x{Size:X})]");
        sb.AppendLine($"public unsafe struct {StructName}\n{{");
        if (SuperName != null)
        {
            sb.AppendLine($"\t[FieldOffset(0x0)] public {SuperName} Super; // Size: 0x{SuperSize:X}");
        }
        foreach (var prop in Properties)
            sb.AppendLine($"\t{prop.Serialize(context)}");       
    }

    protected (string?, int) GetSuperInfo(Context context)
    {
        string? superName = null;
        var superSize = 0;
        if (SuperInternalName != null)
        {
            if (context.Registry.Structs.TryGetValue(SuperInternalName, out var super))
            {
                superName = Builtins.SanitizeName(super.DisplayName);
                superSize = super.Size;
            }
            else
            {
                Log.Warning($"Failed to get super: {SuperInternalName}");
            }
        }
        return (superName, superSize);
    }
    
    public override string Serialize(Context context)
    {
        var sb = new StringBuilder();

        if (Mod.Config.Mode == DumpFileMode.FilePerType) context.Builtins.AddHeader(sb);

        var structName = Builtins.SanitizeName(DisplayName);

        var (superName, superSize) = GetSuperInfo(context);
        WriteStructBase(sb, structName, superName, superSize, context);
        sb.AppendLine("}");
        return sb.ToString();
    }

    public virtual ObjectType Type => ObjectType.Struct;

    public virtual string GetUnmanagedTypeName() => DisplayName;
}