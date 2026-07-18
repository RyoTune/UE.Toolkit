using System.Diagnostics.CodeAnalysis;
using UE.Toolkit.Core.Types.Interfaces;
using UE.Toolkit.Core.Types.Unreal.Common.DynamicMap;
using UE.Toolkit.Core.Types.Unreal.Factories;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;
using UE.Toolkit.Interfaces;

namespace UE.Toolkit.Reloaded.Common.DynamicMap;

public class StringDynamicMapKeyType(IFMapProperty property, IUnrealFactory factory, 
    IUnrealObjects objects, IUnrealMemoryInternal memory)
    : BaseDynamicMapKeyType(property, factory)
{
    private IUnrealObjects Objects => objects;
    private IUnrealMemoryInternal Memory => memory;
    
    public override unsafe bool FromString(string text, [NotNullWhen(true)] out IDynamicMapKey? key)
    {
        key = new StringDynamicMapKey(*Objects.CreateFString(text), Memory);
        return true;
    }
    
    public override unsafe IDynamicMapKey FromPtr(nint ptr) => new StringDynamicMapKey(*(FString*)ptr, Memory);
    
    public override int DynSizeOf() => 8;
}

public class StringDynamicMapKey(FString value, IUnrealMemoryInternal memory)
    : BaseDynamicMapKey<FString>(value)
{
    private IUnrealMemoryInternal Memory => memory;
    
    public override unsafe void Write(nint ptr)
    {
        var str = (FString*)ptr;
        if (str->Data.AllocatorInstance != null)
        {
            Memory.Free((nint)str->Data.AllocatorInstance);
        }
        *(FString*)ptr = Value;
    }
}