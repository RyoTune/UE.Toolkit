using System.Xml;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;

namespace UE.Toolkit.Reloaded.ObjectWriters.Nodes;

public class StructDocument : IFieldNode
{
    private static readonly Dictionary<string, Dictionary<string, IFProperty>> CachedStructsFields = [];
    
    private string StructName { get; }
    private IUStruct Property { get; }
    // private string StructName => Property.NamePrivate.ToString();
    private nint BaseAddress { get; }
    private NodeFactory Factory { get; }
    private readonly Dictionary<string, IFProperty> _fields;
    
    public StructDocument(string structName, IUStruct property, nint baseAddress, NodeFactory factory)
    {
        StructName = structName;
        Property = property;
        BaseAddress = baseAddress;
        Factory = factory;
        
        // Cache struct fields data cause reflection slow...
        if (CachedStructsFields.TryGetValue(property.NamePrivate.ToString(), out var cachedFields))
        {
            _fields = cachedFields;
        }
        else
        {
            _fields = CachedStructsFields[property.NamePrivate.ToString()] =
                property.PropertyLink.Select(x => (x.NamePrivate, x)).ToDictionary();
            CachedStructsFields[property.NamePrivate.ToString()] = _fields;
        }
    }

    public unsafe void ConsumeNode(XmlReader reader)
    {
        var anyElementFound = false;
        while (reader.Read())
        {
            if (reader.Name == StructName || reader.NodeType == XmlNodeType.EndElement) break;
            if (reader.NodeType != XmlNodeType.Element) continue;
            if (AtItemElement(reader)) Log.Warning($"{nameof(StructDocument)} || Unexpected '{WriterConstants.ItemTag}' element found. Error?");
            
            anyElementFound = true;
            
            var fieldName = reader.Name;
            if (_fields.TryGetValue(fieldName, out var fieldData))
            {
                var fieldPtr = BaseAddress + fieldData.Offset_Internal;
                Log.Debug($"StructDocument->{fieldData.NamePrivate} @ 0x{fieldPtr:x}");
                Factory.Create(fieldData, fieldPtr).ConsumeNode(reader);
            }
            else
            {
                Log.Warning($"{nameof(StructDocument)} || Field '{fieldName}' not found in struct '{StructName}'.");
                break;
            }
        }
        
        if (!anyElementFound) Log.Warning($"{nameof(StructDocument)} || No elements found in '{StructName}'. Error?");
        Log.Verbose($"{nameof(StructDocument)} || Field '{StructName}' node consumed.");
    }
    
    private static bool AtItemElement(XmlReader reader)
        => reader.Name == WriterConstants.ItemTag
           && reader.GetAttribute(WriterConstants.ItemIdAttr) != null;
}