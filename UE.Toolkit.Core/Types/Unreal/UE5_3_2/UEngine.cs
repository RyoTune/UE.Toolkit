using System.Runtime.InteropServices;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Core.Types.Unreal.UE5_3_2;

[StructLayout(LayoutKind.Explicit, Size = 0x1078)]
public struct UEngine
{
    [FieldOffset(0xf80)] public TArray<Ptr<FWorldContext>> WorldList;   
}