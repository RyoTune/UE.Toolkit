using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using UE.Toolkit.Core.Types.Unreal.Factories;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Core.Types.Unreal.Common.DynamicMap;

public interface IDynamicMapSizedType
{
    int DynSizeOf();
}

public interface IDynamicMapValueType : IDynamicMapSizedType;

public class DynamicMapValueSystemType(Type type) : IDynamicMapValueType
{
    private Type Type => type;
    
    public int DynSizeOf() => Marshal.SizeOf(type);
}

public class DynamicMapValueUnrealProperty(IFProperty type) : IDynamicMapValueType
{
    private IFProperty Type => type;

    public int DynSizeOf() => Type.ElementSize;
}

public interface IDynamicMapKeyType : IDynamicMapSizedType
{
    bool FromString(string text, [NotNullWhen(true)] out IDynamicMapKey? key);
    IDynamicMapKey FromPtr(nint ptr);
}

public abstract class BaseDynamicMapKeyType(IFMapProperty property, IUnrealFactory factory) : IDynamicMapKeyType
{
    protected IUnrealFactory Factory => factory;
    protected IFMapProperty Property => property;

    public abstract bool FromString(string text, [NotNullWhen(true)] out IDynamicMapKey? key);
    public abstract IDynamicMapKey FromPtr(nint ptr);
    public abstract int DynSizeOf();
}

public interface IDynamicMapKey
{
    void Write(nint ptr);
    uint GetTypeHash();
}

public abstract class BaseDynamicMapKey<TInner>(TInner value)
    : IDynamicMapKey, IMapHashable where TInner : IMapHashable
{
    protected TInner Value { get; set; } = value;
    public uint GetTypeHash() => Value!.GetTypeHash();
    public abstract void Write(nint ptr);
    public override string ToString() => Value.ToString();

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Value.Equals(((BaseDynamicMapKey<TInner>)obj).Value);
    }
}