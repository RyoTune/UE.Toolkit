using System.Text;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;

namespace UE.Toolkit.DumperMod.Definitions;

public abstract class BasePropertyDefintion(string name, int size, int offset, Func<string> propTypeName) : ISerializable
{
    public string Name { get; set; } = name;
    public int Size { get; set; } = size;
    public int Offset { get; set; } = offset;
    public Func<string> PropTypeName { get; } = propTypeName;
    
    public abstract string Serialize(Context context);
}

public class PropertyStructDefinition(string name, int size, int offset, Func<string> propTypeName) 
    : BasePropertyDefintion(name, size, offset, propTypeName)
{
    public override string Serialize(Context context) => $"[FieldOffset(0x{Offset:X})] public {PropTypeName()} {Builtins.SanitizeName(Name)}; // Size: 0x{Size:X}";
}

public class PropertyClassDefinition(string name, int size, int offset, Func<string> propTypeName,
    Func<string, string> accessor, Func<string, string> mutator) 
    : BasePropertyDefintion(name, size, offset, propTypeName)
{
    private Func<string, string> Accessor => accessor;
    private Func<string, string> Mutator => mutator;
    
    public override string Serialize(Context context)
    {
        var propName = Builtins.SanitizeName(Name);
        var sb = new StringBuilder();
        var typename = PropTypeName();
        sb.AppendLine($"\tpublic unsafe {typename} {propName}");
        sb.AppendLine("\t{");
        sb.AppendLine($"\t\t{Accessor(typename)}");
        sb.AppendLine($"\t\t{Mutator(typename)}");
        sb.AppendLine("\t}");
        return sb.ToString();
    }
}

public abstract class BasePropertyFactory(Context context)
{
    public List<BasePropertyDefintion> ResolveProperties(IEnumerable<IFProperty> propLink, int SuperStructEnd)
    {
        List<BasePropertyDefintion> props = [];
        foreach (var prop in propLink)
        {
            // Stop when we find a property with an offset lower than the size of the super struct.
            // PropertyLink contains a list of fields declared by object in ascending order, then from it's super
            // object and so on.
            if (prop.Offset_Internal < SuperStructEnd) break;
            var newProp = ResolveProperty(prop);
            var numSameName = props.Count(x => x.Name.StartsWith(newProp.Name)); // Compare against sanitized name, since that's what props are using.
            if (numSameName > 0) newProp.Name = $"{newProp.Name}_{numSameName + 1}";
            props.Add(newProp);
        }

        return props;
    }

    protected abstract BasePropertyDefintion ResolveProperty(IFProperty prop);
    
