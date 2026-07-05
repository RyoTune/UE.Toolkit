using System.Runtime.InteropServices;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Core.Types.Unreal.UE5_7_4;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
public unsafe struct FFieldClass
{
    public FName Name;
    public EClassFlags ClassFlags;
    public ulong Id;
    public ulong CastFlags;
    public FFieldClass* SuperClass;
    public FField* DefaultObject;
    public nint FieldConstructor;
    public FThreadSafeCounter UnqiueNameIndexCounter;
}

[StructLayout(LayoutKind.Sequential)]
public struct FThreadSafeCounter
{
    public int Counter;
}