using System.Runtime.InteropServices;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Core.Types.Unreal.UE5_6_1;

[StructLayout(LayoutKind.Explicit, Size = 0x1240)]
public struct UEngine
{
    [FieldOffset(0x1148)] public TArray<Ptr<FWorldContext>> WorldList;
}