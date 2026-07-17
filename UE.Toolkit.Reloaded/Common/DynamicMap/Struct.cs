using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using UE.Toolkit.Core.Types.Interfaces;
using UE.Toolkit.Core.Types.Unreal.Common.DynamicMap;
using UE.Toolkit.Core.Types.Unreal.Factories;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;
using UE.Toolkit.Interfaces;

namespace UE.Toolkit.Reloaded.Common.DynamicMap;

public interface IStructMapKeyImpl
{
    bool FromString(string text, IStructMapKeyImpl impl, [NotNullWhen(true)] out IDynamicMapKey? key);
    uint GetTypeHash(nint ptr);
}

[StructLayout(LayoutKind.Explicit, Size = 0x18)]
struct FKey_Private
{
    [FieldOffset(0x0)] public FName KeyName; // Size: 0x8   
}

public class FKeyMapKeyImpl : IStructMapKeyImpl
{
    public bool FromString(string text, IStructMapKeyImpl impl, [NotNullWhen(true)] out IDynamicMapKey? key)
    {
        var Bytes = new FKey_Private
        {
            KeyName = new(text)
        };
        unsafe
        {
            var AsBytes = new byte[sizeof(FKey_Private)];
            Marshal.Copy((nint)(&Bytes), AsBytes, 0, sizeof(FKey_Private));
            key = new StructDynamicMapKey(AsBytes, impl);
            return true;
        }
    }

    public unsafe uint GetTypeHash(nint ptr) => ((FKey_Private*)ptr)->KeyName.GetTypeHash();
}

[StructLayout(LayoutKind.Explicit, Size = 0x10)]
struct FGuid_Private
{
    [FieldOffset(0x0)] public int A; // Size: 0x4
    [FieldOffset(0x4)] public int B; // Size: 0x4
    [FieldOffset(0x8)] public int C; // Size: 0x4
    [FieldOffset(0xC)] public int D; // Size: 0x4
}

/*
public class FGuidMapKeyImpl : IStructMapKeyImpl
{
    public bool FromString(string text, IStructMapKeyImpl impl, [NotNullWhen(true)] out IDynamicMapKey? key)
    {
        throw new NotImplementedException();
    }

    public uint GetTypeHash(nint ptr)
    {
        throw new NotImplementedException();
    }
}
*/

public static class StructMapKeyRegistry
{
    public static Dictionary<string, IStructMapKeyImpl> Implementations = new()
    {
        { "Key", new FKeyMapKeyImpl() },
        // { "Guid", new FGuidMapKeyImpl() },
    };
}

public class StructDynamicMapKeyType : IDynamicMapKeyType
{
    private IFMapProperty Property;
    private IStructMapKeyImpl Impl;
    private IFStructProperty TypeProp;
    private IUnrealFactory Factory;
    private IUnrealObjects Objects;
    private IUnrealMemoryInternal Memory;

    private StructDynamicMapKeyType(IFMapProperty property, IStructMapKeyImpl impl,
        IFStructProperty typeProp, IUnrealFactory factory, IUnrealObjects objects, IUnrealMemoryInternal memory)
    {
        Property = property;
        Impl = impl;
        TypeProp = typeProp;
        Factory = factory;
        Objects = objects;
        Memory = memory;
    }

    public static StructDynamicMapKeyType? Create(IFMapProperty property, IFStructProperty typeProp, 
        IUnrealFactory factory, IUnrealObjects objects, IUnrealMemoryInternal memory)
    {
        if (!StructMapKeyRegistry.Implementations.TryGetValue(
                typeProp.Struct.NamePrivate.ToString(),
                out var impl))
        {
            return null;
        }
        return new StructDynamicMapKeyType(property, impl, typeProp, factory, objects, memory);
    }

    public int DynSizeOf() => TypeProp.ElementSize;

    public bool FromString(string text, [NotNullWhen(true)] out IDynamicMapKey? key)
        => Impl.FromString(text, Impl, out key);

    public IDynamicMapKey FromPtr(nint ptr)
    {
        var Value = new byte[DynSizeOf()];
        Marshal.Copy(ptr, Value, 0, Value.Length);
        return new StructDynamicMapKey(Value, Impl);
    }
}

public class StructDynamicMapKey(byte[] value, IStructMapKeyImpl impl) : IDynamicMapKey
{
    private byte[] Value => value;
    private IStructMapKeyImpl Impl => impl;
    
    public void Write(nint ptr) => Marshal.Copy(Value, 0, ptr, Value.Length);

    public unsafe uint GetTypeHash()
    {
        fixed (byte* pValue = Value)
        {
            return Impl.GetTypeHash((nint)pValue);
        }
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Value.Equals(((StructDynamicMapKey)obj).Value);
    }
}