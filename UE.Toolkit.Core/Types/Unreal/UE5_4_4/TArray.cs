// ReSharper disable InconsistentNaming

using System.Collections;
using System.Runtime.InteropServices;
using UE.Toolkit.Core.Types.Interfaces;

namespace UE.Toolkit.Core.Types.Unreal.UE5_4_4;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct TArray<T> where T : unmanaged
{
    public T* AllocatorInstance;
    public int ArrayNum;
    public int ArrayMax;

    public TArrayList<T> ToManaged(IUnrealMemoryInternal Memory)
    {
        fixed (TArray<T>* self = &this) return new TArrayList<T>(self, Memory);
    }
}

public unsafe class TArrayListStatic
{
    internal const int DEFAULT_ARRAY_SIZE = 4;
    
    public static int CalculateNewArraySizeStatic(TArray<byte>* Arr) => Arr->AllocatorInstance != null ? Arr->ArrayMax * 2 : DEFAULT_ARRAY_SIZE;
    
    // Expose this to allow for TArray reallocation in Object XML
    public static void ResizeToStatic(TArray<byte>* Arr, int newSize, int elemSize, IUnrealMemoryInternal Allocator)
    {
        var newAlloc = (byte*)Allocator.Malloc(elemSize * newSize);
        if (Arr->AllocatorInstance != null)
        {
            NativeMemory.Copy(Arr->AllocatorInstance, newAlloc, (nuint)(Arr->ArrayMax * elemSize));
            Allocator.Free((nint)Arr->AllocatorInstance);
        }
        Arr->AllocatorInstance = newAlloc;
        Arr->ArrayMax = newSize;
    }
}

