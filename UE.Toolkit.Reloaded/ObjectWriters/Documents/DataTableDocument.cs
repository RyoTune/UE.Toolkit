using System.Diagnostics.CodeAnalysis;
using System.Xml;
using UE.Toolkit.Core.Types;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Reloaded.ObjectWriters.Nodes;

public class DataTableDocument(string tableName, nint baseAddress, NodeFactory factory) : IFieldNode
{
    
    private string TableName => tableName;
    private nint BaseAddress => baseAddress;
    private NodeFactory Factory => factory;

    public unsafe void ConsumeNode(XmlReader reader)
    {
        // DataTable map type is always FName + Pointer, can just use UObject.
        var tempTable = new UDataTableManaged<Ptr<byte>>((UDataTable<Ptr<byte>>*)BaseAddress, Factory.Memory);
        
        // DataTable item type.
        if (!TryGetRowType(reader, tempTable, out var rowStruct)) return;
        Log.Verbose($"{nameof(DataTableDocument)} || Row Struct is '{rowStruct.NamePrivate}'.");
        
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
                Log.Error($"{nameof(DataTableDocument)} || '{WriterConstants.ItemTag}' in field '{TableName}' is missing an ID.");
                break;
            }

            var Key = new FName(id);
            if (!tempTable.ContainsKey(Key))
            {
                tempTable.AddRow(Key, new Ptr<byte>((byte*)Factory.Memory.MallocZeroed(rowStruct.PropertiesSize)));
                Log.Debug($"{nameof(DataTableDocument)} || Added row with ID '{id}' into '{TableName}'.");   
            }
            var itemTree = subReader.ReadSubtree();
            itemTree.MoveToContent();
            new StructDocument($"{TableName} (ID: {id})", rowStruct, (nint)tempTable[Key].Value->Value, Factory)
                .ConsumeNode(itemTree);
        }
        
        Log.Verbose($"{nameof(DataTableDocument)} || Field '{TableName}' node consumed.");
    }

    private bool TryGetRowType(XmlReader reader, UDataTableManaged<Ptr<byte>> dataTable, 
        [NotNullWhen(true)] out IUStruct? rowStruct)
    {
        // DataTable root object
        var rowStructName = reader.GetAttribute(WriterConstants.RowStructAttr) ?? dataTable.RowStructName;
        rowStruct = Factory.Classes.GetScriptStructInfoFromName($"F{rowStructName}", out var rowStructNew) ? rowStructNew : null;
        if (rowStruct == null)
        {
            Log.Error($"{nameof(DataTableDocument)} || Data Table row type '{rowStructName}' does not exist.");
        }
        return rowStruct != null;   
    }
}