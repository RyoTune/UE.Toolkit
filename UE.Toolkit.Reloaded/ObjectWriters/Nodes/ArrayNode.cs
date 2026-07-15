using System.Xml;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Reloaded.ObjectWriters.Nodes;

public class ArrayNode(IFArrayProperty property, nint value, NodeFactory factory) : IFieldNode
{
    private IFArrayProperty Property => property;
    private nint Value => value;
    private NodeFactory Factory => factory;
    
    public unsafe void ConsumeNode(XmlReader reader)
    {
        var itemType = Property.Inner;
        var itemSize = itemType.ElementSize;
        var tempArray = (TArray<byte>*)Value;
        
        // Get any item nodes.
        using var subReader = reader.ReadSubtree();
        subReader.MoveToContent();
        
        while (subReader.Read())
        {
            if (subReader.NodeType != XmlNodeType.Element) continue;
            if (subReader.Name != WriterConstants.ItemTag) throw new($"Only '{WriterConstants.ItemTag}' elements can be directly inside an array. Found: {subReader.Name}");
            
            var id = subReader.GetAttribute(WriterConstants.ItemIdAttr);
            if (id == null)
            {
                Log.Error($"{nameof(ArrayNode)} || '{WriterConstants.ItemTag}' is missing an ID.");
                break;
            }

            int itemIdx;
            if (id == "+")
            {
                itemIdx = -1;
            }
            else if (!int.TryParse(id, out itemIdx))
            {
                Log.Warning($"{nameof(ArrayNode)} || Invalid ID: {id}");
                break;
            }
            
            if (itemIdx != -1)
            {
                itemIdx -= 1; // We're doing 1 indexing because normal people can't handle 0...
                // (What did Ryo mean by this...)
                if (itemIdx < 0 || itemIdx > tempArray->ArrayNum)
                {
                    Log.Warning($"{nameof(ArrayNode)} || ID is either less than 1 or more than item count: {id} || Total: {tempArray->ArrayNum}");
                    break;
                }   
            } else
            {
                Log.Verbose($"{nameof(ArrayNode)} @ 0x{(nint)tempArray:x} || Old Size : {tempArray->ArrayNum} || Old Capacity: {tempArray->ArrayMax}");
                if (tempArray->ArrayNum == tempArray->ArrayMax)
                    TArrayListStatic.ResizeToStatic(tempArray, TArrayListStatic.CalculateNewArraySizeStatic(tempArray), itemSize, Factory.Memory);
                itemIdx = tempArray->ArrayNum;
                tempArray->ArrayNum++;
            }

            var itemPtr = itemIdx * itemSize + (nint)tempArray->AllocatorInstance;
            Factory.Create(itemType, itemPtr).ConsumeNode(subReader);
        }
        
        Log.Verbose($"{nameof(ArrayNode)} || Field '{property.NamePrivate}' node consumed.");
    }
}