using System.Globalization;

namespace UE.Toolkit.Reloaded.ObjectWriters.Writers;

public unsafe class PrimitiveFieldWriter(string fieldName, nint fieldPtr, int fieldBit, Type fieldType) : IFieldWriter
{
    private static IFormatProvider FloatProvider = new CultureInfo("en-US");
    
    private nint? _ogValue;
    private Type _fieldType = fieldType;
    private int _fieldBit = fieldBit;

    public void Reset()
    {
        if (_ogValue != null) SetField((nint)_ogValue);
    }

    public void SetField(string value)
    {
        // Handle enum values either by name or integer.
        if (_fieldType.IsEnum)
        {
            var enumType = _fieldType;
            _fieldType = Enum.GetUnderlyingType(enumType);
            
            // Value is enum member name.
            if (Enum.TryParse(enumType, value, true, out var nameValue))
            {
                SetField(Convert.ToDouble(nameValue, FloatProvider));
            }

            // Value is enum integer value.
            else if (double.TryParse(value, FloatProvider, out var intValue))
            {
                SetField(intValue);
            }
        }
        else if (bool.TryParse(value, out var boolValue))
        {
            SetField(boolValue);
        }
        else if (double.TryParse(value, FloatProvider, out var numValue))
        {
            SetField(numValue);
        }
        else
        {
            Log.Error($"{nameof(PrimitiveFieldWriter)} || Invalid value '{value}' for field '{fieldName}'.");
        }
    }

    public void SetField<TValue>(TValue value)
        where TValue : unmanaged
    {
        _ogValue ??= *(nint*)fieldPtr;
        
        switch (_fieldType.Name)
        {
            case "Int64":
                *(nint*)fieldPtr = (nint)Convert.ToInt64(value);
                break;
            case "UInt64":
                *(nint*)fieldPtr = (nint)Convert.ToUInt64(value);
                break;
            case "Int32":
                *(int*)fieldPtr = Convert.ToInt32(value);
                break;
            case "UInt32":
                *(uint*)fieldPtr = Convert.ToUInt32(value);
                break;
            case "Int16":
                *(short*)fieldPtr = Convert.ToInt16(value);
                break;
            case "UInt16":
                *(ushort*)fieldPtr = Convert.ToUInt16(value);
                break;
            case "Byte":
                *(byte*)fieldPtr = Convert.ToByte(value);
                break;
            case "SByte":
                *(sbyte*)fieldPtr = Convert.ToSByte(value);
                break;
            case "Boolean":
                var Value = Convert.ToBoolean(value);
                *(byte*)fieldPtr |= (byte)(*(byte*)&Value << _fieldBit);
                break;
            case "Single":
                *(float*)fieldPtr = Convert.ToSingle(value, FloatProvider);
                break;
            case "Double":
                *(double*)fieldPtr = Convert.ToDouble(value, FloatProvider);
                break;
            default:
                Log.Error($"{nameof(PrimitiveFieldWriter)} || Invalid type '{_fieldType.Name}' for field '{fieldName}'.");
                return;
        }
        
        Log.Debug($"{nameof(PrimitiveFieldWriter)} || Field '{fieldName}' at 0x{fieldPtr:X} set to: {value}");
    }
}