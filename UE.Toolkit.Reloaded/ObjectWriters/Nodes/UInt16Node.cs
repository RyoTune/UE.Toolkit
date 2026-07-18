using UE.Toolkit.Core.Types;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;

namespace UE.Toolkit.Reloaded.ObjectWriters.Nodes;

public class UInt16Node(IFProperty property, Ptr<ushort> value) : ValuePrimitiveNode<IFProperty, ushort>(property, value)
{
    protected override unsafe void SetField(string text) => *Value.Value = Convert.ToUInt16(text);
}