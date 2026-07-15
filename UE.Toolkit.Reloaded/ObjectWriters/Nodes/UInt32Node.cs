using UE.Toolkit.Core.Types;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;

namespace UE.Toolkit.Reloaded.ObjectWriters.Nodes;

public class UInt32Node(IFProperty property, Ptr<uint> value) : ValuePrimitiveNode<IFProperty, uint>(property, value)
{
    protected override unsafe void SetField(string text) => *Value.Value = Convert.ToUInt32(text);
}