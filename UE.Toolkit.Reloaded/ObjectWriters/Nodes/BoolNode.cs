using UE.Toolkit.Core.Types;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;

namespace UE.Toolkit.Reloaded.ObjectWriters.Nodes;

public class BoolNode(IFBoolProperty property, Ptr<byte> value) : ValuePrimitiveNode<IFBoolProperty, byte>(property, value)
{
    protected override unsafe void SetField(string text)
    {
        var Bool = Convert.ToBoolean(text);
        *Value.Value |= (byte)(property.FieldMask * *(byte*)&Bool);
    }
}