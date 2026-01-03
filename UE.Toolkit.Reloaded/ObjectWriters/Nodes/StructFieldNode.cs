using System.Runtime.InteropServices;
using System.Xml;

namespace UE.Toolkit.Reloaded.ObjectWriters.Nodes;

internal record FieldData(Type type, nint offset, int bit);

public class StructFieldNode : IFieldNode
{
    private static readonly Dictionary<string, Dictionary<string, FieldData>> CachedStructsFields = [];
    
    private readonly Dictionary<string, FieldData> _fields;
    private readonly string _structName;
    private readonly nint _structPtr;
    private readonly Type _structType;
    private readonly FieldNodeFactory _nodeFactory;

    private static Dictionary<string, FieldData> GetFieldsFromStruct(Type structType)
    {
        var FieldList = new Dictionary<string, FieldData>();
        int? LastOffset = null;
        int CurrentBit = 0;
        foreach (var Field in structType.GetFields().SelectMany((x, i) =>
                 {
                     if (i == 0 && x.Name == "Super") return GetFieldsFromStruct(x.FieldType).AsEnumerable();
                     // Handle cases where sequences of booleans are in bitflag form (1 bit) instead of C form (1 byte)
                     // We'll need to save the current bit as well to correctly write values for Object XML
                     var CurrentOffset = (int)Marshal.OffsetOf(structType, x.Name);
                     CurrentBit = x.FieldType == typeof(bool) && LastOffset == CurrentOffset ? CurrentBit + 1 : 0;
                     LastOffset = CurrentOffset;
                     return Enumerable.Repeat<KeyValuePair<string, FieldData>>(new(
                         x.Name, new(x.FieldType, CurrentOffset, CurrentBit)), 1);
                 }))
        {
            // ToDictionary() implementation will crash if there are duplicate field names
            // While this shouldn't be the case with new struct dumps, make sure that the old 
            // format still works
            FieldList.TryAdd(Field.Key, Field.Value);
        }
        return FieldList;
    }

    public StructFieldNode(string structName, nint structPtr, Type structType, FieldNodeFactory nodeFactory)
    {
        _structName = structName;
        _structPtr = structPtr;
        _structType = structType;
        _nodeFactory = nodeFactory;

        // Cache struct fields data cause reflection slow...
        if (CachedStructsFields.TryGetValue(structType.Name, out var cachedFields))
        {
            _fields = cachedFields;
        }
        else
        {
            _fields = GetFieldsFromStruct(structType);
            CachedStructsFields[structType.Name] = _fields;
        }
    }

    public void ConsumeNode(XmlReader reader)
    {
        var anyElementFound = false;
        while (reader.Read())
        {
            if (reader.Name == _structName || reader.NodeType == XmlNodeType.EndElement) break;
            if (reader.NodeType != XmlNodeType.Element) continue;
            if (AtItemElement(reader)) Log.Warning($"{nameof(StructFieldNode)} || Unexpected '{WriterConstants.ItemTag}' element found. Error?");
            
            anyElementFound = true;
            
            var fieldName = reader.Name;
            if (_fields.TryGetValue(fieldName, out var fieldData))
            {
                var fieldType = fieldData.type;
                var fieldPtr = _structPtr + fieldData.offset;
                if (_nodeFactory.TryCreate(fieldName, fieldPtr, fieldData.bit, fieldType, out var fieldNode))
                {
                    fieldNode.ConsumeNode(reader);
                }
                else
                {
                    Log.Warning($"{nameof(StructFieldNode)} || Unsupported node type: {fieldType.Name}");
                    break;
                }
            }
            else
            {
                Log.Warning($"{nameof(StructFieldNode)} || Field '{fieldName}' not found in struct '{_structType.Name}'.");
                break;
            }
        }
        
        if (!anyElementFound) Log.Warning($"{nameof(StructFieldNode)} || No elements found in '{_structName}'. Error?");
        Log.Verbose($"{nameof(StructFieldNode)} || Field '{_structName}' node consumed.");
    }

    private static bool AtItemElement(XmlReader reader)
        => reader.Name == WriterConstants.ItemTag
           && reader.GetAttribute(WriterConstants.ItemIdAttr) != null;
}