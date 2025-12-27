// ReSharper disable InconsistentNaming

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using UE.Toolkit.Core.Types.Interfaces;

namespace UE.Toolkit.Core.Types.Unreal.UE5_4_4;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct TMap<InKeyType, InValueType>
    where InKeyType : unmanaged
    where InValueType : unmanaged
{
    // From TArray
    public TMapElement<InKeyType, InValueType>* Elements; // Data
    public int MapNum; // ArrayNum
    public int MapMax; // ArrayMax

    // From TSparseArray
    // public TBitArray AllocationFlags;
    // public int FirstFreeIndex;
    // public int NumFreeIndices;
    
    // From TSet (probably)
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct TMapElement<InKeyType, InValueType>
    where InKeyType : unmanaged
    where InValueType : unmanaged
{
    public InKeyType Key;
    public InValueType* Value;

    // From TSetElementBase
    /** The id of the next element in the same hash bucket. */
    public int HashNextId; // FSetElementId

    /** The hash bucket that the element is currently linked to. */
    public int HashIndex;
}

/// <summary>
/// Defines types that are hashable. Useful in cases such as using it as a key for a <c>TMap</c>
/// </summary>
public interface IMapHashable
{
    public uint GetTypeHash();
}

// Built in hashable types

[StructLayout(LayoutKind.Sequential)]
public struct TMapElementHashable<KeyType, ValueType>
    where KeyType : unmanaged, IEquatable<KeyType>, IMapHashable
    where ValueType : unmanaged
{
    public KeyType Key;
    public ValueType Value;
    public int HashNextId;
    public int HashIndex;
}

public unsafe struct HashablePtr<T>(Ptr<T> ptr) : IMapHashable, IEquatable<HashablePtr<T>> where T: unmanaged
{
    public Ptr<T> Ptr = ptr;
    public uint GetTypeHash() => HashablePtrUtils.GetPtrTypeHash((nint)Ptr.Value); 
    public bool Equals(HashablePtr<T> other) => Ptr.Value == other.Ptr.Value;
}

public struct HashableUntypedPtr(nint ptr) : IMapHashable, IEquatable<HashableUntypedPtr>
{
    public nint Ptr = ptr;
    public uint GetTypeHash() => HashablePtrUtils.GetPtrTypeHash(Ptr);

    public bool Equals(HashableUntypedPtr other) => Ptr == other.Ptr;
}

internal static class HashablePtrUtils
{
    internal static uint GetPtrTypeHash(nint Ptr) // FUN_140904980
    {
        uint iVar4 = (uint)(Ptr >> 4);
        uint uVar3 = 0x9e3779b9U - iVar4 ^ iVar4 << 8;
        uint uVar1 = (uint)-(uVar3 + iVar4) ^ uVar3 >> 0xd;
        uint uVar5 = (iVar4 - uVar3) - uVar1 ^ uVar1 >> 0xc;
        uVar3 = (uVar3 - uVar5) - uVar1 ^ uVar5 << 0x10;
        uVar1 = (uVar1 - uVar3) - uVar5 ^ uVar3 >> 5;
        uVar5 = (uVar5 - uVar3) - uVar1 ^ uVar1 >> 3;
        uVar3 = (uVar3 - uVar5) - uVar1 ^ uVar5 << 10;
        uint ret = ((uVar1 - uVar3) - uVar5) ^ (uVar3 >> 0xf);
        return ret;   
    }   
}

public struct HashableInt : IMapHashable, IEquatable<HashableInt>
{
    public int Value;
    public HashableInt(int value) { Value = value; }
    public uint GetTypeHash() => (uint)Value;
    public bool Equals(HashableInt other) => other.Value == Value;
    public override string ToString() => Value.ToString();
}

[StructLayout(LayoutKind.Explicit, Size = 0x8)]
public struct HashableInt8 : IMapHashable, IEquatable<HashableInt8>
{
    [FieldOffset(0x0)] public int Value;
    public HashableInt8(int value) { Value = value; }
    public uint GetTypeHash() => (uint)Value;
    public bool Equals(HashableInt8 other) => other.Value == Value;
    public override string ToString() => Value.ToString();
}

public static class TypeExtensions
{
    public static HashablePtr<T> AsHashable<T> (this Ptr<T> ptr) where T : unmanaged => new(ptr);
    public static HashableInt AsHashable(this int val) => new(val);
    public static HashableInt8 AsHashable8(this int val) => new(val);
    public static uint HashCombine(uint a, uint b)
    { // FUN_141cbc830
        uint uVar1 = a - b ^ b >> 0xd;
        uint uVar3 = (uint)(-0x61c88647 - uVar1) - b ^ uVar1 << 8;
        uint uVar2 = (b - uVar3) - uVar1 ^ uVar3 >> 0xd;
        uVar1 = (uVar1 - uVar3) - uVar2 ^ uVar2 >> 0xc;
        uVar3 = (uVar3 - uVar1) - uVar2 ^ uVar1 << 0x10;
        uVar2 = (uVar2 - uVar3) - uVar1 ^ uVar3 >> 5;
        uVar1 = (uVar1 - uVar3) - uVar2 ^ uVar2 >> 3;
        uVar3 = (uVar3 - uVar1) - uVar2 ^ uVar1 << 10;
        return (uVar2 - uVar3) - uVar1 ^ uVar3 >> 0xf;
    }
}

// Look for edge cases where this isn't the case.
internal static class MapConstants
{
    internal const int SIZE_OF_ARRAY = 0x10;
    internal const int SIZE_OF_BIT_ALLOCATOR = 0x20;
    internal const int SIZE_OF_FREE_LIST = 0x10;

    internal const int DEFAULT_ARRAY_SIZE = 4;
    internal static int MIN_SIZE_FOR_HASH_LIST = 0x4;
    internal static int HASH_INITIAL_SIZE = 0x10;

    internal const int INVALID_HASH_ID = -1;
}

public class TMapElementAccessor<TElemKey, TElemValue> : IEnumerable<Ptr<TElemValue>>, IDisposable
    where TElemKey : unmanaged, IEquatable<TElemKey>, IMapHashable
    where TElemValue : unmanaged
{
    protected unsafe TArray<TMapElementHashable<TElemKey, TElemValue>>* Elements;
    protected IUnrealMemoryInternal Allocator;
    protected bool OwnsInstance = false;
    private bool Disposed = false;

    public unsafe TMapElementAccessor(TArray<TMapElementHashable<TElemKey, TElemValue>>* _Self, IUnrealMemoryInternal _Allocator, bool _OwnsInstance = false)
    {
        Self = _Self;
        Allocator = _Allocator;
        OwnsInstance = _OwnsInstance;
    }

    internal unsafe TArray<TMapElementHashable<TElemKey, TElemValue>>* Self
    {
        get => Elements;
        set => Elements = value;
    }
    
    internal unsafe TMapElementHashable<TElemKey, TElemValue>* Allocation
    {
        get => Elements->AllocatorInstance;
        set => Elements->AllocatorInstance = value;
    }
    
    internal unsafe int Size
    {
        get => Elements->ArrayNum;
        set => Elements->ArrayNum = value;
    }
    
    internal unsafe int Capacity
    {
        get => Elements->ArrayMax;
        set => Elements->ArrayMax = value;
    }

    internal unsafe TElemValue* this[int Index]
    {
        get => &Allocation[Index].Value;
        set => Allocation[Index].Value = *value;
    }
    
    internal unsafe TElemKey GetKey(int Index) => Allocation[Index].Key;
    internal unsafe TElemKey SetKey(int Index, TElemKey Key) => Allocation[Index].Key = Key;
    internal unsafe int GetNextHashId(int Index) => Allocation[Index].HashNextId;
    internal unsafe int SetNextHashId(int Index, int Value) => Allocation[Index].HashNextId = Value;
    internal unsafe int GetHashIndex(int Index) => Allocation[Index].HashIndex;
    internal unsafe int SetHashIndex(int Index, int Value) => Allocation[Index].HashIndex = Value;
    internal unsafe nint GetAddress(int Index) => (nint)(&Allocation[Index]);
    internal unsafe TMapElementHashable<TElemKey, TElemValue>* GetEntry(int Index) => &Allocation[Index];
    internal unsafe int SizeOf() => sizeof(TMapElementHashable<TElemKey, TElemValue>);
    internal void Clear() => Size = 0;

    #region ENUMERABLE INTERFACE
    public unsafe IEnumerator<Ptr<TElemValue>> GetEnumerator() => new TMapElementAccessorEnumerator<TElemKey, TElemValue>(Self);
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion

    #region DISPOSE INTERFACE

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // Same destructor as TArray
    protected virtual void Dispose(bool disposing)
    {
        if (!Disposed)
        {
            if (disposing) { }
            // Disposed unmanaged resources (for Unreal)
            if (OwnsInstance)
            {
                unsafe
                {
                    if (Allocation != null)
                        Allocator.Free((nint)Allocation);
                    Allocator.Free((nint)Elements);
                }
            }
            Disposed = true;
        }
    }

    ~TMapElementAccessor() => Dispose(false);

    #endregion
}

public class TMapElementAccessorEnumerator<TElemKey, TElemValue> : IEnumerator<Ptr<TElemValue>>
    where TElemKey : unmanaged, IEquatable<TElemKey>, IMapHashable
    where TElemValue : unmanaged
{
    protected int Position = -1;
    protected unsafe TArray<TMapElementHashable<TElemKey, TElemValue>>* Self;
    public unsafe Ptr<TElemValue> Current => new(&Self->AllocatorInstance[Position].Value);

    public unsafe TMapElementAccessorEnumerator(TArray<TMapElementHashable<TElemKey, TElemValue>>* _Self)
    {
        Self = _Self;
    }

    object IEnumerator.Current => Current;

    public void Dispose() { }
    public unsafe bool MoveNext() => ++Position < Self->ArrayNum;
    public void Reset() => Position = -1;
}

/// <summary>
/// <para>
/// Maps a particular key to a particular value. This structure provides O(1) performance for finding objects. This requires that the
/// type defined in <c>TElemKey</c> is <c>IMapHashable</c> (GetTypeHash in Unreal)
/// </para>
/// <para>
/// WARNING: Removal methods such as <c>Clear()</c> and <c>Remove()</c> do not invoke the destructor for the inner object!
/// The user has to manually invoke the destructor by calling <c>Dispose()</c> on the target object!
/// </para>
/// </summary>
/// <typeparam name="TElemKey">Type used for keys</typeparam>
/// <typeparam name="TElemValue">Type used for values</typeparam>
public unsafe class TMapDictionary<TElemKey, TElemValue> : IDictionary<TElemKey, Ptr<TElemValue>>, IDisposable
    where TElemKey : unmanaged, IEquatable<TElemKey>, IMapHashable
    where TElemValue : unmanaged
{
    /// <summary>
    ///     <c>TMap Structure</c>
    ///     <list type="bullet">
    ///         <listheader>
    ///             <term>TArray</term>
    ///             <description>Stores a sparse list of elements</description>
    ///         </listheader>
    ///         <item>
    ///             <term><c>TArray&lt;TMapElement&lt;TElemKey, TElemValue&gt;&gt;</c> Elements</term>
    ///             <description>0x0</description>
    ///         </item>
    ///     </list>
    ///     <list type="bullet">
    ///         <listheader>
    ///             <term>TSparseArray</term>
    ///             <description>Tracks which parts of the allocation have elements assigned to them</description>
    ///         </listheader>
    ///         <item>
    ///             <term><c>TBitArray</c> AllocationFlags</term>
    ///             <description>0x10</description>
    ///         </item>
    ///         <item>
    ///             <term><c>int</c> FirstFreeIndex</term>
    ///             <description>0x30</description>
    ///         </item>
    ///         <item>
    ///             <term><c>int</c> NumFreeIndices</term>
    ///             <description>0x34</description>
    ///         </item>
    ///         <item>
    ///             <term><c>int</c> FirstHashIndex</term>
    ///             <description>0x38</description>
    ///         </item>
    ///     </list>
    ///     <list type="bullet">
    ///         <listheader>
    ///             <term>TSet</term>
    ///             <description>Hashes</description>
    ///         </listheader>
    ///         <item>
    ///             <term><c>int*</c> Hash</term>
    ///             <description>0x40</description>
    ///         </item>
    ///         <item>
    ///             <term><c>int</c> HashSize</term>
    ///             <description>0x48</description>
    ///         </item>
    ///     </list>
    /// </summary>
    public nint Self { get; }

    protected IUnrealMemoryInternal Allocator;
    protected TMapElementAccessor<TElemKey, TElemValue> Elements;
    protected TBitArrayList BitAllocator { get; }
    protected Action<string>? DebugCallback;
    protected bool OwnsInstance;
    protected bool Disposed = false;

    private TArray<TMapElementHashable<TElemKey, TElemValue>>* ElementsRaw
    {
        get => (TArray<TMapElementHashable<TElemKey, TElemValue>>*)Self;
    }

    private byte* BitAllocatorRaw => (byte*)(Self + MapConstants.SIZE_OF_ARRAY);

    private int* FirstFreeIndexPtr => (int*)(Self + MapConstants.SIZE_OF_ARRAY + MapConstants.SIZE_OF_BIT_ALLOCATOR);
    
    private int FirstFreeIndex
    {
        get => *FirstFreeIndexPtr;
        set => *FirstFreeIndexPtr = value;
    }

    private int* NumFreeIndicesPtr => (int*)(Self + MapConstants.SIZE_OF_ARRAY + MapConstants.SIZE_OF_BIT_ALLOCATOR + sizeof(int));
    
    private int NumFreeIndices
    {
        get => *NumFreeIndicesPtr;
        set => *NumFreeIndicesPtr = value;
    }
    
    private int* FirstHashIndexPtr => (int*)(Self + MapConstants.SIZE_OF_ARRAY + MapConstants.SIZE_OF_BIT_ALLOCATOR + sizeof(nint));

    private int FirstHashIndex
    {
        get => *FirstHashIndexPtr;
        set => *FirstHashIndexPtr = value;
    }

    private int** HashesPtr
    {
        get => (int**)(Self + MapConstants.SIZE_OF_ARRAY + MapConstants.SIZE_OF_BIT_ALLOCATOR + MapConstants.SIZE_OF_FREE_LIST);
    }
    private int* Hashes
    {
        get => *HashesPtr;
        set => *HashesPtr = value;
    }

    private int* HashSizePtr => (int*)(Self + MapConstants.SIZE_OF_ARRAY + MapConstants.SIZE_OF_BIT_ALLOCATOR + MapConstants.SIZE_OF_FREE_LIST + sizeof(nint));

    private int HashSize
    {
        get => *HashSizePtr;
        set => *HashSizePtr = value;
    }

    // 0x50
    private static int SizeOf => MapConstants.SIZE_OF_ARRAY + MapConstants.SIZE_OF_BIT_ALLOCATOR + MapConstants.SIZE_OF_FREE_LIST + sizeof(nint) + sizeof(nint);

    /// <summary>
    /// Wraps a <c>TMapDictionary</c> around an existing <c>TMap</c> created in C++
    /// </summary>
    /// <param name="_Self">Pointer to an existing <c>TMap</c></param>
    /// <param name="_Allocator">The Unreal allocator, used for methods that modify the <c>TMap</c></param>
    public TMapDictionary(TMap<TElemKey, TElemValue>* _Self, IUnrealMemoryInternal _Allocator, Action<string>? _DebugCallback = null)
    {
        Self = (nint)_Self;
        Allocator = _Allocator;
        Elements = new(ElementsRaw, Allocator);
        OwnsInstance = false;
        BitAllocator = new(BitAllocatorRaw, Allocator);
        DebugCallback = _DebugCallback;
    }

    /// <summary>
    /// Allocate a new <c>TMap</c> owned by this <c>TMapDictionary</c> instance. It will be garbage collected once
    /// taken out of scope.
    /// </summary>
    /// <param name="_Allocator">The Unreal allocator, used for methods that modify the <c>TMap</c></param>
    public TMapDictionary(IUnrealMemoryInternal _Allocator)
    {
        Self = _Allocator.MallocZeroed(SizeOf);
        Allocator = _Allocator;
        Elements = new(ElementsRaw, Allocator);
        OwnsInstance = true;
        BitAllocator = new(BitAllocatorRaw, Allocator);
        // Sets ArrayMax for BitAllocator to number of inline bits (128)
        BitAllocator.Clear();
    }

    private TElemValue* TryGetLinear(TElemKey key)
    {
        if (Elements.Size == 0) return null;
        for (var i = 0; i < Elements.Size; i++)
        {
            if (Elements.GetKey(i).Equals(key))
            {
                return Elements[i];
            }
        }
        return null;
    }

    private TElemValue* TryGetByHash(TElemKey key)
    {
        TElemValue* value = null;
        // Hash alloc doesn't exist for single element maps,
        // so fallback to linear search
        if (Hashes == null) return TryGetLinear(key);
        var elementTarget = Hashes[key.GetTypeHash() & (HashSize - 1)];
        while (elementTarget != MapConstants.INVALID_HASH_ID)
        {
            if (Elements.GetKey(elementTarget).Equals(key))
            {
                value = Elements[elementTarget];
                break;
            }
            elementTarget = Elements.GetNextHashId(elementTarget);
        }
        return value;
    }
    private int GetBucketListTail(int HashIndex)
    {
        int currentIndex = Hashes[HashIndex];
        while (true)
        {
            if (Elements.GetNextHashId(currentIndex) == MapConstants.INVALID_HASH_ID) break;
            currentIndex = Elements.GetNextHashId(currentIndex);
        }
        return currentIndex;
    }

    private void Rehash(int NewSize)
    {
        
        int* NewHashAlloc = (int*)Allocator.Malloc(sizeof(int) * NewSize);
        NativeMemory.Fill(NewHashAlloc, (nuint)(NewSize * sizeof(int)), 0xff);
        if (Hashes != null) Allocator.Free((nint)Hashes);
        Hashes = NewHashAlloc;
        for (int i = 0; i < Elements.Size; i++)
        {
            var newHashIndex = (int)Elements.GetKey(i).GetTypeHash() & (NewSize - 1);
            Elements.SetHashIndex(i, newHashIndex);
            if (Hashes[newHashIndex] == MapConstants.INVALID_HASH_ID) Hashes[newHashIndex] = i;
            else Elements.SetNextHashId(GetBucketListTail(newHashIndex), i);
            Elements.SetNextHashId(i, MapConstants.INVALID_HASH_ID);
        }
        HashSize = NewSize;
    }

    private void ResizeTo(int NewSize)
    {

        nint NewElementAlloc = Allocator.Malloc(NewSize * Elements.SizeOf());
        if (Elements.Allocation != null)
        {
            NativeMemory.Copy((byte*)Elements.Allocation, (byte*)NewElementAlloc, (nuint)(Elements.Size * Elements.SizeOf()));
            Allocator.Free((nint)Elements.Allocation);
        }
        Elements.Allocation = (TMapElementHashable<TElemKey, TElemValue>*)NewElementAlloc;
        Elements.Capacity = NewSize;
    }

    int CalculateNewArraySize() => (Elements.Allocation != null) ? Elements.Capacity * 2 : MapConstants.DEFAULT_ARRAY_SIZE;
    /// <summary>
    /// Relinquishes ownership of the TMap so that C#'s garbage collector doesn't delete this.
    /// </summary>
    public void Leak() => OwnsInstance = false;

    bool InBounds(int index) => index >= 0 && index < Elements.Size;

    /// <summary>
    /// Returns if it should be faster to clear the hash by going through elements instead of resetting the whole bucket lists
    /// </summary>
    bool ShouldClearByElements() => Elements.Size < (HashSize / 4);

    #region DICTIONARY INTERFACE

    public ICollection<TElemKey> Keys
    {
        get
        {
            ICollection<TElemKey> Keys = new List<TElemKey>();
            for (var i = 0; i < Elements.Size; i++)
                if (BitAllocator[i])
                    Keys.Add(Elements.GetKey(i));
            return Keys;
        }
    }

    public ICollection<Ptr<TElemValue>> Values
    {
        get
        {
            ICollection<Ptr<TElemValue>> Values = new List<Ptr<TElemValue>>();
            for (var i = 0; i < Elements.Size; i++)
                if (BitAllocator[i])
                    Values.Add(new(Elements[i]));
            return Values;
        }
    }

    public int Count => Elements.Size;

    public bool IsReadOnly => false;

    public Ptr<TElemValue> this[TElemKey key] 
    {
        get => new(TryGetByHash(key));
        set => *TryGetByHash(key) = *value.Value;
    }

    public void Add(TElemKey key, Ptr<TElemValue> value)
    {
        if (ContainsKey(key)) return; // Don't allow duplicate keys
        if (Hashes == null && Elements.Size + 1 >= MapConstants.MIN_SIZE_FOR_HASH_LIST) Rehash(MapConstants.HASH_INITIAL_SIZE);
        else if (Hashes != null && Elements.Size == HashSize) Rehash(HashSize * 2);
        if (Elements.Size == Elements.Capacity) ResizeTo(CalculateNewArraySize());
        // Get hash index for new key
        if (Hashes != null)
        {
            var hashIndex = (int)(key.GetTypeHash() & (HashSize - 1));
            if (Hashes[hashIndex] == MapConstants.INVALID_HASH_ID) Hashes[hashIndex] = Elements.Size;
            else Elements.SetNextHashId(GetBucketListTail(hashIndex), Elements.Size);
            Elements.SetNextHashId(Elements.Size, MapConstants.INVALID_HASH_ID);
            Elements.SetHashIndex(Elements.Size, hashIndex);
        }
        else
        {
            Elements.SetNextHashId(Elements.Size, Elements.Size - 1);
            Elements.SetHashIndex(Elements.Size, 0);
            FirstHashIndex = Elements.Size;
            HashSize = 1;

        }
        // Add a new element to the array
        Elements.SetKey(Elements.Size, key);
        Elements[Elements.Size] = value.Value;
        // Update the bit allocator
        BitAllocator.Add(true);
        Elements.Size++;
    }

    public void AddValue(TElemKey key, TElemValue value) => Add(key, new(&value));

    public bool ContainsKey(TElemKey key) => Keys.Contains(key);

    public bool Remove(TElemKey key)
    {
        // Find the target element in the map
        TMapElementHashable<TElemKey, TElemValue>* Previous = null;
        TMapElementHashable<TElemKey, TElemValue>* TargetEntry = null;

        var ElementTarget = Hashes[key.GetTypeHash() & (HashSize - 1)];
        while (ElementTarget != MapConstants.INVALID_HASH_ID)
        {
            if (Elements.GetKey(ElementTarget).Equals(key))
            {
                TargetEntry = Elements.GetEntry(ElementTarget);
                break;
            }
            Previous = Elements.GetEntry(ElementTarget);
            ElementTarget = Elements.GetNextHashId(ElementTarget);
        }
        if (TargetEntry == null) // Key does not exist
        {
            return false;
        }
        if (Previous != null) // Remove from the element linked list
        {
            Previous->HashNextId = TargetEntry->HashNextId;
        } else // Removing first element in linked list chain, replace head index
        {
            Hashes[key.GetTypeHash() & (HashSize - 1)] = TargetEntry->HashNextId;
        }
        // Remove element from the elements array - RemoveAt from TSparseArray
        //FirstFreeIndex = ElementTarget;
        //NumFreeIndices++;
        BitAllocator[ElementTarget] = false;
        return true;
    }

    public bool TryGetValue(TElemKey key, out Ptr<TElemValue> value)
    {
        Ptr<TElemValue> GotValue = new(TryGetByHash(key));
        value = GotValue.Value != null ? GotValue : new(null);
        return GotValue.Value != null;
    }

    public void Add(KeyValuePair<TElemKey, Ptr<TElemValue>> item) => Add(item.Key, item.Value);

    public void Clear()
    {
        // TSet.Empty() from UE
        // Reset element buckets to invalid
        for (int i = 0; i < Elements.Size; i++)
        {
            Hashes[i] = MapConstants.INVALID_HASH_ID;
        }
        Elements.Clear();
        BitAllocator.Clear();
    }

    public bool Contains(KeyValuePair<TElemKey, Ptr<TElemValue>> item) => this[item.Key] == item.Value;

    public void CopyTo(KeyValuePair<TElemKey, Ptr<TElemValue>>[] array, int arrayIndex)
    {
        if (!InBounds(arrayIndex))
        {
            return;
        }
        for (var i = 0; i < Elements.Size - arrayIndex; i++)
        {
            array[i] = new(Elements.GetKey(i), new(Elements[i]));
        }
    }

    public bool Remove(KeyValuePair<TElemKey, Ptr<TElemValue>> item) => Remove(item.Key);

    public IEnumerator<KeyValuePair<TElemKey, Ptr<TElemValue>>> GetEnumerator() => new TMapAccessorEnumerator<TElemKey, TElemValue>(Elements.Self);

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
            if (disposing) // Dispose managed resources
            {
                Elements.Dispose();
                BitAllocator.Dispose();
            }
            // Disposed unmanaged resources (for Unreal)
            if (OwnsInstance)
            {
                if (Hashes != null)
                    Allocator.Free((nint)Hashes);
                Allocator.Free(Self);
            }
            Disposed = true;
        }
    }

    ~TMapDictionary() => Dispose(false);

    #endregion
}

