using System.Text;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.DumperMod.Definitions;

public class ClassFactory(Context context, ObjectType objectType, IUClass uclass) : BaseObjectFactory(context, objectType)
{
    public override void Register() {
        var innerName = uclass.NamePrivate.ToString();
        var displayName = uclass.GetNativeName();
        var size = uclass.PropertiesSize;
        var alignment = uclass.MinAlignment;
        
        // TODO: Flag stuff?
        var super = uclass.GetSuperClass();
        var superSize = super?.PropertiesSize ?? 0;
        var superName = super?.NamePrivate.ToString();
        
        // TODO: Generate delegates.

        var propClass = new PropertyClassFactory(Context).ResolveProperties(uclass.PropertyLink, superSize);
        var propStruct = new PropertyStructFactory(Context).ResolveProperties(uclass.PropertyLink, superSize);

        // TODO
        // var functions = new FunctionFactory(Context).ResolveFunctions(uclass);
        
        Context.Registry.Structs[innerName] =
            new ClassDefinition(innerName, displayName, size, alignment, propClass, propStruct, [], superName);
    }
}

public class ClassFactoryStatic(Context context, ObjectType objectType, IUClass uclass)
    : BaseObjectFactory(context, objectType)
{
    public override void Register() {
        var innerName = uclass.NamePrivate.ToString();
        var displayName = uclass.GetNativeName();
        var size = uclass.PropertiesSize;
        var alignment = uclass.MinAlignment;
        
        var super = uclass.GetSuperClass();
        var superSize = super?.PropertiesSize ?? 0;
        var superName = super?.NamePrivate.ToString();
        
        var propStruct = new PropertyStructFactory(Context).ResolveProperties(uclass.PropertyLink, superSize);
        Context.Registry.Structs[innerName] = new StructDefinition(innerName, displayName, size, alignment, propStruct, superName);
    }
}

public class ClassDefinition(
    string internalName,
    string displayName,
    int size,
    int alignment,
    List<BasePropertyDefintion> propClass,
    List<BasePropertyDefintion> propStruct,
    List<FunctionDefinition> functions,
    string? superInternalName)
    : StructDefinition(internalName, displayName, size, alignment, propClass, superInternalName)
{
    private List<BasePropertyDefintion> PropStruct => propStruct;
    private List<FunctionDefinition> Functions => functions;
    
    public override string Serialize(Context context)
    {
        var sb = new StringBuilder();
        if (Mod.Config.Mode == DumpFileMode.FilePerType) context.Builtins.AddHeader(sb);
        var DisplayNameCS = Builtins.SanitizeName(DisplayName);
        var DisplayNameRepr = $"{DisplayNameCS}_Repr";
        StructDefinition? SuperDef = null;
        if (SuperInternalName != null) context.Registry.Structs.TryGetValue(SuperInternalName, out SuperDef);
        var SuperDefName = SuperDef != null ? Builtins.SanitizeName(SuperDef.DisplayName) : "ObjectImpl";
        sb.AppendLine($"public class {DisplayNameCS}(IUObject inner) : {SuperDefName}(inner), ITypeRepr<{DisplayNameRepr}>\n{{");
        var ReprNewModifier = SuperDef != null  ? "new " : string.Empty;
        sb.AppendLine($"\tpublic {ReprNewModifier}unsafe {DisplayNameRepr}* Repr => ({DisplayNameRepr}*)Inner.Ptr;\n");
        foreach (var prop in Properties)
            sb.AppendLine(prop.Serialize(context));
        foreach (var func in Functions)
            sb.AppendLine(func.Serialize(context));
        sb.AppendLine("}\n");
        var reprDef = new StructDefinitionRepr(InternalName, DisplayName, Size, Alignment, PropStruct, SuperInternalName);
        sb.AppendLine(reprDef.Serialize(context));
        return sb.ToString();
    }
    
    public override ObjectType Type => ObjectType.Class;
    
    public override string GetUnmanagedTypeName() => DisplayName + "_Repr";
}

public class StructDefinitionRepr(
    string internalName,
    string displayName,
    int size,
    int alignment,
    List<BasePropertyDefintion> properties,
    string? superInternalName)
    : StructDefinition(internalName, displayName, size, alignment, properties, superInternalName)
{
    public override string Serialize(Context context)
    {
        var sb = new StringBuilder();

        if (Mod.Config.Mode == DumpFileMode.FilePerType) context.Builtins.AddHeader(sb);
        
        var DisplayNameCS = Builtins.SanitizeName(DisplayName);
        var DisplayNameRepr = $"{DisplayNameCS}_Repr";
        var (superName, superSize) = GetSuperInfo(context);
        var superNameFmt = superName != null ? $"{superName}_Repr" : null;
        WriteStructBase(sb, DisplayNameRepr, superNameFmt, superSize, context);
        var sep = Properties.Count > 0 ? "\n" : string.Empty;
        sb.AppendLine($"{sep}\tpublic {DisplayNameCS} ToManaged(IUnrealFactory factory)\n\t{{");
        sb.AppendLine($"\t\tfixed ({DisplayNameRepr}* self = &this) return new(factory.CreateUObject((nint)self));");
        sb.AppendLine("\t}\n}");
        return sb.ToString();
    }
}