using UE.Toolkit.Core.Types;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;

namespace UE.Toolkit.Reloaded.ObjectWriters.Nodes;

public class Int32Node(IFProperty property, Ptr<int> value) : ValuePrimitiveNode<IFProperty, int>(property, value)
{
    protected override unsafe void SetField(string text) => *Value.Value = Convert.ToInt32(text);
}