public class TMapAccessorEnumerator<TElemKey, TElemValue> : IEnumerator<KeyValuePair<TElemKey, Ptr<TElemValue>>>
    where TElemKey : unmanaged, IEquatable<TElemKey>, IMapHashable
    where TElemValue : unmanaged
{
    protected int Position = -1;
    protected unsafe TArray<TMapElementHashable<TElemKey, TElemValue>>* Self;

    public unsafe TMapAccessorEnumerator(TArray<TMapElementHashable<TElemKey, TElemValue>>* _Self)
    {
        Self = _Self;
    }

    public unsafe KeyValuePair<TElemKey, Ptr<TElemValue>> Current => new(Self->AllocatorInstance[Position].Key, new(&Self->AllocatorInstance[Position].Value));

    object IEnumerator.Current => Current;

    public void Dispose() { }
    public unsafe bool MoveNext() => ++Position < Self->ArrayNum;
    public void Reset() => Position = -1;
}

/// INTERNAL USE
/// Used in cases where the value type is not known at compile time, such as when processing Object XML.
/// The only operations supported are ones required to work with Object XML.
public class TMapDynamicElementAccessor<TElemKey> : IDisposable
    where TElemKey : unmanaged, IEquatable<TElemKey>, IMapHashable
{
    protected unsafe TArray<byte>* Elements;
    private Type ValueType;
    protected IUnrealMemoryInternal Allocator;
    protected bool OwnsInstance = false;
    private bool Disposed = false;
    
    public unsafe TMapDynamicElementAccessor(TArray<byte>* _Self, Type _ValueType, IUnrealMemoryInternal _Allocator, bool _OwnsInstance = false)
    {
        Self = _Self;
        ValueType = _ValueType;
        Allocator = _Allocator;
        OwnsInstance = _OwnsInstance;
    }
    
    internal unsafe TArray<byte>* Self
    {
        get => Elements;
        set => Elements = value;
    }
    
    internal unsafe byte* Allocation
    {
        get => Elements->AllocatorInstance;
        set => Elements->AllocatorInstance = value;
    }
    
    internal unsafe int Size
    {
        get => Elements->ArrayNum;
        set => Elements->ArrayNum = value;
    }
    
    internal unsafe int Capacity
    {
        get => Elements->ArrayMax;
        set => Elements->ArrayMax = value;
    }

    internal int ValueSize => Marshal.SizeOf(ValueType);

    internal unsafe nint this[int Index]
    {
        get => (nint)Allocation + (Index * ValueSize);
    }

    // TMapElement layout:
    // public KeyType Key;
    // public ValueType Value;
    // public int HashNextId;
    // public int HashIndex;
    
    internal unsafe int SizeOf() => sizeof(TElemKey) + ValueSize + 2 * sizeof(int);
    private int GetKeyOffset(int Index) => Index * SizeOf();
    private unsafe int GetValueOffset(int Index) => GetKeyOffset(Index) + sizeof(TElemKey);
    private int GetNextHashIdOffset(int Index) => GetKeyOffset(Index + 1) - 2 * sizeof(int);
    private int GetHashIndexOffset(int Index) => GetKeyOffset(Index + 1) - sizeof(int);
    
    internal unsafe TElemKey GetKey(int Index) => *(TElemKey*)(Allocation + GetKeyOffset(Index));
    internal unsafe TElemKey SetKey(int Index, TElemKey Key) => *(TElemKey*)(Allocation + GetKeyOffset(Index)) = Key;
    internal unsafe int GetNextHashId(int Index) => *(int*)(Allocation + GetNextHashIdOffset(Index));
    internal unsafe int SetNextHashId(int Index, int Value) => *(int*)(Allocation + GetNextHashIdOffset(Index)) = Value;
    internal unsafe int GetHashIndex(int Index) => *(int*)(Allocation + GetHashIndexOffset(Index));
    internal unsafe int SetHashIndex(int Index, int Value) => *(int*)(Allocation + GetHashIndexOffset(Index)) = Value;
    internal unsafe nint GetAddress(int Index) => (nint)(Allocation + GetValueOffset(Index));
    
    #region DISPOSE INTERFACE

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // Same destructor as TArray
    protected virtual void Dispose(bool disposing)
    {
        if (!Disposed)
        {
            if (disposing) { }
            // Disposed unmanaged resources (for Unreal)
            if (OwnsInstance)
            {
                unsafe
                {
                    if (Allocation != null)
                    {
                        Allocator.Free((nint)Allocation);
                    }
                    Allocator.Free((nint)Elements);
                }
            }
            Disposed = true;
        }
    }

    ~TMapDynamicElementAccessor() => Dispose(false);

    #endregion
}

