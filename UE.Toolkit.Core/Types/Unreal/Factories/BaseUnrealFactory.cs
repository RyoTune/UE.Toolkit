using UE.Toolkit.Core.Types.Interfaces;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;

namespace UE.Toolkit.Core.Types.Unreal.Factories;

public abstract class BaseUnrealFactory : IUnrealFactory
{
    
    public IUnrealMemoryInternal? Memory { get; set; }
    
    public T Cast<T>(IPtr obj)
    {
        var typeName = typeof(T).Name;
        switch (typeName)
        {
            case nameof(IUObject):
                return (T)CreateUObject(obj.Ptr);
            case nameof(IUClass):
                return (T)CreateUClass(obj.Ptr);
            case nameof(IUScriptStruct):
                return (T)CreateUScriptStruct(obj.Ptr);
            case nameof(IUEnum):
                return (T)CreateUEnum(obj.Ptr);
            case nameof(IUUserDefinedEnum):
                return (T)CreateUUserDefinedEnum(obj.Ptr);
            
            case nameof(IFByteProperty):
                return (T)CreateFByteProperty(obj.Ptr);
            case nameof(IFBoolProperty):
                return (T)CreateFBoolProperty(obj.Ptr);
            case nameof(IFEnumProperty):
                return (T)CreateFEnumProperty(obj.Ptr);
            case nameof(IFObjectProperty):
                return (T)CreateFObjectProperty(obj.Ptr);
            case nameof(IFSoftClassProperty):
                return (T)CreateFSoftClassProperty(obj.Ptr);
            case nameof(IFClassProperty):
                return (T)CreateFClassProperty(obj.Ptr);
            case nameof(IFStructProperty):
                return (T)CreateFStructProperty(obj.Ptr);
            case nameof(IFMapProperty):
                return (T)CreateFMapProperty(obj.Ptr);
            case nameof(IFInterfaceProperty):
                return (T)CreateFInterfaceProperty(obj.Ptr);
            case nameof(IFArrayProperty):
                return (T)CreateFArrayProperty(obj.Ptr);
            case nameof(IFSetProperty):
                return (T)CreateFSetProperty(obj.Ptr);
            case nameof(IFOptionalProperty):
                return (T)CreateFOptionalProperty(obj.Ptr);
            case nameof(IFDelegateProperty):
                return (T)CreateFDelegateProperty(obj.Ptr);
            
            default:
                throw new NotSupportedException(typeName);
        }
    }
    public abstract nint SizeOf<T>();
    // Type alignment when placed inline, such as the value type in a TMap
    public nint GetAlignment(IFProperty prop)
    {
        return prop.ClassPrivate.Name switch
        {
            "Int8Property" or "BoolProperty" => 1,
            "Int16Property" or "UInt16Property" => 2,
            "IntProperty" or "Int32Property" or "UInt32Property" or "FloatProperty" or "NameProperty" => 4,
            "Int64Property" or "UInt64Property" or "DoubleProperty" or "StrProperty" or "TextProperty" or "ObjectProperty" 
                or "SoftObjectProperty" or "SoftClassProperty" or "ArrayProperty" => 8,
            "StructProperty" => CreateFStructProperty(prop.Ptr).Struct.MinAlignment,
            _ => throw new NotSupportedException(prop.ClassPrivate.Name)
        };
    }

    public abstract IFProperty CreateFProperty(nint ptr);
    public abstract IFBoolProperty CreateFBoolProperty(nint ptr);
    public abstract IFByteProperty CreateFByteProperty(nint ptr);
    public abstract IFEnumProperty CreateFEnumProperty(nint ptr);
    public abstract IFObjectProperty CreateFObjectProperty(nint ptr);
    public abstract IFSoftClassProperty CreateFSoftClassProperty(nint ptr);
    public abstract IFClassProperty CreateFClassProperty(nint ptr);
    public abstract IFStructProperty CreateFStructProperty(nint ptr);
    public abstract IFMapProperty CreateFMapProperty(nint ptr);
    public abstract IFInterfaceProperty CreateFInterfaceProperty(nint ptr);
    public abstract IFArrayProperty CreateFArrayProperty(nint ptr);
    public abstract IFSetProperty CreateFSetProperty(nint ptr);
    public abstract IFOptionalProperty CreateFOptionalProperty(nint ptr);
    public abstract IFDelegateProperty CreateFDelegateProperty(nint ptr);
    
    public abstract IUObjectArray CreateUObjectArray(nint ptr);
    
    public abstract IUObject CreateUObject(nint ptr);
    public abstract IUClass CreateUClass(nint ptr);
    public abstract IUScriptStruct CreateUScriptStruct(nint ptr);
    public abstract IUEnum CreateUEnum(nint ptr);
    public abstract IUField CreateUField(nint ptr);
    public abstract IUStruct CreateUStruct(nint ptr);
    public abstract IUUserDefinedEnum CreateUUserDefinedEnum(nint ptr);
    // public abstract IUPackage CreateUPackage(nint ptr);
    public abstract IUFunction CreateUFunction(nint ptr);
    
    public abstract IFFieldClass CreateFFieldClass(nint ptr);
    public abstract IFField CreateFField(nint ptr);
    public abstract IFFieldVariant CreateFFieldVariant(nint ptr); 
    
    public abstract IFStructParams CreateFStructParams(nint ptr);
    public abstract IFPropertyParams CreateFPropertyParams(nint ptr);
    public abstract IFGenericPropertyParams CreateFGenericPropertyParams(nint ptr);
    
    public abstract IFWorldContext CreateFWorldContext(nint ptr);
    public abstract IUEngine CreateUEngine(nint ptr);
    public abstract IUGameInstance CreateUGameInstance(nint ptr);
    
    public abstract IFStaticConstructObjectParameters CreateFStaticConstructObjectParameters();
    public abstract IFActorSpawnParameters CreateFActorSpawnParameters();
}