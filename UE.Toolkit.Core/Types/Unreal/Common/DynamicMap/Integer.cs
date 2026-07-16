using System.Diagnostics.CodeAnalysis;
using UE.Toolkit.Core.Types.Unreal.Factories;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Core.Types.Unreal.Common.DynamicMap;

public class IntDynamicMapKeyType(IFMapProperty property, IUnrealFactory factory) 
    : BaseDynamicMapKeyType(property, factory)
{
    public override int DynSizeOf() => Factory.GetAlignment(Property.ValueProp) switch
    {
        <= 4 => 4,
        _ => 8
    };

    public override bool FromString(string text, [NotNullWhen(true)] out IDynamicMapKey? key)
    {
        key = null;
        if (!int.TryParse(text, out var value))
        {
            return false;
        }
        key = new IntDynamicMapKey(new(value));
        return true;   
    }

    public override unsafe IDynamicMapKey FromPtr(nint ptr) => new IntDynamicMapKey(new(*(int*)ptr));
}

public class IntDynamicMapKey(HashableInt value)
    : BaseDynamicMapKey<HashableInt>(value)
{
    public override unsafe void Write(nint ptr) => *(int*)ptr = Value.Value;
}