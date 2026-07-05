using System.Diagnostics;
using System.Text;
using UE.Toolkit.Core.Types.Unreal.Factories;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;
using UE.Toolkit.Interfaces;

// ReSharper disable InconsistentNaming

namespace UE.Toolkit.DumperMod;

public unsafe class Dumper
{
    private record PropertyDefinition(string Name, int Size, int Offset, Func<string> PropTypeName) : ICsharpText
    {
        public string GetCsharpText()
        {
            return $"[FieldOffset(0x{Offset:X})] public {PropTypeName()} {SanitizeName(Name)}; // Size: 0x{Size:X}";
        }
    }

    private record UStructDefinition(string InternalName, string DisplayName, int Size, int Alignment, PropertyDefinition[] Properties, string? SuperInternalName) : ICsharpText
    {
        public string Name => SanitizeName(DisplayName);

        public string GetCsharpText()
        {
            var sb = new StringBuilder();

            if (Mod.Config.Mode == DumpFileMode.FilePerType)
            {
                AddHeader(sb);
            }
            
            sb.AppendLine($"[StructLayout(LayoutKind.Explicit, Pack = {Alignment}, Size = 0x{Size:X})]");
            sb.AppendLine($"public unsafe struct {SanitizeName(DisplayName)}\n{{");
            if (SuperInternalName != null)
            {
                if (_UStructDefinitions.TryGetValue(SuperInternalName, out var super))
                {
                    sb.AppendLine($"    [FieldOffset(0x0)] public {SanitizeName(super.DisplayName)} Super; // Size: 0x{super.Size:X}");
                }
                else
                {
                    Log.Warning($"Failed to get super: {SuperInternalName}");
                }
            }
            
            foreach (var prop in Properties)
            {
                sb.AppendLine($"    {prop.GetCsharpText()}");
            }
            
            sb.AppendLine("}");
            return sb.ToString();
        }
    }

    private record UEnumDefinition(string Name, string UnderlyingType, Dictionary<string, long> Entries) : ICsharpText
    {
        public string GetCsharpText()
        {
            var sb = new StringBuilder();
            
            if (Mod.Config.Mode == DumpFileMode.FilePerType)
            {
                AddHeader(sb);
            }
            
            sb.AppendLine($"public enum {SanitizeName(Name)} : {UnderlyingType}\n{{");
            foreach (var entry in Entries)
            {
                sb.AppendLine($"    {SanitizeEntryName(entry.Key)} = {entry.Value},");
            }

            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string SanitizeEntryName(string name) => SanitizeName(name.Split("::").Last());
    }
    
    private static readonly Dictionary<string, UStructDefinition> _UStructDefinitions = [];
    private readonly Dictionary<string, UEnumDefinition> _UEnumDefinitions = [];
    private readonly string _dumpDir;
    private readonly IUnrealObjects _uobjs;
    private readonly IUnrealStrings _strs;
    private readonly IUnrealFactory _factory;

    public Dumper(IUnrealFactory factory, IUnrealObjects uobjs, IUnrealStrings strs, string dumpDir)
    {
        _uobjs = uobjs;
        _strs = strs;
        _dumpDir = dumpDir;
        _factory = factory;
        
        if (Directory.Exists(dumpDir)) Directory.Delete(dumpDir, true);
        Directory.CreateDirectory(dumpDir);
    }

