using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Xml;
using UE.Toolkit.Core.Types;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Reloaded.ObjectWriters.Nodes;

public class DataTableFieldNode(string fieldName, nint fieldPtr, Type fieldType, FieldNodeFactory nodeFactory) : IFieldNode
{
    public unsafe void ConsumeNode(XmlReader reader)
    {
        // DataTable item type.
        if (!TryGetRowType(reader, out var itemType)) return;
        var itemSize = Marshal.SizeOf(itemType);
        
        // DataTable map type is always FName + Pointer, can just use UObject.
        // var tempTable = new ToolkitDataTable<UObjectBase>((UDataTable<UObjectBase>*)fieldPtr); 
        var tempTable = new UDataTableManaged<Ptr<byte>>((UDataTable<Ptr<byte>>*)fieldPtr, nodeFactory.Memory);
        
        // Get any item nodes.
        using var subReader = reader.ReadSubtree();
        subReader.MoveToContent();
        
        while (subReader.Read())
        {
            if (subReader.NodeType != XmlNodeType.Element) continue;
            if (subReader.Name != WriterConstants.ItemTag) throw new($"Only '{WriterConstants.ItemTag}' elements can be directly inside a UDataTable. Found: {subReader.Name}");
            
            var id = subReader.GetAttribute(WriterConstants.ItemIdAttr);
            if (id == null)
            {
                Log.Error($"{nameof(DataTableFieldNode)} || '{WriterConstants.ItemTag}' in field '{fieldName}' is missing an ID.");
                break;
            }

            var Key = new FName(id);
            if (!tempTable.ContainsKey(Key))
            {
                tempTable.AddRow(Key, new Ptr<byte>((byte*)nodeFactory.Memory.MallocZeroed(itemSize)));
                Log.Debug($"{nameof(DataTableFieldNode)} || Added row with ID '{id}' into '{fieldName}'.");   
            }
            
            if (tempTable.TryGetValue(Key, out var row)
                && nodeFactory.TryCreate($"{fieldName} (ID: {id})", (nint)row.Value->Value, 0, itemType, out var itemNode))
            {
                var itemTree = subReader.ReadSubtree();
                itemTree.MoveToContent();
                itemNode.ConsumeNode(itemTree);
            }
        }
        
        Log.Verbose($"{nameof(DataTableFieldNode)} || Field '{fieldName}' node consumed.");
    }

    private bool TryGetRowType(XmlReader reader, [NotNullWhen(true)] out Type? rowType)
    {
        // DataTable is the property of an object with generic type.
        if (fieldType.IsGenericType)
        {
            rowType = fieldType.GetGenericArguments().FirstOrDefault();
        }
        
        // DataTable root object, type needs to be specified as attribute.
        else
        {
            var rowStruct = reader.GetAttribute(WriterConstants.RowStructAttr);
            if (rowStruct == null)
            {
                rowType = null;
                Log.Error($"{nameof(DataTableFieldNode)} || Root '{nameof(UDataTable<byte>)}' missing '{WriterConstants.RowStructAttr}' attribute.");
                return false;
            }

            var rowStructHint = reader.GetAttribute(WriterConstants.RowStructHintAttr);
            var rowStructProvider = reader.GetAttribute(WriterConstants.RowStructProviderAttr);
            nodeFactory.TypeRegistry.TryGetType(rowStruct, rowStructHint, rowStructProvider, out rowType);
        }

        return rowType != null;
    }
}