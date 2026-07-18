using UE.Toolkit.Core.Types;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;

namespace UE.Toolkit.Reloaded.ObjectWriters.Nodes;

public class Int64Node(IFProperty property, Ptr<long> value) : ValuePrimitiveNode<IFProperty, long>(property, value)
{
    protected override unsafe void SetField(string text) => *Value.Value = Convert.ToInt64(text);
}