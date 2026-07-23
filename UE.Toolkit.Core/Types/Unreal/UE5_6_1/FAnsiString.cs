using System.Runtime.InteropServices;
using System.Text;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Core.Types.Unreal.UE5_6_1;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct FAnsiString : IMapHashable
{
    public TArray<char> Data;
    
    public override string ToString()
        => Data.ArrayNum > 0 ? Marshal.PtrToStringAnsi((nint)Data.AllocatorInstance, Data.ArrayNum - 1) : string.Empty;
    
    public uint GetTypeHash()
    {
        var Bytes = Encoding.ASCII.GetBytes(ToString());
        uint Hash = 0;
        foreach (var Byte in Bytes)
            Hash = ((Hash >> 8) & 0xFFFFFF) ^ FString.CRC_HASH[(Hash ^ Byte) & 0xFF];
        return Hash;
    }
}