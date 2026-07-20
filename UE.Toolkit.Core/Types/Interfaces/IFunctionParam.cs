namespace UE.Toolkit.Core.Types.Interfaces;

public interface IFunctionParam
{
    string PropertyType { get; }
    void Write(nint Destination);
    void Read(nint Destination);
}

public abstract class FunctionParamCopyable<T>(Ptr<T> rvalue, string propertyType, IUnrealMemoryInternal? memory = null) 
    : IFunctionParam, IDisposable where T : unmanaged
{
    public unsafe T Value => *RValue.Value;
    
    protected Ptr<T> RValue => rvalue;

    protected IUnrealMemoryInternal? Memory => memory;

    public string PropertyType => propertyType;

    private static unsafe void Copy(T* source, T* destination) => *destination = *source;

    public unsafe void Write(nint Destination) => Copy(RValue.Value, (T*)Destination);

    public unsafe void Read(nint Destination) => Copy((T*)Destination, RValue.Value);

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

            if (Memory != null)
            {
                unsafe { Memory.Free((nint)RValue.Value); }
            }
            IsDisposed = true;
        }
    }
    
    ~FunctionParamCopyable() => Dispose(false);
}

public enum ProcessEventResult
{
    Success,
    CouldNotFindFunction,
    ParameterTypeMismatch,
}