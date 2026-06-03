using System.Runtime.InteropServices;

namespace UE.Toolkit.Core.Types.Unreal.UE4_27_2;

[StructLayout(LayoutKind.Sequential, Size = 24)]
public struct FText
{
    public nint TextData;
    public nint TextData2;
    public uint Flags;
}