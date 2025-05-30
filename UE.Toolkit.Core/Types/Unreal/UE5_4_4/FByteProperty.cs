using System.Runtime.InteropServices;

namespace UE.Toolkit.Core.Types.Unreal.UE5_4_4;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct FByteProperty
{
    public FProperty Super;
    public UEnum* Enum;
}

[StructLayout(LayoutKind.Sequential)]
public struct FNumericProperty
{
    public FProperty Super;
}