    /// <summary>
    /// Gets the property type name, such as <c>byte</c> for <c>ByteProperty</c>.
    /// This is done lazily since we only have the internal names in <see cref="FProperty"/>
    /// when we want their C++/C# equivalent, which isn't known until object registrations finish.
    /// </summary>
    /// <param name="prop"></param>
    /// <returns>C++/C# property type name.</returns>
    protected Func<string> GetPropTypenameStruct(IFProperty prop)
    {
        var className = prop.ClassPrivate.Name;
        switch (className)
        {
            case "BoolProperty":
                return () => "bool";
            case "ByteProperty":
                var byteProp = context.Factory.Cast<IFByteProperty>(prop);
                if (byteProp.Enum != null)
                {
                    var byteEnumName = byteProp.Enum.NamePrivate.ToString();
                    new EnumFactory(context, context.Factory.Cast<IUEnum>(byteProp.Enum), "byte").Register();
                    return () => Builtins.SanitizeName(byteEnumName);
                }
                
                return () => "byte";
            case "Int8Property":
                return () => "byte";
            case "Int16Property":
                return () => "short";
            case "UInt16Property":
                return () => "ushort";
            case "IntProperty":
                return () => "int";
            case "UInt32Property":
                return () => "uint";
            case "Int64Property":
                return () => "long";
            case "UInt64Property":
                return () => "ulong";
            case "FloatProperty":
                return () => "float";
            case "DoubleProperty":
                return () => "double";
            case "NameProperty":
                return () => "FName";
            case "StrProperty":
                return () => "FString";
            case "TextProperty":
                return () => "FText";
            case "DataTableRowHandle":
                return () => "FDataTableRowHandle";
            case "DelegateProperty":
                return () => "FScriptDelegate";
            case "MulticastInlineDelegateProperty":
            case "MulticastSparseDelegateProperty":
                return () => "FMulticastScriptDelegate";
            case "WeakObjectProperty":
                return () => "FWeakObjectPtr";
            case "ObjectProperty":
                var objPropType = context.Factory.Cast<IFObjectProperty>(prop).PropertyClass.NamePrivate.ToString();
                return () => Builtins.SanitizeName(context.Registry.Structs.TryGetValue(objPropType, out var knownStruct) ? $"{knownStruct.DisplayName}*" : $"{objPropType}*");
            case "SoftObjectProperty":
                var softObjPropType = context.Factory.Cast<IFObjectProperty>(prop).PropertyClass.NamePrivate.ToString();
                return () => Builtins.SanitizeName(context.Registry.Structs.TryGetValue(softObjPropType, out var knownStruct) ? $"TSoftObjectPtr<{knownStruct.DisplayName}>" : $"TSoftObjectPtr<{softObjPropType}>");
            case "SoftClassProperty":
                var softClassPropType = context.Factory.Cast<IFSoftClassProperty>(prop).MetaClass.NamePrivate.ToString();
                return () => Builtins.SanitizeName(context.Registry.Structs.TryGetValue(softClassPropType, out var knownStruct) ? $"TSoftClassPtr<{knownStruct.DisplayName}>" : $"TSoftClassPtr<{softClassPropType}>");
            case "StructProperty":
                var structPropType = context.Factory.Cast<IFStructProperty>(prop).Struct.NamePrivate.ToString();
                return () => Builtins.SanitizeName(context.Registry.Structs.TryGetValue(structPropType, out var knownStruct) ? knownStruct.DisplayName : structPropType);
            case "ClassProperty":
            case "ClassPtrProperty":
                var classPropClass = context.Factory.Cast<IFClassProperty>(prop).MetaClass;
                var classPropType = classPropClass != null ? classPropClass.NamePrivate.ToString() : "UClass*";
                return () => Builtins.SanitizeName(context.Registry.Structs.TryGetValue(classPropType, out var knownStruct) ? $"{knownStruct.DisplayName}*" : classPropType);
            case "EnumProperty":
                var enumProp = context.Factory.Cast<IFEnumProperty>(prop);
                var enumPropName = enumProp.Enum.NamePrivate.ToString();
                new EnumFactory(context, enumProp.Enum, context.Classes.GetPropertyTypeName(enumProp.UnderlyingProp)).Register();
                return () => Builtins.SanitizeName(enumPropName);
            case "MapProperty":
                var mapProp = context.Factory.Cast<IFMapProperty>(prop);
                var mapPropKeyType = context.Classes.GetPropertyTypeName(mapProp.KeyProp);
                var mapPropValueType = context.Classes.GetPropertyTypeName(mapProp.ValueProp);
                return () =>
                {
                    var isKeyPtr = mapPropKeyType.EndsWith('*') || mapPropKeyType.Contains('<'); // Use nint for pointers and generic types.
                    var isValuePtr = mapPropValueType.EndsWith('*') || mapPropValueType.Contains('<');
                    
                    context.Registry.Structs.TryGetValue(mapPropKeyType.TrimEnd('*'), out var knownKeyStruct);
                    context.Registry.Structs.TryGetValue(mapPropValueType.TrimEnd('*'), out var knownValueStruct);

                    var keyName = Builtins.SanitizeName(knownKeyStruct?.DisplayName ?? mapPropKeyType);
                    var valueName = Builtins.SanitizeName(knownValueStruct?.DisplayName ?? mapPropValueType);
                    
                    var keyType = isKeyPtr ? $"Ptr<{keyName}>" : keyName;
                    var valueType = isValuePtr ? $"Ptr<{valueName}>" : valueName;
                   
                    return $"TMap<{keyType}, {valueType}>";
                };
            case "InterfaceProperty":
                var intPropType = context.Factory.Cast<IFInterfaceProperty>(prop).InterfaceClass.NamePrivate.ToString();
                return () => Builtins.SanitizeName(context.Registry.Structs.TryGetValue(intPropType, out var knownStruct) ? $"TScriptInterface<{knownStruct.DisplayName}>" : $"TScriptInterface<{intPropType}>");
            case "ArrayProperty":
                var arrayPropType = context.Classes.GetPropertyTypeName(context.Factory.Cast<IFArrayProperty>(prop).Inner);
                return () =>
                {
                    var isPtrType = arrayPropType.EndsWith('*') || arrayPropType.Contains('<'); // Use nint for pointers and generic types.
                    context.Registry.Structs.TryGetValue(arrayPropType.TrimEnd('*'), out var knownStruct);
                    var arrTypeSanitized = Builtins.SanitizeName(knownStruct?.DisplayName ?? arrayPropType);
                    return isPtrType ?
                        $"TArray<Ptr<{arrTypeSanitized}>>" :
                        $"TArray<{arrTypeSanitized}>";
                };
            case "SetProperty":
                var setPropType = context.Classes.GetPropertyTypeName(context.Factory.Cast<IFSetProperty>(prop).ElementProp);
                return () =>
                {
                    var isPtrType = setPropType.EndsWith('*') || setPropType.Contains('<'); // Use nint for pointers and generic types.;
                    context.Registry.Structs.TryGetValue(setPropType.TrimEnd('*'), out var knownStruct);
                    return isPtrType
                        ? $"TSet<nint> /* TSet<{setPropType}> */"
                        : $"TSet<{Builtins.SanitizeName(knownStruct?.DisplayName ?? setPropType)}>";
                };
            case "OptionalProperty":
                var optionalType = context.Classes.GetPropertyTypeName(context.Factory.Cast<IFOptionalProperty>(prop).ValueProperty);
                return () =>
                {
                    if (context.Registry.Structs.TryGetValue(optionalType, out var knownOptType))
                        optionalType = knownOptType.DisplayName;
                    
                    return $"TOptional<{Builtins.SanitizeName(optionalType)}>";
                };
            case "FieldPathProperty":
                return () => "FFieldPath";
            case "LazyObjectProperty":
                var lazyObjType = context.Factory.Cast<IFObjectProperty>(prop).PropertyClass.NamePrivate.ToString();
                return () =>
                {
                    if (context.Registry.Structs.TryGetValue(lazyObjType, out var knownLazyType))
                        lazyObjType = knownLazyType.DisplayName;
                    
                    return $"TLazyObjectPtr<{Builtins.SanitizeName(lazyObjType)}>";
                };
            case "Utf8StrProperty":
                return () => "FUtf8String";
            case "AnsiStrProperty":
                return () => "FAnsiString";
            default:
                Log.Warning($"Unknown Property: {className}");
                return () => className;
        }
    }
    
