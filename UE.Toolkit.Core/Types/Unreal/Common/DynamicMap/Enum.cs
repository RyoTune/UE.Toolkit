using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using UE.Toolkit.Core.Types.Unreal.Factories;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;

namespace UE.Toolkit.Core.Types.Unreal.Common.DynamicMap;

public static class ToolkitFormatProvider
{
    public static IFormatProvider FloatProvider = new CultureInfo("en-US");
}

public class EnumDynamicMapKeyType(IFMapProperty property, IUnrealFactory factory, IFEnumProperty key)
    : BaseDynamicMapKeyType(property, factory)
{
    private IFEnumProperty Key => key;
    
    public override int DynSizeOf() => int.Max((short)Factory.GetAlignment(Property.ValueProp), Key.ElementSize);

    public override unsafe IDynamicMapKey FromPtr(nint ptr)
    {
        return new EnumDynamicMapKey(Key.ElementSize switch
        {
            1 => *(byte*)ptr,
            2 => *(short*)ptr,
            4 => *(int*)ptr,
            _ => *(long*)ptr,
        }, Key);
    }
    
    public override bool FromString(string text, [NotNullWhen(true)] out IDynamicMapKey? key)
    {
        key = null;
        if (Key.Enum == null || Key.Enum.Ptr == nint.Zero) return false;
        // Value is enum member name.
        if (Key.Enum.TryParse(text, true, out var value))
        {
            key = new EnumDynamicMapKey(value.Value, Key);
            return true;
        }
        if (double.TryParse(text, ToolkitFormatProvider.FloatProvider, out var intValue))
        {
            key = new EnumDynamicMapKey((long)intValue, Key);
            return true;
        }
        return false;
    }
}

public class EnumDynamicMapKey(long value, IFEnumProperty property)
    : IDynamicMapKey
{
    private IFEnumProperty Property => property;

    private long Value => value;

    public unsafe void Write(nint ptr)
    {
        switch (Property.ElementSize)
        {
            case 1:
                *(byte*)ptr = (byte)Value;
                return;
            case 2: 
                *(short*)ptr = (short)Value;
                return;
            case 4:
                *(int*)ptr = (int)Value;
                return;
            default:
                *(long*)ptr = Value;
                return;
        };
    }

    public uint GetTypeHash()
    {
        return Property.ElementSize switch
        {
            1 => IntegerHashing.TypeHashForByte((byte)Value),
            2 => IntegerHashing.TypeHashForShort((short)Value),
            4 => IntegerHashing.TypeHashForInt((int)Value),
            _ => IntegerHashing.TypeHashForLong(Value),
        };
    }
    
    public override string ToString() => Value.ToString();
    
    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Value.Equals(((EnumDynamicMapKey)obj).Value);
    }
}