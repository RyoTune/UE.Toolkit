using UE.Toolkit.Core.Types;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;

namespace UE.Toolkit.Reloaded.ObjectWriters.Nodes;

public class FloatNode(IFProperty property, Ptr<float> value) : ValuePrimitiveNode<IFProperty, float>(property, value)
{
    protected override unsafe void SetField(string text) => *Value.Value = Convert.ToSingle(text, ObjectXMLFormatProvider.FloatProvider);
}