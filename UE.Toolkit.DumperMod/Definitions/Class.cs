using System.Text;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;

namespace UE.Toolkit.DumperMod.Definitions;

public class ClassFactory(Context context, IUClass uclass) : BaseObjectFactory(context)
{
    public override void Register() {
        var className = uclass.NamePrivate.ToString();
        var classNativeName = uclass.GetNativeName();
        var size = uclass.PropertiesSize;
        var alignment = uclass.MinAlignment;
        
        // TODO: Flag stuff?

        var super = uclass.GetSuperClass();
        var superSize = super?.PropertiesSize ?? 0;
        var superName = super?.NamePrivate.ToString();
        //var superNativeName = super != null ? GetNativeClassName(super) : "UObjectBaseUtility";
        
        // TODO: Add super header data.
        
        // TODO: Generate delegates.

        var propClass = new PropertyClassFactory(Context).ResolveProperties(uclass.PropertyLink, superSize);
        var propStruct = new PropertyStructReprFactory(Context).ResolveProperties(uclass.PropertyLink, superSize);

        // TODO: Generate functions.

        Context.Registry.Structs[className] =
            new ClassDefinition(className, classNativeName, size, alignment, propClass, propStruct, superName);
        
        Log.Debug($"UObject: {classNativeName}");
    }
}

public class ClassDefinition(
    string internalName,
    string displayName,
    int size,
    int alignment,
    List<BasePropertyDefintion> propClass,
    List<BasePropertyDefintion> propStruct,
    string? superInternalName)
    : StructDefinition(internalName, displayName, size, alignment, propClass, superInternalName)
{
    private List<BasePropertyDefintion> PropStruct => propStruct;
    
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
        var ReprNewModifier = DisplayNameCS != "UObject" ? "new " : string.Empty;
        sb.AppendLine($"\tpublic {ReprNewModifier}unsafe {DisplayNameRepr}* Repr => ({DisplayNameRepr}*)Inner.Ptr;\n");
        foreach (var prop in Properties)
            sb.AppendLine(prop.Serialize(context));
        sb.AppendLine("}\n");
        var reprDef = new StructDefinitionRepr(InternalName, DisplayName, Size, Alignment, PropStruct, SuperInternalName);
        sb.AppendLine(reprDef.Serialize(context));
        return sb.ToString();
    }
    
    public override ObjectType Type => ObjectType.Class;
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
        sb.AppendLine($"\n\tpublic {DisplayNameCS} ToManaged(IUnrealFactory factory)\n\t{{");
        sb.AppendLine($"\t\tfixed ({DisplayNameRepr}* self = &this) return new(factory.CreateUObject((nint)self));");
        sb.AppendLine("\t}\n}");
        return sb.ToString();
    }
}