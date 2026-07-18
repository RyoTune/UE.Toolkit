using System.Xml;
using UE.Toolkit.Core.Types;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;

namespace UE.Toolkit.Reloaded.ObjectWriters.Nodes;

public static class EnumWriter
{
    public static void WriteEnum(IUEnum Enum, Ptr<nint> Value, string text)
    {
        // Value is enum member name.
        long enumValue;
        if (Enum.TryParse(text, true, out var value))
        {
            enumValue = value.Value;
        } else if (double.TryParse(text, ObjectXMLFormatProvider.FloatProvider, out var intValue))
        {
            enumValue = (long)intValue;
        }
        else
        {
            Log.Error($"{nameof(EnumNode)} || Enum {Enum.NamePrivate} does not contain the key '{text}'");
            return;
        }
        // Value is enum integer value.
        long maxValue = 0;
        unsafe
        {
            for (var i = 0; i < Enum.Names.ArrayNum; i++)
            {
                var currentValue = Enum.Names.AllocatorInstance[i].Value;
                if (currentValue > maxValue)
                    maxValue = currentValue;
            }   
            if (maxValue <= byte.MaxValue)
            {
                *(byte*)Value.Value = (byte)enumValue;
            }
            else if (maxValue <= ushort.MaxValue)
            {
                *(ushort*)Value.Value = (ushort)enumValue;
            }
            else if (maxValue <= uint.MaxValue)
            {
                *(uint*)Value.Value = (uint)enumValue;
            }
            else
            {
                *(ulong*)Value.Value = (ulong)enumValue;
            }   
        }
    }
}

public class EnumNode(IFEnumProperty property, Ptr<nint> value) : ValuePrimitiveNode<IFEnumProperty, nint>(property, value)
{
    protected override void SetField(string text) => EnumWriter.WriteEnum(property.Enum, Value, text);
}