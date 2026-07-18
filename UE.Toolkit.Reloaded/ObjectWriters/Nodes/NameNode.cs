using UE.Toolkit.Core.Types;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Reloaded.ObjectWriters.Nodes;

public class NameNode(IFProperty property, Ptr<FName> value) : ValuePrimitiveNode<IFProperty, FName>(property, value)
{
    protected override unsafe void SetField(string text) => *Value.Value = new(text);
}