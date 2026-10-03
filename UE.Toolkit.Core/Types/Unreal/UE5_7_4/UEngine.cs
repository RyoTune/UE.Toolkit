using System.Runtime.InteropServices;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Core.Types.Unreal.UE5_7_4;

[StructLayout(LayoutKind.Explicit, Size = 0x12c0)]
public struct UEngine
{
    [FieldOffset(0x11c0)] public TArray<Ptr<FWorldContext>> WorldList;
}