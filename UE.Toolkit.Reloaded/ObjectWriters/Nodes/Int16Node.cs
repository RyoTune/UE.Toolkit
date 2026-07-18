using UE.Toolkit.Core.Types;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;

namespace UE.Toolkit.Reloaded.ObjectWriters.Nodes;

public class Int16Node(IFProperty property, Ptr<short> value) : ValuePrimitiveNode<IFProperty, short>(property, value)
{
    protected override unsafe void SetField(string text) => *Value.Value = Convert.ToInt16(text);
}