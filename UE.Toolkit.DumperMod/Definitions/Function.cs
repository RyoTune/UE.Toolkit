using System.Text;
using UE.Toolkit.Core.Types.Interfaces;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.DumperMod.Definitions;

public abstract class ParameterDefinition(string name, Func<string> propTypeName, bool isReference) : ISerializable
{
    public string Name { get; } = name;
    public Func<string> PropTypeName { get; } = propTypeName;
    public bool IsReference { get; } = isReference;

    public abstract string Serialize(Context context);
}

public class ParameterHeadDefinition(string name, Func<string> propTypeName, bool isReference) 
    : ParameterDefinition(name, propTypeName, isReference)
{
    public override string Serialize(Context context) => $"{(IsReference ? "ref" : string.Empty)} {PropTypeName()} {Name}";
}

public class ParameterBodyDefinition(string name, Func<string> propTypeName, bool isReference, string? paramTypeName, bool isLast) 
    : ParameterDefinition(name, propTypeName, isReference)
{
    public string? ParamTypeName => paramTypeName;
    public bool IsLast { get; set; } = isLast;
    
    public override string Serialize(Context context)
    {
        var typeName = propTypeName();
        if (ParamTypeName == null)
        {
            return $"// PARAM TODO {typeName}";
        }
        var ptrCast = IsReference ? $"&{Name}" : $"({typeName}*)Unsafe.AsPointer(ref {Name})";
        var paramDecl = ParamTypeName switch
        {
            _ => $"new {ParamTypeName}(new({ptrCast}))"
        };
        return $"{paramDecl}{(IsLast ? string.Empty : ",")}";
    }
}

public class ReturnValueDefinition(string? paramTypeName) : ISerializable
{
    public string? ParamTypeName => paramTypeName;

    public string Serialize(Context context)
    {
        if (ParamTypeName == null)
        {
            return $"return null; // RETURN VALUE TODO {ParamTypeName}";
        }
        var ConstructValue = ParamTypeName switch
        {
            "ObjectProperty" or "ClassProperty" or "ClassPtrProperty" 
                => $"new(Inner.GetFactory().GetUObject((({ParamTypeName})Return).Value))",
            _ => $"(({ParamTypeName})Return).Value"
        };
        return $"return {ConstructValue};";
    }
}

public class FunctionFactory(Context context)
{
    public List<FunctionDefinition> ResolveFunctions(IUClass uclass)
    {
        var PropFactory = new PropertyClassFactory(context);
        return uclass.GetFunctions()
            .Where(x => !x.NamePrivate.ToString().StartsWith("ExecuteUbergraph"))
            .Select(x =>
        {
            List<ParameterHeadDefinition> HeadParams = [];
            List<ParameterBodyDefinition> BodyParams = [];
            ReturnValueDefinition? ReturnValue = null;
            Func<string>? returnTypeName = null;
            foreach (var Field in x.ChildProperties)
            {
                var Param = context.Factory.CreateFProperty(Field.Ptr);
                if (Param.PropertyFlags.HasFlag(EPropertyFlags.CPF_ReturnParm))
                {
                    returnTypeName = PropFactory.GetPropTypenameStruct(Param);
                    var returnParamName = FunctionParamFactory.GetParamNameFromProperty(Param, context.Factory, context.Classes);
                    ReturnValue = new(returnParamName);
                    break;
                }
                var IsReference = Param.PropertyFlags.HasFlag(EPropertyFlags.CPF_OutParm);
                var paramCSName = FunctionParamFactory.GetParamNameFromProperty(Param, context.Factory, context.Classes);
                var typeName = Builtins.SanitizeName(Param.NamePrivate);
                var propTypeName = PropFactory.GetPropTypenameStruct(Param);
                HeadParams.Add(new ParameterHeadDefinition(typeName, propTypeName, IsReference ));
                BodyParams.Add(new ParameterBodyDefinition(typeName, propTypeName, IsReference, paramCSName, false ));
            }
            if (BodyParams.Count > 0) BodyParams.Last().IsLast = true;
            return new FunctionDefinition(Builtins.SanitizeName(x.NamePrivate.ToString()), returnTypeName, HeadParams, BodyParams, ReturnValue);
        }).ToList();
    }
}

public class FunctionDefinition(string name, Func<string>? returnTypeName, List<ParameterHeadDefinition> headParams, 
    List<ParameterBodyDefinition> bodyParams, ReturnValueDefinition? returnParam) 
    : ISerializable
{
    public string Name { get; } = name;
    public Func<string>? ReturnTypeName { get; } = returnTypeName;
    public List<ParameterHeadDefinition> HeadParams { get; } = headParams;
    public List<ParameterBodyDefinition> BodyParams { get; } = bodyParams;
    public ReturnValueDefinition? ReturnParam { get; } = returnParam;

    public string Serialize(Context context)
    {
        var sb = new StringBuilder();
        var returnType = ReturnTypeName?.Invoke() ?? "void";
        sb.AppendLine($"\tpublic unsafe {returnType} {Name}({string.Join(",", HeadParams.Select(x => x.Serialize(context)))})");
        sb.AppendLine("\t{");
        var outVar = (ReturnTypeName != null ? "var Return" : "_");
        if (BodyParams.Count > 0)
        {
            sb.AppendLine($"\t\t_ = Inner.ProcessEvent(\"{Name}\", [");
            foreach (var bodyParam in BodyParams) sb.AppendLine($"\t\t\t{bodyParam.Serialize(context)}");
            sb.AppendLine($"\t\t], out {outVar});");
        }
        else sb.AppendLine($"\t\t_ = Inner.ProcessEvent(\"{Name}\", [], out {outVar});");
        if (ReturnParam != null) sb.AppendLine($"\t\t{ReturnParam.Serialize(context)}");
        sb.AppendLine("\t}");
        return sb.ToString();
    }
}