/// INTERNAL USE
/// Used in cases where the value type is not known at compile time, such as when processing Object XML.
/// The only operations supported are ones required to work with Object XML.
public unsafe class TMapDynamicDictionary<TElemKey> : IDisposable
    where TElemKey : unmanaged, IEquatable<TElemKey>, IMapHashable
{
    public nint Self { get; private set; }

    protected IUnrealMemoryInternal Allocator;
    protected TMapDynamicElementAccessor<TElemKey> Elements;
    private Type ValueType;
    protected TBitArrayList BitAllocator;
    protected bool OwnsInstance;
    protected bool Disposed = false;
    
    private TArray<byte>* ElementsRaw => (TArray<byte>*)Self;
    
    private byte* BitAllocatorRaw => (byte*)(Self + MapConstants.SIZE_OF_ARRAY);

    private int* FirstFreeIndexPtr => (int*)(Self + MapConstants.SIZE_OF_ARRAY + MapConstants.SIZE_OF_BIT_ALLOCATOR);
    
    private int FirstFreeIndex
    {
        get => *FirstFreeIndexPtr;
        set => *FirstFreeIndexPtr = value;
    }

    private int* NumFreeIndicesPtr => (int*)(Self + MapConstants.SIZE_OF_ARRAY + MapConstants.SIZE_OF_BIT_ALLOCATOR + sizeof(int));
    
    private int NumFreeIndices
    {
        get => *NumFreeIndicesPtr;
        set => *NumFreeIndicesPtr = value;
    }
    
    private int* FirstHashIndexPtr => (int*)(Self + MapConstants.SIZE_OF_ARRAY + MapConstants.SIZE_OF_BIT_ALLOCATOR + sizeof(nint));

    private int FirstHashIndex
    {
        get => *FirstHashIndexPtr;
        set => *FirstHashIndexPtr = value;
    }

    private int** HashesPtr
    {
        get => (int**)(Self + MapConstants.SIZE_OF_ARRAY + MapConstants.SIZE_OF_BIT_ALLOCATOR + MapConstants.SIZE_OF_FREE_LIST);
    }
    private int* Hashes
    {
        get => *HashesPtr;
        set => *HashesPtr = value;
    }

    private int* HashSizePtr => (int*)(Self + MapConstants.SIZE_OF_ARRAY + MapConstants.SIZE_OF_BIT_ALLOCATOR + MapConstants.SIZE_OF_FREE_LIST + sizeof(nint));

    private int HashSize
    {
        get => *HashSizePtr;
        set => *HashSizePtr = value;
    }

    // 0x50
    private static int SizeOf => MapConstants.SIZE_OF_ARRAY + MapConstants.SIZE_OF_BIT_ALLOCATOR + MapConstants.SIZE_OF_FREE_LIST + sizeof(nint) + sizeof(nint);
    
    /// <summary>
    /// Wraps a <c>TMapDictionary</c> around an existing <c>TMap</c> created in C++
    /// </summary>
    /// <param name="_Self">Pointer to an existing <c>TMap</c></param>
    /// <param name="_Allocator">The Unreal allocator, used for methods that modify the <c>TMap</c></param>
    public TMapDynamicDictionary(TMap<TElemKey, byte>* _Self, Type _ValueType, IUnrealMemoryInternal _Allocator)
    {
        Self = (nint)_Self;
        ValueType = _ValueType;
        Allocator = _Allocator;
        Elements = new(ElementsRaw, ValueType, Allocator);
        OwnsInstance = false;
        BitAllocator = new(BitAllocatorRaw, Allocator);
    }
   
    // ValueType*
    private nint TryGetLinear(TElemKey key)
    {
        if (Elements.Size == 0) return nint.Zero;
        for (var i = 0; i < Elements.Size; i++)
            if (Elements.GetKey(i).Equals(key))
                return Elements.GetAddress(i);
        return nint.Zero;
    }

    private nint TryGetByHash(TElemKey key)
    {
        var value = nint.Zero;
        // Hash alloc doesn't exist for single element maps,
        // so fallback to linear search
        if (Hashes == null) return TryGetLinear(key);
        var elementTarget = Hashes[key.GetTypeHash() & (HashSize - 1)];
        while (elementTarget != MapConstants.INVALID_HASH_ID)
        {
            if (Elements.GetKey(elementTarget).Equals(key))
            {
                value = Elements.GetAddress(elementTarget);
                break;
            }
            elementTarget = Elements.GetNextHashId(elementTarget);
        }
        return value;
    }
    private int GetBucketListTail(int HashIndex)
    {
        var currentIndex = Hashes[HashIndex];
        while (true)
        {
            if (Elements.GetNextHashId(currentIndex) == MapConstants.INVALID_HASH_ID) break;
            currentIndex = Elements.GetNextHashId(currentIndex);
        }
        return currentIndex;
    }
    
    public bool TryGetValue(TElemKey key, out nint pValue)
    {
        pValue = new(TryGetByHash(key));
        return pValue != nint.Zero;
    }

    public void AddIndirect(TElemKey key, nint pNewItem)
    {
        if (ContainsKey(key)) return; // Don't allow duplicate keys
        if (Hashes == null && Elements.Size + 1 >= MapConstants.MIN_SIZE_FOR_HASH_LIST) Rehash(MapConstants.HASH_INITIAL_SIZE);
        else if (Hashes != null && Elements.Size == HashSize) Rehash(HashSize * 2);
        if (Elements.Size == Elements.Capacity) ResizeTo(CalculateNewArraySize());
        // Get hash index for new key
        if (Hashes != null)
        {
            var hashIndex = (int)(key.GetTypeHash() & (HashSize - 1));
            if (Hashes[hashIndex] == MapConstants.INVALID_HASH_ID) Hashes[hashIndex] = Elements.Size;
            else Elements.SetNextHashId(GetBucketListTail(hashIndex), Elements.Size);
            Elements.SetNextHashId(Elements.Size, MapConstants.INVALID_HASH_ID);
            Elements.SetHashIndex(Elements.Size, hashIndex);
        }
        else
        {
            Elements.SetNextHashId(Elements.Size, Elements.Size - 1);
            Elements.SetHashIndex(Elements.Size, 0);
            FirstHashIndex = Elements.Size;
            HashSize = 1;
        }
        // Add a new element to the array
        Elements.SetKey(Elements.Size, key);
        NativeMemory.Copy((void*)pNewItem, (void*)Elements.GetAddress(Elements.Size), (nuint)Elements.ValueSize);
        // Update the bit allocator
        BitAllocator.Add(true);
        Elements.Size++;
    }
    
    public ICollection<TElemKey> Keys
    {
        get
        {
            ICollection<TElemKey> Keys = new List<TElemKey>();
            for (var i = 0; i < Elements.Size; i++)
            {
                Keys.Add(Elements.GetKey(i));
            }
            return Keys;
        }
    }
    
    public bool ContainsKey(TElemKey key) => Keys.Contains(key);
    
    public int Count => Elements.Size;

    private void Rehash(int NewSize)
    {
        
        var NewHashAlloc = (int*)Allocator.Malloc(sizeof(int) * NewSize);
        NativeMemory.Fill(NewHashAlloc, (nuint)(NewSize * sizeof(int)), 0xff);
        if (Hashes != null) Allocator.Free((nint)Hashes);
        Hashes = NewHashAlloc;
        for (var i = 0; i < Elements.Size; i++)
        {
            var newHashIndex = (int)Elements.GetKey(i).GetTypeHash() & (NewSize - 1);
            Elements.SetHashIndex(i, newHashIndex);
            if (Hashes[newHashIndex] == MapConstants.INVALID_HASH_ID) Hashes[newHashIndex] = i;
            else Elements.SetNextHashId(GetBucketListTail(newHashIndex), i);
            Elements.SetNextHashId(i, MapConstants.INVALID_HASH_ID);
        }
        HashSize = NewSize;
    }

    private void ResizeTo(int NewSize)
    {

        var NewElementAlloc = (byte*)Allocator.Malloc(NewSize * Elements.SizeOf());
        if (Elements.Allocation != null)
        {
            NativeMemory.Copy(Elements.Allocation, NewElementAlloc, (nuint)(Elements.Size * Elements.SizeOf()));
            Allocator.Free((nint)Elements.Allocation);
        }
        Elements.Allocation = NewElementAlloc;
        Elements.Capacity = NewSize;
    }

    int CalculateNewArraySize() => (Elements.Allocation != null) ? Elements.Capacity * 2 : MapConstants.DEFAULT_ARRAY_SIZE;
    /// <summary>
    /// Relinquishes ownership of the TMap so that C#'s garbage collector doesn't delete this.
    /// </summary>
    public void Leak() => OwnsInstance = false;

    bool InBounds(int index) => index >= 0 && index < Elements.Size;
    
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
            if (disposing) // Dispose managed resources
            {
                Elements.Dispose();
                BitAllocator.Dispose();
            }
            // Disposed unmanaged resources (for Unreal)
            if (OwnsInstance)
            {
                if (Hashes != null)
                    Allocator.Free((nint)Hashes);
                Allocator.Free(Self);
            }
            Disposed = true;
        }
    }

    ~TMapDynamicDictionary() => Dispose(false);

    #endregion
}