    protected (string Name, int Size, int Offset, string ClassName) GetBaseInfo(IFProperty prop)
    {
        var name = Builtins.SanitizeName(prop.NamePrivate);
        var size = prop.ElementSize;
        var offset = prop.Offset_Internal;
        var className = prop.ClassPrivate.Name;
        return (name, size, offset, className);
    }
}

public class PropertyStructFactory(Context context) : BasePropertyFactory(context)
{
    
    protected override BasePropertyDefintion ResolveProperty(IFProperty prop)
    {
        var (name, size, offset, _) = GetBaseInfo(prop);
        if (name is "bool" or "float") name += '_';
        return new PropertyStructDefinition(name, size, offset, GetPropTypenameStruct(prop));
    }
}

public class PropertyStructReprFactory(Context context) : BasePropertyFactory(context)
{
    private Func<string> GetPropTypenameStructRepr(IFProperty prop)
    {
        var className = prop.ClassPrivate.Name;
        switch (className)
        {
            case "ObjectProperty":
                var objPropType = context.Factory.Cast<IFObjectProperty>(prop).PropertyClass.NamePrivate.ToString();
                return () => Builtins.SanitizeName(context.Registry.Structs.TryGetValue(objPropType, out var knownStruct) ? $"{knownStruct.DisplayName}_Repr*" : $"{objPropType}*");
            default:
                return GetPropTypenameStruct(prop);
        }
    }
    
