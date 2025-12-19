using System.Runtime.InteropServices;
using UE.Toolkit.Core.Types.Interfaces;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Reloaded.ObjectWriters.Writers;

public unsafe class PtrFieldWriter(string fieldName, nint fieldPtr, Type fieldType, IObjectCreator objCreator) : IFieldWriter
{

    private byte[]? _ogData;
    
    public void Reset()
    {
        if (_ogData != null) Marshal.Copy(_ogData, 0, fieldPtr, _ogData.Length);
    }

    public void SetField(string value)
    {
        var strPtr = Marshal.StringToHGlobalUni(value);
        SetField(strPtr);
        Marshal.FreeHGlobal(strPtr);
    }

    public void SetField<TValue>(TValue value) where TValue : unmanaged
    {
        var strValue = Marshal.PtrToStringUni(*(nint*)&value)!;
        if (fieldType.Name.StartsWith("TSoftObjectPtr") || fieldType.Name.StartsWith("TSoftClassPtr"))
        {
            var SizeOf = Marshal.SizeOf<FSoftObjectPtr>();
            _ogData = new byte[SizeOf];
            Marshal.Copy(fieldPtr, _ogData, 0, SizeOf);
            var ObjectPtr = (TSoftObjectPtr<byte>*)fieldPtr;
            ObjectPtr->SoftObjectPtr.Super.ObjectId.AssetPath.PackageName = new FName(strValue);
            Log.Debug($"{nameof(PtrFieldWriter)} || Field '{fieldName}' at 0x{fieldPtr:X} set to: {strValue}");
        }
        else
        {
            Log.Error($"{nameof(PtrFieldWriter)} || Invalid type '{fieldType.Name}' for field '{fieldName}'.");
        }
    }
}