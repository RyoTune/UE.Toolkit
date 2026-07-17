using System.Runtime.InteropServices;
using System.Xml;
using UE.Toolkit.Core.Types;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Reloaded.Common.DynamicMap;

namespace UE.Toolkit.Reloaded.ObjectWriters.Nodes;

public class GuidNode(IFProperty property, Ptr<Guid> value) : IFieldNode
{
    private Ptr<Guid> Value => value;
    
    private byte[]? InitialValue { get; set; }
    
    private string GetFieldValue(XmlReader reader) 
        => reader.GetAttribute("value") ?? reader.ReadElementContentAsString();
    
    public void ConsumeNode(XmlReader reader)
    {
        var fieldValue = GetFieldValue(reader);
        if (InitialValue != null)
        {
            unsafe
            {
                InitialValue = new byte[sizeof(Guid)];
                Marshal.Copy((nint)Value.Value, InitialValue, 0, sizeof(Guid));
            }
        }

        if (!Guid.TryParse(fieldValue, out var guid))
        {
            Log.Warning($"{nameof(GuidNode)} || Field '{property.NamePrivate}' has an incorrectly formatted GUID! '{fieldValue}'");
            return;
        }

        unsafe
        {
            *Value.Value = guid;
            Log.Verbose($"{nameof(GuidNode)} || Field '{property.NamePrivate}' at 0x{(nint)Value.Value:X} set to '{fieldValue}'"); 
        }
    }
}