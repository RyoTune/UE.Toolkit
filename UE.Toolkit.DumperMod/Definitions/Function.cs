using System.Text;
using UE.Toolkit.Core.Types.Interfaces;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.DumperMod.Definitions;

public abstract class ParameterDefinition(string name, Func<string> propTypeName, IFProperty meta) : ISerializable
{
    public string Name { get; } = name;
    public Func<string> PropTypeName { get; } = propTypeName;
    public IFProperty Meta { get; } = meta;

    public bool IsReference => Meta.PropertyFlags.HasFlag(EPropertyFlags.CPF_OutParm);

    public abstract string Serialize(Context context);
}

public class ParameterHeadDefinition(string name, Func<string> propTypeName, IFProperty meta) 
    : ParameterDefinition(name, propTypeName, meta)
{
    public override string Serialize(Context context) => $"{(IsReference ? "ref" : string.Empty)} {PropTypeName()} {Name}";
}

public class ParameterBodyDefinition(string name, Func<string> propTypeName, IFProperty meta, bool isLast) 
    : ParameterDefinition(name, propTypeName, meta)
{
    public bool IsLast { get; set; } = isLast;
    
    public override string Serialize(Context context)
    {
        var typeName = propTypeName();
        var paramTypeName = FunctionParamFactory.GetParamNameFromProperty(Meta, context.Factory, context.Classes);
        if (paramTypeName == null) return $"// PARAM TODO {typeName}";
        typeName = Meta.ClassPrivate.Name switch
        {
            "ArrayProperty" => "TArray<int>",
            "MapProperty" => "TMap<int, int>",
            "SetProperty" => "TSet<int>",
            "ByteProperty" => "byte",
            "InterfaceProperty" => "TScriptInterface<int>",
            "SoftClassProperty" => "TSoftClassPtr<int>",
            "SoftObjectProperty" => "TSoftObjectPtr<int>",
            _ => typeName
        };
        var next = IsLast ? string.Empty : ",";
        if (Meta.ClassPrivate.Name is "ObjectProperty" or "ClassProperty" or "ClassPtrProperty")
            return $"new {paramTypeName}(new(&{Name + "_Ptr"})){next}";
        var ptrCast = IsReference ? $"({typeName}*)Unsafe.AsPointer(ref {Name})" : $"&{Name}";
        if (!IsReference)
        {
            ptrCast = Meta.ClassPrivate.Name switch
            {
                "ByteProperty" => $"(byte*)(&{Name})",
                "ArrayProperty" => $"(TArray<int>*)(&{Name})",
                "MapProperty" => $"(TMap<int, int>*)(&{Name})",
                "SetProperty" => $"(TSet<int>*)(&{Name})",
                "InterfaceProperty" => $"(TScriptInterface<int>*)(&{Name})",
                "SoftClassProperty" => $"(TSoftClassPtr<int>*)(&{Name})",
                "SoftObjectProperty" => $"(TSoftObjectPtr<int>*)(&{Name})",
                _ => ptrCast
            };
        }
        var paramDecl = Meta.ClassPrivate.Name switch
        {
            "BoolProperty" => $"new {paramTypeName}(new({ptrCast}), {context.Factory.CreateFBoolProperty(Meta.Ptr).FieldMask})",
            "StructProperty" => $"new {paramTypeName}(new({ptrCast}), {context.Factory.CreateFStructProperty(Meta.Ptr).Struct.PropertiesSize})",
            "EnumProperty" => $"new {paramTypeName}(new({ptrCast}), {Meta.ElementSize})",
            "TextProperty" => $"new {paramTypeName}(new({ptrCast}), {context.Classes.GetFTextSize()})",
            _ => $"new {paramTypeName}(new({ptrCast}))"
        };
        return $"{paramDecl}{next}";
    }
}

public class ReturnValueDefinition(Func<string> typeName, string metaName, string? paramTypeName) : ISerializable
{
    public Func<string> TypeName => typeName;
    public string MetaName => metaName;
    public string? ParamTypeName => paramTypeName;

    private string CreateStructPropertyReturn()
    {
        var Result = $"{TypeName()} ReturnValue;";
        Result += $"\n\t\t(({ParamTypeName}?)Return)!.Write((nint)(&ReturnValue));";
        Result += "\n\t\treturn ReturnValue;";
        return Result;
    }

    private string CreateSingleLineReturn()
    {
        var ConstructValue = MetaName switch
        {
            "ObjectProperty" or "ClassProperty" or "ClassPtrProperty" 
                => $"new(Inner.GetFactory().CreateUObject((({ParamTypeName}?)Return)!.Value))",
            "ByteProperty" => $"({TypeName()})((({ParamTypeName}?)Return)!.Value)",
            _ => $"(({ParamTypeName}?)Return)!.Value"
        };
        return $"return {ConstructValue};";
    }

    public string Serialize(Context context)
    {
        if (ParamTypeName == null) return $"return null; // RETURN VALUE TODO {TypeName()}";
        return MetaName switch
        {
            "StructProperty" or "EnumProperty" or "TextProperty" or "ArrayProperty" 
                or "MapProperty" or "SetProperty" or "InterfaceProperty" 
                or "SoftClassProperty" or "SoftObjectProperty" or "DelegateProperty"
                or "MulticastInlineDelegateProperty" or "MulticastSparseDelegateProperty"
                => CreateStructPropertyReturn(),
            _ => CreateSingleLineReturn()
        };
    }
}