    protected override BasePropertyDefintion ResolveProperty(IFProperty prop)
    {
        var (name, size, offset, _) = GetBaseInfo(prop);
        if (name is "bool" or "float") name += '_';
        return new PropertyStructDefinition(name, size, offset, GetPropTypenameStructRepr(prop));
    }
}

public class PropertyClassFactory(Context context) : BasePropertyFactory(context)
{
    private Func<string, string> GetPropAccessor(IFProperty prop, Func<string> getTypeName)
    {
        var className = prop.ClassPrivate.Name;
        switch (className)
        {
            case "ByteProperty" or "Int8Property" or "Int16Property" or "UInt16Property" or "IntProperty"
                or "UInt32Property" or "Int64Property" or "UInt64Property" or "FloatProperty" or "DoubleProperty"
                or "NameProperty" or "StructProperty":
                return propName => $"get => *({getTypeName()}*)(Inner.Ptr + FieldOffsets[\"{propName}\"]);";
            case "BoolProperty":
                var BoolProp = context.Factory.CreateFBoolProperty(prop.Ptr);
                return (BoolProp.FieldMask == byte.MaxValue) switch
                {
                    true => propName => $"get => *(bool*)(Inner.Ptr + FieldOffsets[\"{propName}\"]);",
                    false => propName => $"get => (*(byte*)(Inner.Ptr + FieldOffsets[\"{propName}\"]) & {BoolProp.FieldMask}) == 0;"
                };
            case "ObjectProperty":
                return propName =>
                    $"get => new(Inner.GetFactory().CreateUObject(*(nint*)(Inner.Ptr + FieldOffsets[\"{propName}\"])));";
            default:
                return _ => $"get => throw new NotSupportedException(\"!! GET TODO {className} !!\");";
        }  
    }

    private Func<string, string> GetPropMutator(IFProperty prop, Func<string> getTypeName)
    {
        var className = prop.ClassPrivate.Name;
        switch (className)
        {
            case "ByteProperty" or "Int8Property" or "Int16Property" or "UInt16Property" or "IntProperty"
                or "UInt32Property" or "Int64Property" or "UInt64Property" or "FloatProperty" or "DoubleProperty"
                or "NameProperty" or "StructProperty":
                return propName => $"set => *({getTypeName()}*)(Inner.Ptr + FieldOffsets[\"{propName}\"]) = value;";
            case "BoolProperty":
                var BoolProp = context.Factory.CreateFBoolProperty(prop.Ptr);
                return (BoolProp.FieldMask == byte.MaxValue) switch
                {
                    true => propName => $"set => *(bool*)(Inner.Ptr + FieldOffsets[\"{propName}\"]) = value;",
                    false => propName => $"set => *(byte*)(Inner.Ptr + FieldOffsets[\"{propName}\"]) ^= (byte)(Convert.ToByte({propName} != value) * {BoolProp.FieldMask});"
                };
            case "ObjectProperty":
                return propName => $"set => *(nint*)(Inner.Ptr + FieldOffsets[\"{propName}\"]) = value.Inner.Ptr;";
            default:
                return _ => $"set => throw new NotSupportedException(\"!! SET TODO {className} !!\");";
        }
    }

    private Func<string> GetPropTypenameClass(IFProperty prop)
    {
        var className = prop.ClassPrivate.Name;
        switch (className)
        {
            case "ObjectProperty":
                var objPropType = context.Factory.Cast<IFObjectProperty>(prop).PropertyClass.NamePrivate.ToString();
                return () => Builtins.SanitizeName(context.Registry.Structs.TryGetValue(objPropType, out var knownStruct) ? $"{knownStruct.DisplayName}" : $"{objPropType}*");
            default:
                return GetPropTypenameStruct(prop);
        }
    }
    
    protected override BasePropertyDefintion ResolveProperty(IFProperty prop)
    {
        var (name, size, offset, _) = GetBaseInfo(prop);
        if (name is "bool" or "float") name += '_';
        var propTypename = GetPropTypenameClass(prop);
        var getAccessor = GetPropAccessor(prop, propTypename);
        var getMutator = GetPropMutator(prop, propTypename);
        return new PropertyClassDefinition(name, size, offset, propTypename, getAccessor, getMutator);
    }
}