using System.Runtime.InteropServices;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Core.Types.Unreal.UE5_6_1;

[StructLayout(LayoutKind.Sequential, Size = 0xC0)]
public struct UScriptStruct
{
    public UStruct Super;
    public EStructFlags StructFlags;
    public bool bPrepareCppStructOpsCompleted;
    public nint CppStructOps;
}