public class FunctionFactory(Context context)
{
    public List<FunctionDefinition> ResolveFunctions(IUClass uclass)
    {
        var PropFactory = new PropertyClassFactory(context);
        return uclass.GetFunctions()
            .Where(x =>
            {
                var Name = x.NamePrivate.ToString();
                return !(Name.StartsWith("ExecuteUbergraph") || Name.StartsWith("EvaluateGraphExposedInputs"));
            })
            .Select(x =>
        {
            List<ParameterHeadDefinition> HeadParams = [];
            List<ParameterBodyDefinition> BodyParams = [];
            List<string> BodyPrefix = [];
            List<string> BodyPostfix = [];
            ReturnValueDefinition? ReturnValue = null;
            Func<string>? returnTypeName = null;
            Dictionary<string, int> nameUse = [];
            foreach (var Field in x.ChildProperties)
            {
                var Param = context.Factory.CreateFProperty(Field.Ptr);
                if (Param.PropertyFlags.HasFlag(EPropertyFlags.CPF_ReturnParm))
                {
                    returnTypeName = PropFactory.GetPropTypenameFunctionParam(Param);
                    var returnParamName = FunctionParamFactory.GetParamNameFromProperty(Param, context.Factory, context.Classes);
                    ReturnValue = new(returnTypeName, Param.ClassPrivate.Name, returnParamName);
                    break;
                }
                var paramName = Builtins.SanitizeForTypename(Builtins.SanitizeName(Param.NamePrivate));
                if (nameUse.TryGetValue(paramName, out var useCount))
                {
                    paramName += $"_{useCount}";
                    nameUse[paramName] = useCount + 1;
                }
                else
                {
                    nameUse[paramName] = 1;
                }
                var propTypeName = PropFactory.GetPropTypenameFunctionParam(Param);
                if (propTypeName == null)
                {
                    Log.Error($"{nameof(ResolveFunctions)} || Could not determine data types for all parameters in '{x.NamePrivate}'");
                    return null;
                }
                HeadParams.Add(new ParameterHeadDefinition(paramName, propTypeName, Param ));
                BodyParams.Add(new ParameterBodyDefinition(paramName, propTypeName, Param, false ));
                if (Param.ClassPrivate.Name is "ObjectProperty" or "ClassProperty" or "ClassPtrProperty")
                {
                    BodyPrefix.Add($"nint {paramName}_Ptr = {paramName}.Inner.Ptr;");
                    if (Param.PropertyFlags.HasFlag(EPropertyFlags.CPF_OutParm))
                        BodyPostfix.Add($"{paramName} = new(Inner.GetFactory().CreateUObject({paramName}_Ptr));");
                }
            }
            if (BodyParams.Count > 0) BodyParams.Last().IsLast = true;
            var funcName = x.NamePrivate.ToString();
            var funcNameSanitized = Builtins.SanitizeForFunctionName(Builtins.SanitizeName(funcName));
            return new FunctionDefinition(funcNameSanitized, funcName, returnTypeName, HeadParams, BodyParams, ReturnValue, BodyPrefix, BodyPostfix);
        })
            .Where(x => x != null)
            .ToList();
    }
}

public class FunctionDefinition(string name, string rawName, Func<string>? returnTypeName, List<ParameterHeadDefinition> headParams, 
    List<ParameterBodyDefinition> bodyParams, ReturnValueDefinition? returnParam, List<string> bodyPrefix, List<string> bodyPostfix)
    : ISerializable
{
    public string Name { get; } = name;
    public string RawName { get; } = rawName;
    public Func<string>? ReturnTypeName { get; } = returnTypeName;
    public List<ParameterHeadDefinition> HeadParams { get; } = headParams;
    public List<ParameterBodyDefinition> BodyParams { get; } = bodyParams;
    public ReturnValueDefinition? ReturnParam { get; } = returnParam;
    public List<string> BodyPrefix { get; } = bodyPrefix;
    public List<string> BodyPostfix { get; } = bodyPostfix;

    public string Serialize(Context context)
    {
        var sb = new StringBuilder();
        var returnType = ReturnTypeName?.Invoke() ?? "void";
        sb.AppendLine($"\tpublic unsafe {returnType} {Name}({string.Join(",", HeadParams.Select(x => x.Serialize(context)))})");
        sb.AppendLine("\t{");
        foreach (var preLine in BodyPrefix) sb.AppendLine($"\t\t{preLine}");
        var outVar = (ReturnTypeName != null ? "var Return" : "_");
        if (BodyParams.Count > 0)
        {
            sb.AppendLine($"\t\t_ = Inner.ProcessEvent(\"{RawName}\", [");
            foreach (var bodyParam in BodyParams) sb.AppendLine($"\t\t\t{bodyParam.Serialize(context)}");
            sb.AppendLine($"\t\t], out {outVar});");
        }
        else sb.AppendLine($"\t\t_ = Inner.ProcessEvent(\"{RawName}\", [], out {outVar});");
        foreach (var postLine in BodyPostfix) sb.AppendLine($"\t\t{postLine}");
        if (ReturnParam != null) sb.AppendLine($"\t\t{ReturnParam.Serialize(context)}");
        sb.AppendLine("\t}");
        return sb.ToString();
    }
}