using System.Runtime.InteropServices;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Core.Types.Unreal.UE5_1_1;

[StructLayout(LayoutKind.Explicit, Size = 0x1050)]
public struct UEngine
{
    [FieldOffset(0xea8)] public TArray<Ptr<FWorldContext>> WorldList;   
}