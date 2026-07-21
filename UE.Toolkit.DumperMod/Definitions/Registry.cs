using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.DumperMod.Definitions;

public enum ObjectType
{
    Class, // inherits from UClass
    Struct, // inherits from UScriptStruct
    Enum, // inherits from UEnum
    // Interface, // inherits from UInterface
    Other,
}

public class Registry(Context context)
{
    private Context Context = context;

    public Dictionary<string, StructDefinition> Structs = [];
    public Dictionary<string, EnumDefinition> Enums = [];
    private Dictionary<string, ObjectType> Types = [];

    private ObjectType FromTypeName(IUClass uclass) => uclass.NamePrivate.ToString() switch
    {
        "Class" => ObjectType.Class,
        "ScriptStruct" => ObjectType.Struct,
        "Enum" => ObjectType.Enum,
        // "Interface" => ObjectType.Interface,
        _ => ObjectType.Other
    };

    private ObjectType RetrieveObjectType(IUObject uobject)
    {
        string ClassName = uobject.ClassPrivate.NamePrivate.ToString();
        IUClass? CurrentClass = uobject.ClassPrivate;
        while (CurrentClass != null)
        {
            var CurrentType = FromTypeName(CurrentClass);
            if (CurrentType != ObjectType.Other)
            {
                Types[ClassName] = CurrentType;
                return CurrentType;
            }
            CurrentClass = CurrentClass.GetSuperClass();
        }
        Types[ClassName] = ObjectType.Other;
        return ObjectType.Other;
    }

    public void Register()
    {
        var objectArray = Context.Objects.GUObjectArray;
        for (var i = 0; i < objectArray.NumElements; i++)
        {
            var obj = objectArray.IndexToObject(i);
            if (obj == null) continue;
            
            /*
            var typeName = obj.ClassPrivate.NamePrivate.ToString();
            var moduleName = GetModuleNameForPackage(obj.GetOutermost());
            var fileBaseName = obj.ClassPrivate.NamePrivate.ToString();
            Log.Debug($"Object {obj.NamePrivate} | Filename {fileBaseName} | Module {moduleName}");
            */
            var typeName = obj.ClassPrivate.NamePrivate.ToString();
            IObjectFactory? factory = (Types.TryGetValue(typeName, out var objectType) ? objectType : RetrieveObjectType(obj)) switch
                {
                    ObjectType.Class => new ClassFactory(Context, Context.Factory.Cast<IUClass>(obj)),
                    ObjectType.Struct => new StructFactory(Context, Context.Factory.Cast<IUScriptStruct>(obj)),
                    ObjectType.Enum => new EnumFactory(Context, Context.Factory.Cast<IUEnum>(obj)),
                    _ => null
                };
            factory?.Register();
            /*
            if (obj.IsChildOf<UClass>())
            {
                var uclass = Context.Factory.Cast<IUClass>(obj);
                if (obj.IsChildOf<UInterface>())
                {
                    // TODO:
                    var interfaceName = obj.NamePrivate.ToString();
                    Context.Registry.Structs[interfaceName] = new(interfaceName, interfaceName, 0, 0, [], null);
                    Log.Debug($"Interface: {interfaceName}");
                }
                else
                {
                    Context.ClassFactory.Register(uclass);
                }
            }
            else if (obj.IsChildOf<UScriptStruct>())
            {
                Context.StructFactory.Register(Context.Factory.Cast<IUScriptStruct>(obj));
            }
            else if (obj.IsChildOf<UEnum>())
            {
                Context.EnumFactory.Register(Context.Factory.Cast<IUEnum>(obj));
            }
            */
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
    
    private static unsafe string GetHeaderNameForObject(IUObject obj)
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
}