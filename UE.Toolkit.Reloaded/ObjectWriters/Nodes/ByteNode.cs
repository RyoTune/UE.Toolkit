using UE.Toolkit.Core.Types;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;

namespace UE.Toolkit.Reloaded.ObjectWriters.Nodes;

public class ByteNode(IFByteProperty property, Ptr<byte> value) : ValuePrimitiveNode<IFByteProperty, byte>(property, value)
{
    protected override unsafe void SetField(string text)
    {
        if (Property.Enum != null)
        {
            EnumWriter.WriteEnum(property.Enum, new((nint*)Value.Value), text);    
        }
        else
        {
            *Value.Value = Convert.ToByte(text);
        }
    }
}