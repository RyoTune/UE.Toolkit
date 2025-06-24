using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;
using UE.Toolkit.Interfaces;

// ReSharper disable InconsistentNaming

namespace UE.Toolkit.Reloaded.Unreal;

public unsafe class UnrealStrings : IUnrealStrings
{
    private delegate FString* FText_ToString(FText* text);
    private readonly SHFunction<FText_ToString> _FText_ToString = new();

    private delegate byte FTextInspector_GetTableIdAndKey(FText* text, FName* tableId, FString* key);
    private readonly SHFunction<FTextInspector_GetTableIdAndKey> _FTextInspector_GetTableIdAndKey = new();
    
    private delegate FText* UEnum_GetDisplayNameTextByIndex(UUserDefinedEnum* userEnum, FText* outName, int index);
    private readonly SHFunction<UEnum_GetDisplayNameTextByIndex> _getDispNameTextByIdx = new();

    public bool FTextInspectorGetTableIdAndKey(FText* text, [NotNullWhen(true)]out string? tableId, [NotNullWhen(true)]out string? key)
    {
        var tableIdBuffer = (FName*)Marshal.AllocHGlobal(sizeof(FName));
        var keyBuffer = CreateFString();
        
        var result = _FTextInspector_GetTableIdAndKey.Wrapper(text, tableIdBuffer, keyBuffer) == 1;
        if (result)
        {
            tableId = tableIdBuffer->ToString();
            key = keyBuffer->ToString();
        }
        else
        {
            tableId = null;
            key = null;
        }

        Marshal.FreeHGlobal((nint)tableIdBuffer);
        UnrealMemory._FMemory.Free((nint)keyBuffer);
        return result;
    }
    
    public string UEnumGetDisplayNameTextByIndex(nint userEnum, int index)
    {
        var outText = (FText*)Marshal.AllocHGlobal(sizeof(FText));
        
        var dispNameFText =
            _getDispNameTextByIdx.Wrapper((UUserDefinedEnum*)userEnum, outText, index);
        var dispName = FTextToString(dispNameFText);
        
        Marshal.FreeHGlobal((nint)outText);
        return dispName;
    }

    public string FTextToString(FText* text) => _FText_ToString.Wrapper(text)->ToString();
    
    public FString* CreateFString(string? content = null)
    {
        content ??= string.Empty;
        
        var fstring = (FString*)UnrealMemory._FMemory.Malloc(sizeof(FString));
        
        var strBytes = Encoding.Unicode.GetBytes(content);
        fstring->Data.ArrayNum = strBytes.Length;
        fstring->Data.ArrayMax = strBytes.Length;
        fstring->Data.AllocatorInstance = (char*)UnrealMemory._FMemory.Malloc(strBytes.Length);
        Marshal.Copy(strBytes, 0, (nint)fstring->Data.AllocatorInstance, strBytes.Length);
        
        return fstring;
    }
}