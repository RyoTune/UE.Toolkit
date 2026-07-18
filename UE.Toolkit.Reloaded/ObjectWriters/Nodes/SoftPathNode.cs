using System.Runtime.InteropServices;
using System.Xml;
using UE.Toolkit.Core.Types;
using UE.Toolkit.Core.Types.Unreal.Common;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Reloaded.ObjectWriters.Nodes;

public class SoftPathNode(IFProperty property, ISoftObjectPath value) : IFieldNode
{
    private ISoftObjectPath Value => value;
    
    private byte[]? InitialValue { get; set; }
    
    private string GetFieldValue(XmlReader reader) 
        => reader.GetAttribute("value") ?? reader.ReadElementContentAsString();
    
    public void ConsumeNode(XmlReader reader)
    {
        var fieldValue = GetFieldValue(reader);
        if (InitialValue != null)
        {
            InitialValue = new byte[Value.GetSizeOf()];
            Marshal.Copy(Value.Ptr, InitialValue, 0, Value.GetSizeOf());
        }
        Value.SetAssetPath(fieldValue);
        Log.Verbose($"{nameof(SoftPathNode)}<{Value.GetType().FullName}> || Field '{property.NamePrivate}' at 0x{Value.Ptr:X} set to '{fieldValue}'");
    }
}