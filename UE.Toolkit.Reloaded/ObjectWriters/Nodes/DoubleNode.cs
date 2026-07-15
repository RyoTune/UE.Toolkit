using UE.Toolkit.Core.Types;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;

namespace UE.Toolkit.Reloaded.ObjectWriters.Nodes;

public class DoubleNode(IFProperty property, Ptr<double> value) : ValuePrimitiveNode<IFProperty, double>(property, value)
{
    protected override unsafe void SetField(string text) => *Value.Value = Convert.ToDouble(text, ObjectXMLFormatProvider.FloatProvider);
}