    public void DumpObjects()
    {
        Log.Information("Dumping objects...");

        var sw = new Stopwatch();
        
        sw.Start();
        RegisterObjects();

        StringBuilder? sb = null;
        if (Mod.Config.Mode == DumpFileMode.SingleFile)
        {
            sb = new();
            AddHeader(sb);
            AddPtrDefinition(sb);
        }
        else
        {
            var outputFile = Path.Join(_dumpDir, "Builtin_Ptr.cs");
            var sbPtr = new StringBuilder();
            AddHeader(sbPtr);
            AddPtrDefinition(sbPtr);
            File.WriteAllText(outputFile, sbPtr.ToString());
        }

        var numDumped = 0;
        foreach (var item in _UStructDefinitions)
        {
            if (Mod.Config.Mode == DumpFileMode.FilePerType)
            {
                var outputFile = Path.Join(_dumpDir, $"{item.Value.DisplayName}.cs");
                File.WriteAllText(outputFile, item.Value.GetCsharpText());
            }
            else
            {
                sb?.AppendLine(item.Value.GetCsharpText());
            }
            
            numDumped++;
        }
        
        foreach (var item in _UEnumDefinitions)
        {
            if (Mod.Config.Mode == DumpFileMode.FilePerType)
            {
                var outputFile = Path.Join(_dumpDir, $"{item.Key}");
                File.WriteAllText(outputFile, item.Value.GetCsharpText());
            }
            else
            {
                sb?.AppendLine(item.Value.GetCsharpText());
            }
            
            numDumped++;
        }

        sw.Stop();
        if (Mod.Config.Mode == DumpFileMode.SingleFile)
        {
            var singleFileOutput = string.IsNullOrEmpty(Mod.Config.SingleFileOutputName)
                ? Path.Join(_dumpDir, "Types.cs")
                : Path.Join(_dumpDir, $"{Mod.Config.SingleFileOutputName.Replace(".cs", string.Empty)}.cs");
            
            File.WriteAllText(singleFileOutput, sb!.ToString());
            Log.Information($"{numDumped} objects dumped in {sw.ElapsedMilliseconds}ms.\nOutput File: {singleFileOutput}");
        }
        else
        {
            Log.Information($"{numDumped} objects dumped in {sw.ElapsedMilliseconds}ms.\nOutput Folder: {_dumpDir}");
        }
    }

