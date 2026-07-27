using UE.Toolkit.Core.Types.Interfaces;

namespace UE.Toolkit.Core.Types.Unreal.Common.FunctionParam;

public class BoolParam(Ptr<bool> rvalue, int fieldMask, IUnrealMemoryInternal? memory = null) : IFunctionParam, IDisposable
{
    public unsafe bool Value => *RValue.Value;
    
    protected Ptr<bool> RValue => rvalue;

    protected IUnrealMemoryInternal? Memory => memory;

    public string PropertyType => "BoolProperty";

    public int FieldMask => fieldMask;

    private static unsafe void CopyByte(byte* source, byte* destination) => *destination = *source;

    private unsafe bool IsStateDifferent(nint Destination)
    {
        var isDestTrue = (*(byte*)Destination & Convert.ToByte(FieldMask)) != 0;
        return isDestTrue != Value;
    }

    public unsafe void Write(nint Destination)
    {
        if (FieldMask == byte.MaxValue) CopyByte((byte*)RValue.Value, (byte*)Destination);
        else *(byte*)Destination ^= (byte)(Convert.ToByte(IsStateDifferent(Destination)) * FieldMask);
    }

    public unsafe void Read(nint Destination)
    {
        if (FieldMask == byte.MaxValue) CopyByte((byte*)Destination, (byte*)RValue.Value);
        else *RValue.Value = IsStateDifferent(Destination);
    }

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
            unsafe { Memory?.Free((nint)RValue.Value); }
            IsDisposed = true;
        }
    }
    
    ~BoolParam() => Dispose(false);
}