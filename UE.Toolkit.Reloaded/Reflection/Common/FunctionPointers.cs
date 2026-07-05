using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Reloaded.Reflection.Common;

public static class FunctionPointers
{
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    internal static byte Noop_Bool_False() => 0;
    
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    internal static byte Noop_Bool_True() => 1;
    
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    internal static void Noop_Void() {}

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    internal static uint Noop_U32() => 0;

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    internal static unsafe FCapabilities* Noop_GetCapabilities(nint self, FCapabilities* __return_storage_ptr__)
    {
        return __return_storage_ptr__;
    }
}