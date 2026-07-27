using System.Runtime.InteropServices;
using UE.Toolkit.Core.Types.Interfaces;

namespace UE.Toolkit.Core.Types.Unreal.Common.FunctionParam;

public class TextParam(nint value, int ftextSize, IUnrealMemoryInternal? memory = null) 
    : IFunctionParam, IDisposable
{
    public unsafe nint Value => value;

    protected IUnrealMemoryInternal? Memory => memory;

    public string PropertyType => "TextProperty";

    private int FTextSize => ftextSize;

    private unsafe void Copy(nint source, nint destination) =>
        NativeMemory.Copy((void*)source, (void*)destination, (nuint)FTextSize);

    public void Write(nint Destination) => Copy(Value, Destination);

    public void Read(nint Destination) => Copy(Destination, Value);

    protected bool IsDisposed;

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
            Memory?.Free(Value);
            IsDisposed = true;
        }
    }
    
    ~TextParam() => Dispose(false);
}