using System.Runtime.InteropServices;
using UE.Toolkit.Core.Types.Interfaces;

namespace UE.Toolkit.Core.Types.Unreal.Common.FunctionParam;

public class EnumParam(nint ptr, int size, IUnrealMemoryInternal? memory = null) : IFunctionParam, IDisposable
{
    public string PropertyType => "EnumProperty";

    private int Size => size;

    private nint Ptr => ptr;
    
    private IUnrealMemoryInternal? Memory => memory;
    
    private unsafe void Copy(nint source, nint destination) 
        => NativeMemory.Copy((void*)source, (void*)destination, (nuint)Size);

    public void Write(nint Destination) => Copy(Ptr, Destination);
    public void Read(nint Destination) => Copy(Destination, Ptr);
    
    private bool IsDisposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!IsDisposed)
        {
            if (disposing) { }
            Memory?.Free(Ptr);
            IsDisposed = true;
        }
    }
    
    ~EnumParam() => Dispose(false);
}