using System.Runtime.InteropServices;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Core.Types.Unreal.UE5_0_3;

[StructLayout(LayoutKind.Explicit, Size = 0x2c8)]
public unsafe struct FWorldContext
{
    [FieldOffset(0x0)] public WorldType WorldType;
    [FieldOffset(0x270)] public UWorld* ThisCurrentWorld;
}