/// <summary>
/// <para>
/// A dynamically sizable array of typed elements. Assumes that your elements can be relocated without a copy constructor.
/// The main implication is that pointers to elements in the TArray may be invalidated when adding or removing other elements
/// within the array. Removal of elements is O(N) and invalidates the indices of subsequent elements.
/// </para>
/// <para>
/// WARNING: Removal methods such as <c>Clear()</c> and <c>Remove()</c> do not invoke the destructor for the inner object!
/// The user has to manually invoke the destructor by calling <c>Dispose()</c> on the target object!
/// </para>
/// </summary>
/// <typeparam name="TType">The type to store instances of </typeparam>
public unsafe class TArrayList<TType> : IDisposable, IList<Ptr<TType>> where TType : unmanaged
    // TArrayListBase<TType, TStrategy = HeapAllocator<TType, int32>, TSize = int32>
{
    protected TArray<TType>* Self;
    protected IUnrealMemoryInternal Allocator;
    protected bool OwnsInstance;
    protected bool Disposed = false;

    

    /// <summary>
    /// Wraps a <c>TArrayList</c> around an existing <c>TArray</c> created in C++
    /// </summary>
    /// <param name="_Self">Pointer to an existing <c>TArray</c></param>
    /// <param name="_Allocator">The Unreal allocator, used for methods that modify the <c>TArray</c></param>
    public TArrayList(TArray<TType>* _Self, IUnrealMemoryInternal _Allocator)
    {
        Self = _Self;
        Allocator = _Allocator;
        OwnsInstance = false;
    }

    /// <summary>
    /// Creates a new <c>TArrayList</c> by allocating an owned <c>TArray</c> that frees itself when this object is garbage collected.
    /// </summary>
    /// <param name="_Allocator">The Unreal allocator, used for methods that modify the <c>TArray</c></param>
    public TArrayList(IUnrealMemoryInternal _Allocator)
    {
        Allocator = _Allocator;
        Self = (TArray<TType>*)Allocator.MallocZeroed(sizeof(TArray<TType>));
        OwnsInstance = true;
    }

    public TType* Allocation
    {
        get => Self->AllocatorInstance;
        private set => Self->AllocatorInstance = value;
    }

    public int ArrayNum
    {
        get => Self->ArrayNum;
        protected set => Self->ArrayNum = value;
    }

    public int ArrayMax
    {
        get => Self->ArrayMax;
        protected set => Self->ArrayMax = value;
    }

    public TArray<TType>* Base() => Self;

    bool InBounds(int index) => index >= 0 && index < ArrayNum;
    bool InBoundsForInsertion(int index) => index >= 0 && index <= ArrayNum;

    int CalculateNewArraySize() => TArrayListStatic.CalculateNewArraySizeStatic((TArray<byte>*)Self);

    /// <summary>
    /// Relinquish ownership of this <c>TArray</c>. This is used in cases where you know that Unreal will deallocate it or it otherwise
    /// doesn't need deallocation.
    /// </summary>
    public void Leak() => OwnsInstance = false;

    public void ResizeTo(int newSize) => TArrayListStatic.ResizeToStatic((TArray<byte>*)Self, newSize, sizeof(TType), Allocator);

    public void Resize() => ResizeTo(CalculateNewArraySize());

    void InsertInner(int index, Ptr<TType> item, bool add)
    {
        if (add)
        {
            // Allow assigning to this[ArrayNum] to add a new entry
            if (!InBoundsForInsertion(index))
            {
                return;
            }
            if (ArrayNum == ArrayMax)
            {
                Resize();
            }
            // Shift elements to the right
            for (int i = ArrayNum; i > index; i--)
            {
                Allocation[i] = Allocation[i - 1];
            }
            ArrayNum++;
        }
        else
        {
            // We're only replacing existing entries, so this[ArrayNum] is out of bounds
            if (!InBounds(index))
            {
                return;
            }
        }
        Allocation[index] = *item.Value;
    }

    #region BY-VALUE METHODS

    public void AddValue(TType item) => Add(new(&item));
    public bool ContainsValue(TType item)
    {
        foreach (var el in this)
        {
            if ((*el.Value).Equals(item))
            {
                return true;
            }
        }
        return false;
    }
    public TType GetValue(int Index) => *this[Index].Value;

    public int IndexOfValue(TType item)
    {
        int index = 0;
        foreach (var el in this)
        {
            if ((*el.Value).Equals(item))
            {
                return index;
            }
            index++;
        }
        return -1;
    }

    public bool RemoveValue(TType item)
    {
        int ItemIndex = IndexOfValue(item);
        if (ItemIndex != -1)
        {
            RemoveAt(ItemIndex);
            return true;
        }
        return false;
    }

    #endregion

    #region LIST INTERFACE

    public Ptr<TType> this[int index]
    {
        get
        {
            if (InBounds(index))
            {
                return new(&Allocation[index]);
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }
        set => InsertInner(index, value, false);
    }

    public int Count => ArrayNum;

    public bool IsReadOnly => false;

    public void Add(Ptr<TType> item) => Insert(Count, item);

    public void Clear() => ArrayNum = 0;

    public bool Contains(Ptr<TType> item)
    {
        foreach (var el in this)
        {
            if (el.Equals(item))
            {
                return true;
            }
        }
        return false;
    }

    public void CopyTo(Ptr<TType>[] array, int arrayIndex)
    {
        if (!InBounds(arrayIndex))
        {
            return;
        }
        for (int i = 0; i < ArrayNum - arrayIndex; i++)
        {
            array[i] = new(&Allocation[i + arrayIndex]);
        }
    }

    public IEnumerator<Ptr<TType>> GetEnumerator() => new TArrayEnumerator<TType>(this);

    public int IndexOf(Ptr<TType> item)
    {
        int index = 0;
        foreach (var el in this)
        {
            if (el.Equals(item))
            {
                return index;
            }
            index++;
        }
        return -1;
    }

    public void Insert(int index, Ptr<TType> item) => InsertInner(index, item, true);
    public bool Remove(Ptr<TType> item)
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
        for (int i = index; i < ArrayNum - 1; i++)
        {
            Allocation[i] = Allocation[i + 1];
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

    ~TArrayList() => Dispose(false);

    #endregion
}

public class TArrayEnumerator<T> : IEnumerator<Ptr<T>> where T : unmanaged
{
    protected TArrayList<T> Self;
    private int position = -1;

    public object Current
    {
        get => Self[position];
    }
    public TArrayEnumerator(TArrayList<T> _Self) => Self = _Self;
    public bool MoveNext() => ++position < Self.ArrayNum;
    Ptr<T> IEnumerator<Ptr<T>>.Current => (Ptr<T>)Current;
    public void Reset() => position = -1;
    public void Dispose() { }
}