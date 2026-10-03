using System.Runtime.InteropServices;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Core.Types.Unreal.UE5_2_1;

[StructLayout(LayoutKind.Explicit, Size = 0xfb8)]
public struct UEngine
{
    [FieldOffset(0xec0)] public TArray<Ptr<FWorldContext>> WorldList;   
}