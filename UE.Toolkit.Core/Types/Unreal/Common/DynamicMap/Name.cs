using System.Diagnostics.CodeAnalysis;
using UE.Toolkit.Core.Types.Unreal.Factories;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Core.Types.Unreal.Common.DynamicMap;

public class NameDynamicMapKeyType(IFMapProperty property, IUnrealFactory factory) 
    : BaseDynamicMapKeyType(property, factory)
{
    public override bool FromString(string text, [NotNullWhen(true)] out IDynamicMapKey? key)
    {
        key = new NameDynamicMapKey(new(text));
        return true;
    }
    
    public override unsafe IDynamicMapKey FromPtr(nint ptr) => new NameDynamicMapKey(*(FName*)ptr);
    
    public override int DynSizeOf() => 8;
}

public class NameDynamicMapKey(FName value)
    : BaseDynamicMapKey<FName>(value)
{
    public override unsafe void Write(nint ptr) => *(FName*)ptr = Value;
}