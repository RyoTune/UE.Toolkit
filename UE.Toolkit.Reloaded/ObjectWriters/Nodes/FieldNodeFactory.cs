using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using UE.Toolkit.Core.Types.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;
using UE.Toolkit.Interfaces.ObjectWriters;
using UE.Toolkit.Reloaded.Common.GameConfigs;

namespace UE.Toolkit.Reloaded.ObjectWriters.Nodes;

public class FieldNodeFactory(ITypeRegistry typeReg, IObjectCreator objCreator, IUnrealMemoryInternal memory)
{
    public ITypeRegistry TypeRegistry { get; } = typeReg;
    public IUnrealMemoryInternal Memory { get; } = memory;
    
    public bool TryCreate(string fieldName, nint fieldPtr, int fieldBit, Type fieldType, [NotNullWhen(true)]out IFieldNode? node)
    {
        node = Create(fieldName, fieldPtr, fieldBit, fieldType);
        return node != null;
    }

    private IFieldNode? Create(string fieldName, nint fieldPtr, int fieldBit, Type fieldType)
    {
        
        Log.Debug($"{nameof(FieldNodeFactory)} || Create Node '{fieldName}' with type '{fieldType.Name}'.");
        
        if (fieldType.IsPrimitive
            || fieldType.IsEnum
            || fieldType == typeof(string)
            || fieldType == GameConfig.GetFText()
            || fieldType == typeof(FString)
            || fieldType == typeof(FName)
            || fieldType.Name.StartsWith("TSoftObjectPtr")
            || fieldType.Name.StartsWith("TSoftClassPtr")
            )
        {
            return new PrimitiveFieldNode(fieldName, fieldPtr, fieldBit, fieldType, objCreator);
        }

        if (fieldType.Name.StartsWith("TArray"))
        {
            return new TArrayFieldNode(fieldName, fieldPtr, fieldType, this);
        }

        if (fieldType.Name.StartsWith("UDataTable"))
        {
            return new DataTableFieldNode(fieldName, fieldPtr, fieldType, this);
        } 
        
        if (fieldType.Name.StartsWith("TMap"))
        {
            var keyType = fieldType.GetGenericArguments()[0];
            if (keyType == typeof(int))
            {
                var valueType = fieldType.GetGenericArguments()[1];
                // Check if it has a StructLayout with an explicit alignment value, this lets us avoid iterating
                // through each field to determine alignment
                // Every type defined in the extension mod has an alignment value in it's StructLayout
                if (valueType.StructLayoutAttribute != null)
                {
                    return valueType.StructLayoutAttribute!.Pack switch
                    {
                         <= 4 => new TMapIntFieldNode(fieldName, fieldPtr, fieldType, this),
                         _ => new TMapInt8FieldNode(fieldName, fieldPtr, fieldType, this)
                    };
                }
                // Fallback: Iterate through fields (this should not be necessary)
                var LastOffset = 0;
                var bUseInt8 = false;
                foreach (var Field in valueType.GetFields())
                {
                    // Check that the size for each field doesn't exceed 4 bytes
                    if (Marshal.SizeOf(Field.GetType()) > 4)
                    {
                        bUseInt8 = true;
                        break;
                    }
                }
                return bUseInt8 switch
                {
                    true => new TMapInt8FieldNode(fieldName, fieldPtr, fieldType, this),
                    false => new TMapIntFieldNode(fieldName, fieldPtr, fieldType, this),
                };
            }

            if (keyType == typeof(FName))
            {
                return new TMapNameFieldNode(fieldName, fieldPtr, fieldType, this);
            }

            /*
            if (keyType.Name.StartsWith("Ptr")) // TODO
            {
                
            }
            */
            Log.Warning($"{nameof(FieldNodeFactory)} || Field '{fieldName}' with type '{fieldType.Name}' is not currently supported for map editing operations.");
            return null;
        }

        if (fieldType.IsValueType)
        {
            return new StructFieldNode(fieldName, fieldPtr, fieldType, this);
        }

        Log.Error($"{nameof(FieldNodeFactory)} || Unsupported field '{fieldName}' with type '{fieldType.Name}'.");
        return null;
    }
}