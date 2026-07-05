using System.Runtime.InteropServices;
using EObjectFlags = UE.Toolkit.Core.Types.Unreal.UE5_4_4.EObjectFlags;
using EPropertyFlags = UE.Toolkit.Core.Types.Unreal.UE5_4_4.EPropertyFlags;
using EPropertyGenFlags = UE.Toolkit.Core.Types.Unreal.UE5_4_4.EPropertyGenFlags;

namespace UE.Toolkit.Core.Types.Unreal.UE5_2_1;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct FPropertyParamsBase
{
    public nint NameUTF8;
    public nint RepNotifyFuncUTF8;
    public EPropertyFlags PropertyFlags;
    public EPropertyGenFlags PropertyGenFlags;
    public EObjectFlags ObjectFlags;
    public int ArrayDim;
    public delegate* unmanaged[Stdcall]<nint, nint, void> SetterFuncPtr;
    public delegate* unmanaged[Stdcall]<nint, nint, void> GetterFuncPtr;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct FPropertyParamsBaseWithOffset
{
    // public FPropertyParamsBase Super;
    public nint NameUTF8;
    public nint RepNotifyFuncUTF8;
    public EPropertyFlags PropertyFlags;
    public EPropertyGenFlags PropertyGenFlags;
    public EObjectFlags ObjectFlags;
    public int ArrayDim;
    public delegate* unmanaged[Stdcall]<nint, nint, void> SetterFuncPtr;
    public delegate* unmanaged[Stdcall]<nint, nint, void> GetterFuncPtr;
    public int Offset;
}

[StructLayout(LayoutKind.Sequential)]
public struct FGenericPropertyParams
{
    public FPropertyParamsBaseWithOffset Super;
}