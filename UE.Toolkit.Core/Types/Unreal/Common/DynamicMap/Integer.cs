using System.Diagnostics.CodeAnalysis;
using UE.Toolkit.Core.Types.Unreal.Factories;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Core.Types.Unreal.Common.DynamicMap;

public struct HashableByte(byte value) : IMapHashable, IEquatable<HashableByte>
{
    public byte Value = value;
    public uint GetTypeHash() => Value;
    public bool Equals(HashableByte other) => other.Value == Value;
    public override string ToString() => Value.ToString();
}

public struct HashableShort(short value) : IMapHashable, IEquatable<HashableShort>
{
    public short Value = value;
    public uint GetTypeHash() => (uint)Value;
    public bool Equals(HashableShort other) => other.Value == Value;
    public override string ToString() => Value.ToString();
}

public struct HashableLong(long value) : IMapHashable, IEquatable<HashableLong>
{
    public long Value = value;
    public uint GetTypeHash() => (uint)(Value + (Value >> 32 * 23));
    public bool Equals(HashableLong other) => other.Value == Value;
    public override string ToString() => Value.ToString();
}

public class Int8DynamicMapKeyType(IFMapProperty property, IUnrealFactory factory) 
    : BaseDynamicMapKeyType(property, factory)
{
    public override int DynSizeOf() => (int)Factory.GetAlignment(Property.ValueProp);

    public override bool FromString(string text, [NotNullWhen(true)] out IDynamicMapKey? key)
    {
        key = null;
        if (!byte.TryParse(text, out var value))
        {
            return false;
        }
        key = new Int8DynamicMapKey(new(value));
        return true;   
    }

    public override unsafe IDynamicMapKey FromPtr(nint ptr) => new Int8DynamicMapKey(new(*(byte*)ptr));
}

public class Int8DynamicMapKey(HashableByte value)
    : BaseDynamicMapKey<HashableByte>(value)
{
    public override unsafe void Write(nint ptr) => *(byte*)ptr = Value.Value;
}

public class Int16DynamicMapKeyType(IFMapProperty property, IUnrealFactory factory) 
    : BaseDynamicMapKeyType(property, factory)
{
    public override int DynSizeOf() => int.Max((short)Factory.GetAlignment(Property.ValueProp), 2);

    public override bool FromString(string text, [NotNullWhen(true)] out IDynamicMapKey? key)
    {
        key = null;
        if (!short.TryParse(text, out var value))
        {
            return false;
        }
        key = new Int16DynamicMapKey(new(value));
        return true;   
    }

    public override unsafe IDynamicMapKey FromPtr(nint ptr) => new Int16DynamicMapKey(new(*(short*)ptr));
}

public class Int16DynamicMapKey(HashableShort value)
    : BaseDynamicMapKey<HashableShort>(value)
{
    public override unsafe void Write(nint ptr) => *(short*)ptr = Value.Value;
}

public class IntDynamicMapKeyType(IFMapProperty property, IUnrealFactory factory) 
    : BaseDynamicMapKeyType(property, factory)
{
    public override int DynSizeOf() => int.Max((int)Factory.GetAlignment(Property.ValueProp), 4);

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

public class Int64DynamicMapKeyType(IFMapProperty property, IUnrealFactory factory) 
    : BaseDynamicMapKeyType(property, factory)
{
    public override int DynSizeOf() => 8;

    public override bool FromString(string text, [NotNullWhen(true)] out IDynamicMapKey? key)
    {
        key = null;
        if (!long.TryParse(text, out var value))
        {
            return false;
        }
        key = new Int64DynamicMapKey(new(value));
        return true;   
    }

    public override unsafe IDynamicMapKey FromPtr(nint ptr) => new Int64DynamicMapKey(new(*(long*)ptr));
}

public class Int64DynamicMapKey(HashableLong value)
    : BaseDynamicMapKey<HashableLong>(value)
{
    public override unsafe void Write(nint ptr) => *(long*)ptr = Value.Value;
}