using UE.Toolkit.Core.Types;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;

namespace UE.Toolkit.Reloaded.ObjectWriters.Nodes;

public class Int8Node(IFProperty property, Ptr<byte> value) : ValuePrimitiveNode<IFProperty, byte>(property, value)
{
    protected override unsafe void SetField(string text) => *Value.Value = Convert.ToByte(text);
}