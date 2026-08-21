using System.Runtime.InteropServices;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Core.Types.Unreal.UE5_7_4;

[StructLayout(LayoutKind.Sequential, Size = 0xC0)]
public struct UScriptStruct
{
    public UStruct Super;
    public EStructFlags StructFlags;
    public bool bPrepareCppStructOpsCompleted;
    public nint CppStructOps;
}

[StructLayout(LayoutKind.Sequential, Size = 0x18)]
public struct FStructOpsFakeVTable
{
    
}

public unsafe struct ICppStructOps
{
    public FStructOpsFakeVTable* FakeVTable;
    public int Size;
    public int Alignment;
}