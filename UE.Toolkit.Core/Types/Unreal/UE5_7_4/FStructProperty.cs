using System.Runtime.InteropServices;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Core.Types.Unreal.UE5_7_4;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct FStructProperty
{
    public FProperty Super;
    public UScriptStruct* Struct;
}