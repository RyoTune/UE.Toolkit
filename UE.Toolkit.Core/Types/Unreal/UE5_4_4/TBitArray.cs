using System.Collections;
using System.Runtime.InteropServices;
using UE.Toolkit.Core.Types.Interfaces;

// ReSharper disable InconsistentNaming

namespace UE.Toolkit.Core.Types.Unreal.UE5_4_4;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct TBitArray
{
    public int* AllocatorInstance;
    public int NumBits;
    public int MaxBits;
}

internal static class TBitArrayConstants // TInlineAllocator<4>
{
    internal const int BITS_PER_BYTE = 0x8;
    internal const int DEFAULT_ALLOCATOR_SIZE = sizeof(int) * 4;
    internal const int BYTE_MAX = byte.MaxValue;
}

/// <summary>
/// <para>
/// A dynamically sized bit array. Represents an array of booleans, which are stored as one bit each.
/// </para>
/// </summary>
public unsafe class TBitArrayList : IDisposable, IList<bool>
// TArrayListBase<TType, TStrategy = InlineAllocator<int, int32, 4>, TSize = int32>
{
    // Inline allocators insert their elements as the first field of the structure, followed by the heap pointer
    // then the count + capacity fields

    // The inline allocation policy allocates up to a specified number of elements in the same allocation as the container.
    // Any allocation needed beyond that causes all data to be moved into an indirect allocation.
    public byte* Self { get; private set; } // Start with InlineAllocatorSize bytes of inlined flags, then a TArray with remaining flags

    protected IUnrealMemoryInternal Allocator;
    private int InlineAllocatorSize;
    protected bool OwnsInstance;
    protected bool Disposed = false;

    /// <summary>
    /// Wraps a <c>TArrayList</c> around an existing <c>TBitArray</c> created in C++
    /// </summary>
    /// <param name="_Self">Pointer to an existing <c>TBitArray</c></param>
    /// <param name="_Allocator">The Unreal allocator, used for methods that modify the <c>TBitArray</c></param>
    public TBitArrayList(byte* _Self, IUnrealMemoryInternal _Allocator, int _InlineAllocatorSize = TBitArrayConstants.DEFAULT_ALLOCATOR_SIZE)
    {
        Allocator = _Allocator;
        InlineAllocatorSize = _InlineAllocatorSize;
        Self = _Self;
        OwnsInstance = false;
    }

    /// <summary>
    /// Creates a new <c>TArrayList</c> by allocating an owned <c>TArray</c> that frees itself when this object is garbage collected.
    /// </summary>
    /// <param name="_Allocator">The Unreal allocator, used for methods that modify the <c>TBitArray</c></param>
    public TBitArrayList(IUnrealMemoryInternal _Allocator, int _InlineAllocatorSize = TBitArrayConstants.DEFAULT_ALLOCATOR_SIZE)
    {
        Allocator = _Allocator;
        InlineAllocatorSize = _InlineAllocatorSize;
        Self = (byte*)Allocator.MallocZeroed(GetStructSize());
        ArrayMax = InlineBits;
        OwnsInstance = true;
    }

    protected byte* Inline
    {
        get => Self;
    }

    internal int InlineBits => InlineAllocatorSize * TBitArrayConstants.BITS_PER_BYTE;

    protected byte* Allocation
    {
        get => *(byte**)(Self + InlineAllocatorSize);
        set => *(byte**)(Self + InlineAllocatorSize) = value;
    }

    protected byte* Data
    {
        get => ArrayNum > InlineBits ? Allocation : Inline;
        set
        {
            if (Allocation != null)
            {
                Allocation = value;
            }
        }
    }

    public int ArrayNum
    {
        get => *(int*)(Self + InlineAllocatorSize + sizeof(nint));
        protected set => *(int*)(Self + InlineAllocatorSize + sizeof(nint)) = value;
    }

    public int ArrayMax
    {
        get => *(int*)(Self + InlineAllocatorSize + sizeof(nint) + sizeof(int));
        protected set => *(int*)(Self + InlineAllocatorSize + sizeof(nint) + sizeof(int)) = value;
    }

    bool InBounds(int index) => index is >= 0 && index < ArrayNum;
    bool InBoundsForInsertion(int index) => index >= 0 && index <= ArrayNum;
    /// <summary>
    /// Relinquish ownership of this <c>TArray</c>. This is used in cases where you know that Unreal will deallocate it or it otherwise
    /// doesn't need deallocation.
    /// </summary>
    public void Leak() => OwnsInstance = false;
    private int GetStructSize() => InlineAllocatorSize + sizeof(nint) + sizeof(int) * 2;
    private static int GetDefaultStructSize() => TBitArrayConstants.DEFAULT_ALLOCATOR_SIZE + sizeof(nint) + sizeof(int) * 2;
    int CalculateNewArraySize() => ArrayMax * 2;

    public void ResizeTo(int newSize)
    {
        var newAlloc = Allocator.MallocZeroed(newSize / TBitArrayConstants.BITS_PER_BYTE);
        if (Allocation != null)
        { // Resize heap booleans
            NativeMemory.Copy(Allocation, (void*)newAlloc, (nuint)(ArrayNum / TBitArrayConstants.BITS_PER_BYTE));
            Allocator.Free((nint)Allocation);
        } else
        { // Migrate booleans from inline storage to heap
            NativeMemory.Copy(Inline, (byte*)newAlloc, (nuint)InlineBits / TBitArrayConstants.BITS_PER_BYTE);
            NativeMemory.Clear(Inline, (nuint)InlineBits / TBitArrayConstants.BITS_PER_BYTE);
        }
        Allocation = (byte*)newAlloc;
        ArrayMax = newSize;
    }

    public void Resize() => ResizeTo(CalculateNewArraySize());

    public byte* Base() => Self;

    void InsertInner(int index, bool item, bool add)
    {
        if (add)
        {
            // Allow assigning to this[ArrayNum] to add a new entry
            if (!InBoundsForInsertion(index)) return;
            if (ArrayNum == ArrayMax) Resize();
            // Shifts bits if we're inserting into an index other than ArrayNum
            // Shift bytes to the right
            var StartIndex = ArrayNum / TBitArrayConstants.BITS_PER_BYTE;
            var EndIndex = index / TBitArrayConstants.BITS_PER_BYTE;
            for (var i = StartIndex; i > EndIndex; i--)
            {
                Data[i + 1] |= (byte)(Data[i] >> 7);
                Data[i] <<= 1;
            }
            // Shifts bits to the right
            var EndBit = index % TBitArrayConstants.BITS_PER_BYTE;
            byte BitMaskGen() => (byte)((1 << EndBit) - 1);
            var NoShiftBits = Data[EndIndex] & BitMaskGen();
            var ShiftBits = (Data[EndIndex] & ~BitMaskGen()) << 1;
            Data[EndIndex] = (byte)(NoShiftBits | ShiftBits);
            ArrayNum++;
        }
        else
        {
            // We're only replacing existing entries, so this[ArrayNum] is out of bounds
            if (!InBounds(index)) return;
        }
        if (item)
        {
            Data[index / TBitArrayConstants.BITS_PER_BYTE] |= (byte)(1 << (index % TBitArrayConstants.BITS_PER_BYTE));
        }
        else
        {
            Data[index / TBitArrayConstants.BITS_PER_BYTE] &= (byte)(TBitArrayConstants.BYTE_MAX ^ (1 << (index % TBitArrayConstants.BITS_PER_BYTE)));
        }
    }

    #region LIST INTERFACE

    public bool this[int index] 
    { 
        get
        {
            if (InBounds(index))
            {
                return (Data[index / TBitArrayConstants.BITS_PER_BYTE] & (1 << (index % TBitArrayConstants.BITS_PER_BYTE))) != 0;
            }
            throw new IndexOutOfRangeException();
        }
        set => InsertInner(index, value, false);
    }

    public int Count => ArrayNum;

    public bool IsReadOnly => false;

    public void Add(bool item) => Insert(Count, item);

    public void Clear()
    {
        NativeMemory.Clear(Data, (nuint)ArrayMax / TBitArrayConstants.BITS_PER_BYTE);
        ArrayNum = 0;
        ArrayMax = InlineBits;
    }

    public bool Contains(bool item)
    {
        foreach (var b in this)
        {
            if (b == item)
            {
                return true;
            }
        }
        return false;
    }

    public void CopyTo(bool[] array, int arrayIndex)
    {
        if (!InBounds(arrayIndex))
        {
            return;
        }
        for (int i = 0; i < ArrayNum - arrayIndex; i++)
        {
            array[i] = this[i + arrayIndex];
        }
    }

    public IEnumerator<bool> GetEnumerator() => new TBitArrayEnumerator(this);

    public int IndexOf(bool item)
    {
        int index = 0;
        foreach (var b in this)
        {
            if (b == item) return index;
            index++;
        }
        return -1;
    }

    public void Insert(int index, bool item) => InsertInner(index, item, true);

    public bool Remove(bool item)
    {
        int ItemIndex = IndexOf(item);
        if (ItemIndex != -1)
        {
            RemoveAt(ItemIndex);
            return true;
        }
        return false;
    }

    public void RemoveAt(int index)
    {
        if (!InBounds(index))
        {
            return;
        }
        // Shift elements to the left
        var StartIndex = index / TBitArrayConstants.BITS_PER_BYTE;
        var EndIndex = (ArrayNum - 1) / TBitArrayConstants.BITS_PER_BYTE;
        for (int i = StartIndex; i <= EndIndex; i++)
        {
            if (i > 0) 
            { 
                Data[i - 1] |= (byte)((Data[i] & 1) << 7); 
            }
            Data[i] >>= 1;
        }
        ArrayNum--;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion

    #region DISPOSE INTERFACE

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!Disposed)
        {
            if (disposing) { } // Dispose managed resources
            // Disposed unmanaged resources (for Unreal)
            if (OwnsInstance)
            {
                if (Allocation != null)
                {
                    Allocator.Free((nint)Allocation);
                }
                Allocator.Free((nint)Self);
            }
            Disposed = true;
        }
    }

    ~TBitArrayList() => Dispose(false);

    #endregion
}

public class TBitArrayEnumerator : IEnumerator<bool>
{
    private TBitArrayList Owner;
    private int Index = -1;
    public TBitArrayEnumerator(TBitArrayList _Owner) { Owner = _Owner; }
    public bool Current => Owner[Index];
    object IEnumerator.Current => Current;
    public void Dispose() { }
    public bool MoveNext() => ++Index < Owner.Count;
    public void Reset() => Index = -1;
}