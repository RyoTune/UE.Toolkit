using System.Diagnostics.CodeAnalysis;
using System.Xml;
using UE.Toolkit.Core.Types.Unreal.Common.DynamicMap;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Reloaded.ObjectWriters.Nodes;

public static class MapNodeFactory
{
    private static bool CreateMapKey(IFMapProperty property, NodeFactory factory, [NotNullWhen(true)] out IDynamicMapKeyType? MapKey)
    {
        var Key = property.KeyProp;
        MapKey = Key.ClassPrivate.Name switch
        {
            "IntProperty" => new IntDynamicMapKeyType(property, factory.Factory),
            "NameProperty" => new NameDynamicMapKeyType(property, factory.Factory),
            _ => null
        };
        return MapKey != null;
    }
    
    public static IFieldNode CreateMapNode(IFMapProperty property, nint value, NodeFactory factory)
    {
        var Key = property.KeyProp;
        if (!CreateMapKey(property, factory, out var mapKey))
        {
            Log.Warning($"{nameof(MapNodeFactory)} || Field '{property.NamePrivate}' with type '{factory.Classes.GetPropertyTypeName(property.KeyProp)}' is not currently supported for map editing operations.");
            return new DummyNode(property);
        }
        return new MapNode(property, mapKey, value, factory);
    }
}

public class MapNode(IFMapProperty property, IDynamicMapKeyType mapKey, nint value, NodeFactory factory)
    : IFieldNode
{
    protected IFMapProperty Property => property;
    protected IDynamicMapKeyType MapKey => mapKey;
    protected nint Value => value;
    protected NodeFactory Factory => factory;
    
    public void ConsumeNode(XmlReader reader)
    {
        var tempMap = CreateTempMap(Value, Property.ValueProp);
        
        // Get any item nodes.
        using var subReader = reader.ReadSubtree();
        subReader.MoveToContent();

        while (subReader.Read())
        {
            if (subReader.NodeType != XmlNodeType.Element) continue;
            if (subReader.Name != WriterConstants.ItemTag)
                throw new(
                    $"Only '{WriterConstants.ItemTag}' elements can be directly inside a map. Found: {subReader.Name}");

            var id = subReader.GetAttribute(WriterConstants.ItemIdAttr);
            if (id == null)
            {
                Log.Error($"{nameof(MapNode)} || '{WriterConstants.ItemTag}' is missing an ID.");
                break;
            }

            if (!MapKey.FromString(id, out var Key))
            {
                Log.Error($"{nameof(MapNode)} || Could not process map key {id}");
                break;               
            }
            unsafe
            {
                var tempMapBitAlloc = (TArray<byte>*)(tempMap.Self + 0x20); // MapConstants.SIZE_OF_ARRAY + 0x10
                // Ensure that TMap is properly initialized for newly allocated items - BitAllocator's capacity should
                // be 0x80 (128) to account for inline bits (equivalent to Reset()).
                if (tempMapBitAlloc->ArrayMax < 128)
                    tempMapBitAlloc->ArrayMax = 128;
            }
            
            // Get existing value
            if (!ContainsKey(tempMap, Key))
            {
                var newEntry = Factory.Memory.MallocZeroed(Property.ValueProp.ElementSize);
                tempMap.AddIndirect(Key, newEntry);
                Factory.Memory.Free(newEntry);
                Log.Debug($"{nameof(MapNode)} || Added entry at 0x{newEntry:x} with key '{Key}' into '{Property.NamePrivate}'");
            }

            if (tempMap.TryGetValue(Key, out var valuePtr))
            {
                var itemTree = subReader.ReadSubtree();
                itemTree.MoveToContent();
                Factory.Create(Property.ValueProp, valuePtr).ConsumeNode(itemTree);
            }
        }
    }

    public TMapDynamicDictionary CreateTempMap(nint fieldPtr, IFProperty valueType)
        => new(fieldPtr, MapKey, new DynamicMapValueUnrealProperty(valueType), Factory.Memory);
    
    public virtual bool ContainsKey(TMapDynamicDictionary dict, IDynamicMapKey key) => dict.ContainsKey(key);
}