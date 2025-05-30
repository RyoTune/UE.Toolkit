using System.Runtime.InteropServices;

namespace UE.Toolkit.Core.Types.Unreal.UE5_4_4;

[StructLayout(LayoutKind.Sequential, Size = 0x30, Pack = 8)]
public unsafe struct UField
{
    public UObjectBase Super;
    public UField* Next;
}