using System.Runtime.InteropServices;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Core.Types.Unreal.UE5_5_4;

[StructLayout(LayoutKind.Explicit, Size = 0x11f0)]
public struct UEngine
{
    [FieldOffset(0x10f8)] public TArray<Ptr<FWorldContext>> WorldList;
}