    private void RegisterObjects()
    {
        var objArray = _uobjs.GUObjectArray;
        for (int i = 0; i < _uobjs.GUObjectArray.NumElements; i++)
        {
            //if (i > _uobjs.GUObjectArray.ObjLastNonGCIndex) break;
            
            var obj = _uobjs.GUObjectArray.IndexToObject(i);
            if (obj == null) continue;

            // var uclass = (Core.Types.Unreal.UE4_27_2.UClass*)obj.Ptr;
            // continue;
            var moduleName = GetModuleNameForPackage(obj.GetOutermost());
            var fileBaseName = GetHeaderNameForObject(obj);
            
            if (obj.IsChildOf<UClass>())
            {
                var uclass = _factory.Cast<IUClass>(obj);
                if (obj.IsChildOf<UInterface>())
                {
                    // TODO:
                    var interfaceName = obj.NamePrivate.ToString();
                    _UStructDefinitions[interfaceName] = new(interfaceName, interfaceName, 0, 0, [], null);
                    Log.Debug($"Interface: {interfaceName}");
                }
                else
                {
                    GenerateObjectDefinition(uclass);
                }
            }
            else if (obj.IsChildOf<UScriptStruct>())
            {
                GenerateStructDefinition(_factory.Cast<IUScriptStruct>(obj));
            }
            else if (obj.IsChildOf<UEnum>())
            {
                GenerateEnumDefinition(_factory.Cast<IUEnum>(obj));
            }
        }
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
    
    private static string GetHeaderNameForObject(IUObject obj)
    {
        string? headerName = null;
        UObjectBase* finalObj;
        
        if (obj.IsA<UClass>() || obj.IsA<UScriptStruct>())
        {
            headerName = obj.NamePrivate.ToString();
        }
        else if (obj.IsA<UEnum>())
        {
            headerName = obj.NamePrivate.ToString();
        }
        else
        {
            // TODO: UFunction stuff;
        }
        
        // TODO: Other stuff?
        return headerName;
    }

    private static void AddHeader(StringBuilder sb)
    {
        sb.AppendLine("/* Generated with UE Toolkit: Dumper (1.8.0)     */");
        sb.AppendLine("/* GitHub: https://github.com/RyoTune/UE.Toolkit */");
        sb.AppendLine("/* Author: RyoTune                               */");
        sb.AppendLine("/* Special thanks to UE4SS team and Rirurin      */");
        sb.AppendLine("/* whose code was used for reference.            */");
        sb.AppendLine();
        
        sb.AppendLine("using System.Runtime.InteropServices;");
            
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

    private static void AddPtrDefinition(StringBuilder sb)
    {
        sb.AppendLine("public readonly unsafe struct Ptr<T>(T* value) : IEquatable<Ptr<T>>");
        sb.AppendLine("    where T : unmanaged");
        sb.AppendLine("{");
        sb.AppendLine("    public readonly T* Value = value;");
        sb.AppendLine();
        sb.AppendLine("    public bool Equals(Ptr<T> other) => Value == other.Value;");
        sb.AppendLine("    public static bool operator ==(Ptr<T> a, Ptr<T> b) => a.Equals(b);");
        sb.AppendLine("    public static bool operator !=(Ptr<T> a, Ptr<T> b) => !a.Equals(b);");
        sb.AppendLine("}");
        sb.AppendLine();
    }

    private void GenerateObjectDefinition(IUClass uclass)
    {
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

        var properties = ResolveProperties(uclass.PropertyLink, superSize);

        // TODO: Generate functions.

        _UStructDefinitions[className] =
            new(className, classNativeName, size, alignment, properties, superName);
        
        Log.Debug($"UObject: {classNativeName}");
    }

    private void GenerateStructDefinition(IUScriptStruct scriptStruct)
    {
        var structName = scriptStruct.NamePrivate.ToString();
        var structNativeName = scriptStruct.GetNativeName();
        var size = scriptStruct.PropertiesSize;
        var alignment = scriptStruct.MinAlignment;
        var super = scriptStruct.SuperStruct;
        var superSize = super?.PropertiesSize ?? 0;
        var superName = super?.NamePrivate.ToString();
        //var superNativeName = super != null ? GetNativeStructName((UScriptStruct*)super) : null;
        
        var props = ResolveProperties(scriptStruct.PropertyLink, superSize);

        _UStructDefinitions[structName] = new(structName, structNativeName, size, alignment, props, superName);
        Log.Debug($"UScriptStruct: {structNativeName}");
    }
    
    private void GenerateEnumDefinition(IUEnum uenum, string? knownType = null)
    {
        var name = uenum.NamePrivate.ToString();
        if (_UEnumDefinitions.ContainsKey(name) && knownType == null) return;
        
        var entries = new Dictionary<string, long>();
        
        var dispNames = new Dictionary<int, string>();
        if (uenum.IsChildOf<UUserDefinedEnum>())
        {
            var userEnum = _factory.Cast<IUUserDefinedEnum>(uenum);
            for (var i = 0; i < uenum.Names.ArrayNum; i++)
            {
                var dispName = _strs.UEnumGetDisplayNameTextByIndex(userEnum.Ptr, i);
                dispNames[i] = $"{name}::{dispName}";
            }
        }

        long bigEntryValue = 0;
        string bigEntryName = string.Empty;
        for (int i = 0; i < uenum.Names.ArrayNum; i++)
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

        bool hasNegativeValue = entries.Any(x => x.Value < 0);
        if (underlyingType == "byte" && hasNegativeValue) underlyingType = "sbyte";
        if (underlyingType != "byte" && !hasNegativeValue && !underlyingType.StartsWith('u')) underlyingType = $"u{underlyingType}";
        
        _UEnumDefinitions[name] = new(name, underlyingType, entries);
    }

    private PropertyDefinition[] ResolveProperties(IEnumerable<IFProperty> propLink, int SuperStructEnd)
    {
        var props = new List<PropertyDefinition>();
        foreach (var prop in propLink)
        {
            // Stop when we find a property with an offset lower than the size of the super struct.
            // PropertyLink contains a list of fields declared by object in ascending order, then from it's super
            // object and so on.
            if (prop.Offset_Internal < SuperStructEnd)
                break;
            var newProp = ResolveProperty(prop);
            var numSameName = props.Count(x => x.Name.StartsWith(newProp.Name)); // Compare against sanitized name, since that's what props are using.
            
            // Handle multiple properties with the same name.
            if (numSameName == 0)
            {
                props.Add(newProp);
            }
            else
            {
                props.Add(newProp with { Name = $"{newProp.Name}_{numSameName + 1}" });
            }
        }

        return props.ToArray();
    }

    private PropertyDefinition ResolveProperty(IFProperty prop)
    {
        var (name, size, offset, _) = GetBaseInfo(prop);
        if (name == "bool" || name == "float") name += '_';
        
        var resProp = new PropertyDefinition(SanitizeName(name), size, offset, GetPropertyTypeNameLazy(prop));
        return resProp;
    }

    /// <summary>
    /// Gets the property type name, such as <c>byte</c> for <c>ByteProperty</c>.
    /// This is done lazily since we only have the internal names in <see cref="FProperty"/>
    /// when we want their C++/C# equivalent, which isn't known until object registrations finish.
    /// </summary>
    /// <param name="prop"></param>
    /// <returns>C++/C# property type name.</returns>
    private Func<string> GetPropertyTypeNameLazy(IFProperty prop)
    {
        var className = prop.ClassPrivate.Name;
        switch (className)
        {
            case "BoolProperty":
                return () => "bool";
            case "ByteProperty":
                var byteProp = _factory.Cast<IFByteProperty>(prop);
                if (byteProp.Enum != null)
                {
                    var byteEnumName = byteProp.Enum.NamePrivate.ToString();
                    GenerateEnumDefinition(_factory.Cast<IUEnum>(byteProp.Enum), "byte");
                    return () => SanitizeName(byteEnumName);
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
                var objPropType = _factory.Cast<IFObjectProperty>(prop).PropertyClass.NamePrivate.ToString();
                return () => SanitizeName(_UStructDefinitions.TryGetValue(objPropType, out var knownStruct) ? $"{knownStruct.DisplayName}*" : $"{objPropType}*");
            case "SoftObjectProperty":
                var softObjPropType = _factory.Cast<IFObjectProperty>(prop).PropertyClass.NamePrivate.ToString();
                return () => SanitizeName(_UStructDefinitions.TryGetValue(softObjPropType, out var knownStruct) ? $"TSoftObjectPtr<{knownStruct.DisplayName}>" : $"TSoftObjectPtr<{softObjPropType}>");
            case "SoftClassProperty":
                var softClassPropType = _factory.Cast<IFSoftClassProperty>(prop).MetaClass.NamePrivate.ToString();
                return () => SanitizeName(_UStructDefinitions.TryGetValue(softClassPropType, out var knownStruct) ? $"TSoftClassPtr<{knownStruct.DisplayName}>" : $"TSoftClassPtr<{softClassPropType}>");
            case "StructProperty":
                var structPropType = _factory.Cast<IFStructProperty>(prop).Struct.NamePrivate.ToString();
                return () => SanitizeName(_UStructDefinitions.TryGetValue(structPropType, out var knownStruct) ? knownStruct.DisplayName : structPropType);
            case "ClassProperty":
            case "ClassPtrProperty":
                var classPropClass = _factory.Cast<IFClassProperty>(prop).MetaClass;
                var classPropType = classPropClass != null ? classPropClass.NamePrivate.ToString() : "UClass*";
                return () => SanitizeName(_UStructDefinitions.TryGetValue(classPropType, out var knownStruct) ? $"{knownStruct.DisplayName}*" : classPropType);
            case "EnumProperty":
                var enumProp = _factory.Cast<IFEnumProperty>(prop);
                var enumPropName = enumProp.Enum.NamePrivate.ToString();
                GenerateEnumDefinition(enumProp.Enum, GetPropertyTypeName(enumProp.UnderlyingProp));
                return () => SanitizeName(enumPropName);
            case "MapProperty":
                var mapProp = _factory.Cast<IFMapProperty>(prop);
                var mapPropKeyType = GetPropertyTypeName(mapProp.KeyProp);
                var mapPropValueType = GetPropertyTypeName(mapProp.ValueProp);
                return () =>
                {
                    var isKeyPtr = mapPropKeyType.EndsWith('*') || mapPropKeyType.Contains('<'); // Use nint for pointers and generic types.
                    var isValuePtr = mapPropValueType.EndsWith('*') || mapPropValueType.Contains('<');
                    
                    _UStructDefinitions.TryGetValue(mapPropKeyType.TrimEnd('*'), out var knownKeyStruct);
                    _UStructDefinitions.TryGetValue(mapPropValueType.TrimEnd('*'), out var knownValueStruct);

                    var keyName = SanitizeName(knownKeyStruct?.DisplayName ?? mapPropKeyType);
                    var valueName = SanitizeName(knownValueStruct?.DisplayName ?? mapPropValueType);
                    
                    var keyType = isKeyPtr ? $"Ptr<{keyName}>" : keyName;
                    var valueType = isValuePtr ? $"Ptr<{valueName}>" : valueName;
                   
                    return $"TMap<{keyType}, {valueType}>";
                };
            case "InterfaceProperty":
                var intPropType = _factory.Cast<IFInterfaceProperty>(prop).InterfaceClass.NamePrivate.ToString();
                return () => SanitizeName(_UStructDefinitions.TryGetValue(intPropType, out var knownStruct) ? $"TScriptInterface<{knownStruct.DisplayName}>" : $"TScriptInterface<{intPropType}>");
            case "ArrayProperty":
                var arrayPropType = GetPropertyTypeName(_factory.Cast<IFArrayProperty>(prop).Inner);
                return () =>
                {
                    var isPtrType = arrayPropType.EndsWith('*') || arrayPropType.Contains('<'); // Use nint for pointers and generic types.
                    _UStructDefinitions.TryGetValue(arrayPropType.TrimEnd('*'), out var knownStruct);
                    var arrTypeSanitized = SanitizeName(knownStruct?.DisplayName ?? arrayPropType);
                    return isPtrType ?
                        $"TArray<Ptr<{arrTypeSanitized}>>" :
                        $"TArray<{arrTypeSanitized}>";
                };
            case "SetProperty":
                var setPropType = GetPropertyTypeName(_factory.Cast<IFSetProperty>(prop).ElementProp);
                return () =>
                {
                    var isPtrType = setPropType.EndsWith('*') || setPropType.Contains('<'); // Use nint for pointers and generic types.;
                    _UStructDefinitions.TryGetValue(setPropType.TrimEnd('*'), out var knownStruct);
                    return isPtrType
                        ? $"TSet<nint> /* TSet<{setPropType}> */"
                        : $"TSet<{SanitizeName(knownStruct?.DisplayName ?? setPropType)}>";
                };
            case "OptionalProperty":
                var optionalType = GetPropertyTypeName(_factory.Cast<IFOptionalProperty>(prop).ValueProperty);
                return () =>
                {
                    if (_UStructDefinitions.TryGetValue(optionalType, out var knownOptType))
                        optionalType = knownOptType.DisplayName;
                    
                    return $"TOptional<{SanitizeName(optionalType)}>";
                };
            case "FieldPathProperty":
                return () => "FFieldPath";
            case "LazyObjectProperty":
                var lazyObjType = _factory.Cast<IFObjectProperty>(prop).PropertyClass.NamePrivate.ToString();
                return () =>
                {
                    if (_UStructDefinitions.TryGetValue(lazyObjType, out var knownLazyType))
                        lazyObjType = knownLazyType.DisplayName;
                    
                    return $"TLazyObjectPtr<{SanitizeName(lazyObjType)}>";
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

    /// <summary>
    /// Gets the property type name, such as <c>byte</c> for <c>ByteProperty</c>.
    /// </summary>
    /// <param name="prop"></param>
    /// <returns></returns>
    private string GetPropertyTypeName(IFProperty prop)
    {
        var className = prop.ClassPrivate.Name;
        switch (className)
        {
            case "BoolProperty":
                return "bool";
            case "ByteProperty" or "Int8Property":
                return "byte";
            case "Int16Property":
                return "short";
            case "UInt16Property":
                return "ushort";
            case "IntProperty":
                return "int";
            case "UInt32Property":
                return "uint";
            case "Int64Property":
                return "long";
            case "UInt64Property":
                return "ulong";
            case "FloatProperty":
                return "float";
            case "DoubleProperty":
                return "double";
            case "NameProperty":
                return "FName";
            case "StrProperty":
                return "FString";
            case "TextProperty":
                return "FText";
            case "DataTableRowHandle":
                return "FDataTableRowHandle";
            case "ObjectProperty":
                return $"{_factory.Cast<IFObjectProperty>(prop).PropertyClass.NamePrivate}*";
            case "SoftObjectProperty":
                return $"TSoftObjectPtr<{_factory.Cast<IFObjectProperty>(prop).PropertyClass.NamePrivate}>";
            case "SoftClassProperty":
                return $"TSoftClassPtr<{_factory.Cast<IFSoftClassProperty>(prop).MetaClass.NamePrivate}>";
            case "StructProperty":
                return _factory.Cast<IFStructProperty>(prop).Struct.NamePrivate.ToString();
            case "ClassProperty":
            case "ClassPtrProperty":
                return _factory.Cast<IFClassProperty>(prop).MetaClass!.NamePrivate.ToString();
            case "EnumProperty":
                return _factory.Cast<IFEnumProperty>(prop).Enum.NamePrivate.ToString();
            case "MapProperty":
                var mapProp = _factory.Cast<IFMapProperty>(prop);
                var mapPropKeyType = GetPropertyTypeName(mapProp.KeyProp);
                var mapPropValueType = GetPropertyTypeName(mapProp.ValueProp);
                return $"TMap<{mapPropKeyType}, {mapPropValueType}>";
            case "InterfaceProperty":
                return $"TScriptInterface<{_factory.Cast<IFInterfaceProperty>(prop).NamePrivate}>";
            case "ArrayProperty":
                return $"TArray<{GetPropertyTypeName(_factory.Cast<IFArrayProperty>(prop).Inner)}>";
            case "SetProperty":
                return $"TSet<{GetPropertyTypeName(_factory.Cast<IFSetProperty>(prop).ElementProp)}>";
            case "DelegateProperty":
                return "FScriptDelegate";
            case "MulticastInlineDelegateProperty":
            case "MulticastSparseDelegateProperty":
                return "FMulticastScriptDelegate";
            case "WeakObjectProperty":
                return "FWeakObjectPtr";
            case "FieldPathProperty":
                return "FFieldPath";
            case "Utf8StrProperty":
                return "FUtf8String";
            case "AnsiStrProperty":
                return "FAnsiString";
            default:
                Log.Warning($"Unknown Property: {className}");
                return className;
        }
    }

    private (string Name, int Size, int Offset, string ClassName) GetBaseInfo(IFProperty prop)
    {
        var name = SanitizeName(prop.NamePrivate);
        var size = prop.ElementSize;
        var offset = prop.Offset_Internal;
        var className = prop.ClassPrivate.Name;
        return (name, size, offset, className);
    }

    private static string SanitizeName(string name)
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
        if (name == "object") name = "_object";
        
        if (char.IsDigit(name[0]))
        {
            name = '_' + name;
        }

        return name;
    }

    private interface ICsharpText
    {
        string Name { get; }
    
        string GetCsharpText();
    }
}