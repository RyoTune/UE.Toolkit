using Reloaded.Hooks.Definitions;
using System.Runtime.InteropServices;
using UE.Toolkit.Core.Common;
using UE.Toolkit.Core.Types.Interfaces;
using UE.Toolkit.Interfaces;
using UE.Toolkit.Reloaded.Common.GameConfigs;

// ReSharper disable InconsistentNaming

namespace UE.Toolkit.Reloaded.Unreal;

internal interface IFMalloc
{
    // Memory allocation stubs
    public nint Malloc(nint size, int alignment = MemoryConstants.DEFAULT_ALIGNMENT);
    public nint Realloc(nint ptr, nint size, int alignment = MemoryConstants.DEFAULT_ALIGNMENT);
    public void Free(nint original);
    public bool GetAllocSize(nint ptr, ref nint size);
    public nint QuantizeSize(nint count, int alignment = MemoryConstants.DEFAULT_ALIGNMENT);

    internal delegate nint FMemory_Malloc(nint self, nint size, int alignment = MemoryConstants.DEFAULT_ALIGNMENT);
    internal delegate nint FMemory_Realloc(nint self, nint ptr, nint size, int alignment = MemoryConstants.DEFAULT_ALIGNMENT);
    internal delegate void FMemory_Free(nint self, nint ptr);
    internal delegate char FMemory_GetAllocSize(nint self, nint ptr, ref nint size);
    internal delegate nint FMemory_QuantizeSize(nint self, nint count, int alignment = MemoryConstants.DEFAULT_ALIGNMENT);
}

// Runtime/Core/HAL/MallocBinned

internal class FMallocBinned : IFMalloc
{
    public void Free(nint original)
    {
        throw new NotImplementedException();
    }

    public bool GetAllocSize(nint ptr, ref nint size)
    {
        throw new NotImplementedException();
    }

    public nint Malloc(nint size, int alignment = MemoryConstants.DEFAULT_ALIGNMENT)
    {
        throw new NotImplementedException();
    }

    public nint QuantizeSize(nint count, int alignment = 0)
    {
        throw new NotImplementedException();
    }

    public nint Realloc(nint ptr, nint size, int alignment = MemoryConstants.DEFAULT_ALIGNMENT)
    {
        throw new NotImplementedException();
    }
}

// Runtime/Core/HAL/MallocBinned2
// used in Persona 3 Reload
internal class FMallocBinned2 : IFMalloc
{
    protected nint Ptr;

    private IFunctionPtr<IFMalloc.FMemory_Malloc>? _Malloc;
    private IFunctionPtr<IFMalloc.FMemory_Realloc>? _Realloc;
    private IFunctionPtr<IFMalloc.FMemory_Free>? _Free;
    private IFunctionPtr<IFMalloc.FMemory_GetAllocSize>? _GetAllocSize;
    private IFunctionPtr<IFMalloc.FMemory_QuantizeSize>? _QuantizeSize;
    private IReloadedHooks Hooks;
    public FMallocBinned2(nint _Ptr, IReloadedHooks _Hooks)
    {
        Ptr = _Ptr;
        Hooks = _Hooks;
    }

    public unsafe void Free(nint original)
    {
        if (_Free == null)
            _Free = Hooks.CreateFunctionPtr<IFMalloc.FMemory_Free>(**(nuint**)Ptr + 0x30);
        _Free.GetDelegate()(*(nint*)Ptr, original);
    }

    public unsafe bool GetAllocSize(nint ptr, ref nint size)
    {
        if (_GetAllocSize == null)
            _GetAllocSize = Hooks.CreateFunctionPtr<IFMalloc.FMemory_GetAllocSize>(**(nuint**)Ptr + 0x40);
        return _GetAllocSize.GetDelegate()(*(nint*)Ptr, ptr, ref size) != 0 ? true : false;
    }

    public unsafe nint Malloc(nint size, int alignment = MemoryConstants.DEFAULT_ALIGNMENT)
    {
        if (_Malloc == null)
            _Malloc = Hooks.CreateFunctionPtr<IFMalloc.FMemory_Malloc>(**(nuint**)Ptr + 0x10);
        var value = _Malloc.GetDelegate()(*(nint*)Ptr, size, alignment);
        return value;
    }

    public unsafe nint Realloc(nint ptr, nint size, int alignment = MemoryConstants.DEFAULT_ALIGNMENT)
    {
        if (_Realloc == null)
            _Realloc = Hooks.CreateFunctionPtr<IFMalloc.FMemory_Realloc>(**(nuint**)Ptr + 0x20);
        return _Realloc.GetDelegate()(*(nint*)Ptr, ptr, size, alignment);
    }

    public unsafe nint QuantizeSize(nint count, int alignment = MemoryConstants.DEFAULT_ALIGNMENT)
    {
        if (_QuantizeSize == null)
            _QuantizeSize = Hooks.CreateFunctionPtr<IFMalloc.FMemory_QuantizeSize>(**(nuint**)Ptr + 0x38);
        return _QuantizeSize.GetDelegate()(*(nint*)Ptr, count, alignment);
    }
}

// Runtime/Core/HAL/MallocBinned3

internal class FMallocBinned3 : IFMalloc
{
    public void Free(nint original)
    {
        throw new NotImplementedException();
    }

    public bool GetAllocSize(nint ptr, ref nint size)
    {
        throw new NotImplementedException();
    }

    public nint Malloc(nint size, int alignment = MemoryConstants.DEFAULT_ALIGNMENT)
    {
        throw new NotImplementedException();
    }

    public nint QuantizeSize(nint count, int alignment = 0)
    {
        throw new NotImplementedException();
    }

    public nint Realloc(nint ptr, nint size, int alignment = MemoryConstants.DEFAULT_ALIGNMENT)
    {
        throw new NotImplementedException();
    }
}

public class UnrealMemory : IUnrealMemory
{
    internal static IFMalloc? _FMemory;

    public UnrealMemory()
    {
        ScanHooks.Add("GMalloc", GameConfig.Instance.GMalloc, (hooks, result) =>
        {
            _FMemory = new FMallocBinned2(ToolkitUtils.GetGlobalAddress(result + 3), hooks);
        });
    }
    
    public nint Malloc(nint count, int alignment = MemoryConstants.DEFAULT_ALIGNMENT) => _FMemory!.Malloc(count, alignment);

    public void Free(nint original) => _FMemory!.Free(original);

    public nint Realloc(nint ptr, nint size, int alignment = MemoryConstants.DEFAULT_ALIGNMENT) => _FMemory!.Realloc(ptr, size, alignment);

    public bool GetAllocSize(nint ptr, ref nint size) => _FMemory!.GetAllocSize(ptr, ref size);
    public nint QuantizeSize(nint count, int alignment = MemoryConstants.DEFAULT_ALIGNMENT) => _FMemory!.QuantizeSize(count, alignment);

    public nint MallocZeroed(nint count, int alignment = MemoryConstants.DEFAULT_ALIGNMENT)
    {
        var alloc = Malloc(count, alignment);
        unsafe { NativeMemory.Clear((void*)alloc, (nuint)count); }
        return alloc;
    }
}