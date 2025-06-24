using System.Runtime.InteropServices;

namespace UE.Toolkit.Core.Types.Unreal.UE5_4_4;

[StructLayout(LayoutKind.Sequential, Size = 16)]
public struct FText
{
    public nint TextData;
    public uint Flags;
}