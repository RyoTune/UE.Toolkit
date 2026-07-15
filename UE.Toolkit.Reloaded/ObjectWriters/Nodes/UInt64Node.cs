using UE.Toolkit.Core.Types;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;

namespace UE.Toolkit.Reloaded.ObjectWriters.Nodes;

public class UInt64Node(IFProperty property, Ptr<ulong> value) : ValuePrimitiveNode<IFProperty, ulong>(property, value)
{
    protected override unsafe void SetField(string text) => *Value.Value = Convert.ToUInt64(text);
}