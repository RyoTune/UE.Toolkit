using System.Runtime.InteropServices;

namespace UE.Toolkit.Core.Types.Unreal.UE5_4_4;

[StructLayout(LayoutKind.Explicit, Size = 0x10a0)]
public struct UEngine
{
    [FieldOffset(0xfa8)] public TArray<Ptr<FWorldContext>> WorldList;
}