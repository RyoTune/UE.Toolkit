using System.Xml;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Reloaded.ObjectWriters.Nodes;

public static class MapNodeFactory
{
    public static IFieldNode CreateMapNode(IFMapProperty property, nint value, NodeFactory factory)
    {
        var Key = property.KeyProp;
        switch (Key.ClassPrivate.Name)
        {
            case "IntProperty":
                // Check if it has a StructLayout with an explicit alignment value, this lets us avoid iterating
                // through each field to determine alignment
                // Every type defined in the extension mod has an alignment value in it's StructLayout
                return factory.Factory.GetAlignment(property.ValueProp) switch
                {
                    <= 4 => new MapNodeInt(property, value, factory),
                    _ => new MapNodeInt8(property, value, factory)
                };
            case "NameProperty":
                return new MapNodeFName(property, value, factory);
            default:
                Log.Warning($"{nameof(MapNodeFactory)} || Field '{property.NamePrivate}' with type '{property.KeyProp.NamePrivate}' is not currently supported for map editing operations.");
                return new DummyNode(property);
        }
    }
}

public abstract class BaseMapNode<TKeyValue>(IFMapProperty property, nint value, NodeFactory factory)
    : IFieldNode where TKeyValue : unmanaged, IMapHashable, IEquatable<TKeyValue>
{
    protected IFMapProperty Property => property;
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
                Log.Error($"{nameof(BaseMapNode<TKeyValue>)} || '{WriterConstants.ItemTag}' is missing an ID.");
                break;
            }

            if (!CreateKeyValue(id, out var KeyMaybe))
            {
                Log.Error($"{nameof(BaseMapNode<TKeyValue>)} || Could not process map key {id}");
                break;               
            }
            var Key = KeyMaybe!.Value;
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
                Log.Debug($"{nameof(BaseMapNode<TKeyValue>)} || Added entry at 0x{newEntry:x} with key '{Key}' into '{Property.NamePrivate}'");
            }

            if (tempMap.TryGetValue(Key, out var valuePtr))
            {
                var itemTree = subReader.ReadSubtree();
                itemTree.MoveToContent();
                Factory.Create(Property.ValueProp, valuePtr).ConsumeNode(itemTree);
            }
        }
    }
    
    public abstract TMapDynamicDictionary<TKeyValue> CreateTempMap(
        nint fieldPtr, IFProperty valueType);

    public abstract bool CreateKeyValue(string id, out TKeyValue? Value);

    public virtual bool ContainsKey(TMapDynamicDictionary<TKeyValue> dict, TKeyValue key)
        => dict.ContainsKey(key);
}

public class MapNodeInt(IFMapProperty property, nint value, NodeFactory factory)
    : BaseMapNode<HashableInt>(property, value, factory)
{
    public override unsafe TMapDynamicDictionary<HashableInt> CreateTempMap(nint fieldPtr, IFProperty valueType)
        => new((TMap<HashableInt, byte>*)fieldPtr, new DynamicMapUnrealProperty(valueType), Factory.Memory);

    public override bool CreateKeyValue(string id, out HashableInt? Value)
    {
        Value = null;
        if (!int.TryParse(id, out var itemIdx))
        {
            Log.Warning($"{nameof(MapNodeInt)} || Invalid ID: {id}");
            return false;
        }
        Value = new HashableInt(itemIdx);
        return true;
    }
}

public class MapNodeInt8(IFMapProperty property, nint value, NodeFactory factory)
    : BaseMapNode<HashableInt8>(property, value, factory)
{
    public override unsafe TMapDynamicDictionary<HashableInt8> CreateTempMap(nint fieldPtr, IFProperty valueType)
        => new((TMap<HashableInt8, byte>*)fieldPtr, new DynamicMapUnrealProperty(valueType), Factory.Memory);

    public override bool CreateKeyValue(string id, out HashableInt8? Value)
    {
        Value = null;
        if (!int.TryParse(id, out var itemIdx))
        {
            Log.Warning($"{nameof(MapNodeInt8)} || Invalid ID: {id}");
            return false;
        }
        Value = new HashableInt8(itemIdx);
        return true;
    }
}

public class MapNodeFName(IFMapProperty property, nint value, NodeFactory factory)
    : BaseMapNode<FName>(property, value, factory)
{
    public override unsafe TMapDynamicDictionary<FName> CreateTempMap(nint fieldPtr, IFProperty valueType)
        => new((TMap<FName, byte>*)fieldPtr, new DynamicMapUnrealProperty(valueType), Factory.Memory);

    public override bool CreateKeyValue(string id, out FName? Value)
    {
        Value = new FName(id);
        return true;
    }
}