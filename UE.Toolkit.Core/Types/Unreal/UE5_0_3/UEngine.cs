using System.Runtime.InteropServices;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Core.Types.Unreal.UE5_0_3;

[StructLayout(LayoutKind.Explicit, Size = 0xd90)]
public struct UEngine
{
    [FieldOffset(0xca0)] public TArray<Ptr<FWorldContext>> WorldList;   
}