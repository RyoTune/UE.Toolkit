using UE.Toolkit.Core.Types.Interfaces;
using UE.Toolkit.Core.Types.Unreal.Factories;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;
using UE.Toolkit.Interfaces;
using UE.Toolkit.Reloaded.Common.GameConfigs;

namespace UE.Toolkit.Reloaded.ObjectWriters.Nodes;

public class NodeFactory(IUnrealObjects objects, IUnrealMemoryInternal memory, 
    IUnrealClasses classes, IUnrealFactory factory)
{
    public IUnrealMemoryInternal Memory => memory;
    public IUnrealObjects Objects => objects;
    public IUnrealClasses Classes => classes;
    public IUnrealFactory Factory => factory;

    private unsafe IFieldNode StructNodeHandleSpecial(IFStructProperty property, nint fieldPtr)
    {
        var structName = property.Struct.NamePrivate.ToString();
        Log.Debug($"Handle StructProperty {structName}");
        return structName switch
        {
            "Guid" => new GuidNode(property, new((Guid*)fieldPtr)),
            "SoftObjectPath" => new SoftPathNode(property, GameConfig.Instance.IntoSoftObjectPath(fieldPtr)),
            _ => new StructNode(property, fieldPtr, this)
        };
    }
    
    public unsafe IFieldNode Create(IFProperty property, nint fieldPtr)
    {
        var className = property.ClassPrivate.Name;
        return className switch
        {
            "BoolProperty" => new BoolNode(Factory.CreateFBoolProperty(property.Ptr), new((byte*)fieldPtr)),
            "ByteProperty" => new ByteNode(Factory.CreateFByteProperty(property.Ptr), new((byte*)fieldPtr)),
            "Int8Property" => new Int8Node(property, new((byte*)fieldPtr)),
            "Int16Property" => new Int16Node(property, new((short*)fieldPtr)),
            "UInt16Property" => new UInt16Node(property, new((ushort*)fieldPtr)),
            "IntProperty" or "Int32Property" => new Int32Node(property, new((int*)fieldPtr)),
            "UInt32Property" => new UInt32Node(property, new((uint*)fieldPtr)),
            "Int64Property" => new Int64Node(property, new((long*)fieldPtr)),
            "UInt64Property" => new UInt64Node(property, new((ulong*)fieldPtr)),
            "FloatProperty" => new FloatNode(property, new((float*)fieldPtr)),
            "DoubleProperty" => new DoubleNode(property, new((double*)fieldPtr)),
            "NameProperty" => new NameNode(property, new((FName*)fieldPtr)),
            "StrProperty" => new StrNode(property, new((FString*)fieldPtr), Objects, Memory),
            "TextProperty" => new TextNode(property, new((FText*)fieldPtr), Objects),
            "ObjectProperty" => new ObjectNode(Factory.CreateFObjectProperty(property.Ptr), fieldPtr, this),
            "SoftObjectProperty" => new SoftObjectNode(property, new((FSoftObjectPtr*)fieldPtr), Classes),
            "SoftClassProperty" => new SoftClassNode(property, new((FSoftObjectPtr*)fieldPtr), Classes),
            "StructProperty" => StructNodeHandleSpecial(Factory.CreateFStructProperty(property.Ptr), fieldPtr),
            "EnumProperty" => new EnumNode(Factory.CreateFEnumProperty(property.Ptr), new((nint*)fieldPtr)),
            "MapProperty" => MapNodeFactory.CreateMapNode(Factory.CreateFMapProperty(property.Ptr), fieldPtr, this),
            "ArrayProperty" => new ArrayNode(Factory.CreateFArrayProperty(property.Ptr), fieldPtr, this),
            _ => new DummyNode(property)
        };
    }
}