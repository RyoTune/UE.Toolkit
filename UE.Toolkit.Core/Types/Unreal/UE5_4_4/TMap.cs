// ReSharper disable InconsistentNaming

using System.Collections;
using System.Diagnostics;
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

/*
public unsafe class TMapDictionary<InKeyType, InValueType> : IDictionary<InKeyType, Ptr<InValueType>>, IEnumerator<KeyValuePair<InKeyType, Ptr<InValueType>>>
    where InKeyType : unmanaged
    where InValueType : unmanaged
{
    
    private readonly TMap<InKeyType, InValueType>* _map;

    public TMapDictionary(TMap<InKeyType, InValueType>* map)
    {
        _map = map;

        var keys = new List<InKeyType>();
        var values = new List<Ptr<InValueType>>();
        
        for (int i = 0; i < map->MapNum; i++)
        {
            var item = &map->Elements[i];
            var key = item->Key;
            keys.Add(key);
        }
    }
    
    #region DICTIONARY

    public IEnumerator<KeyValuePair<InKeyType, Ptr<InValueType>>> GetEnumerator() => this;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(KeyValuePair<InKeyType, Ptr<InValueType>> item)
    {
        throw new NotImplementedException();
    }

    public void Clear()
    {
        throw new NotImplementedException();
    }

    public bool Contains(KeyValuePair<InKeyType, Ptr<InValueType>> item)
    {
        throw new NotImplementedException();
    }

    public void CopyTo(KeyValuePair<InKeyType, Ptr<InValueType>>[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public bool Remove(KeyValuePair<InKeyType, Ptr<InValueType>> item)
    {
        throw new NotImplementedException();
    }

    public int Count => _map->MapNum;
    
    public bool IsReadOnly { get; }
    
    public void Add(InKeyType key, Ptr<InValueType> value)
    {
        throw new NotImplementedException();
    }

    public bool ContainsKey(InKeyType key) => this.Any(x => x.Key.Equals(key));

    public bool Remove(InKeyType key)
    {
        throw new NotImplementedException();
    }

    public bool TryGetValue(InKeyType key, out Ptr<InValueType> value)
    {
        if (ContainsKey(key))
        {
            value = this.First(x => x.Key.Equals(key)).Value;
            return true;
        }

        value = default;
        return false;
    }

    public Ptr<InValueType> this[InKeyType key]
    {
        get
        {
            if (ContainsKey(key)) return this.First(x => x.Key.Equals(key)).Value;
            throw new KeyNotFoundException();
        }

        set
        {
            for (var i = 0; i < _map->MapNum; i++)
            {
                var currItem = &_map->Elements[i];
                if (!currItem->Key.Equals(key)) continue;
                
                currItem->Value = value.Value;
                return;
            }
            
            throw new KeyNotFoundException();
        }
    }

    public ICollection<InKeyType> Keys => this.Select(x => x.Key).ToArray();

    public ICollection<Ptr<InValueType>> Values => this.Select(x => x.Value).ToArray();

    #endregion

    #region ENUMERATOR

    private int _position = -1;

    public bool MoveNext()
    {
        if (_map == null) return false;
        if (_map->Elements == null) return false;
        return ++_position < _map->MapNum;
    }

    public void Reset() => _position = -1;

    public KeyValuePair<InKeyType, Ptr<InValueType>> Current
    {
        get
        {
            var curr = _map->Elements[_position];
            return new(curr.Key, new(curr.Value));
        }
    }

    object? IEnumerator.Current => Current;

    public void Dispose() { }

    #endregion
}
*/

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
public unsafe struct TMapElementHashable<KeyType, ValueType>
    where KeyType : unmanaged, IEquatable<KeyType>, IMapHashable
    where ValueType : unmanaged
{
    public KeyType Key;
    public ValueType Value;
    public int HashNextId;
    public int HashIndex;
}

public unsafe struct HashablePtr<T> : IMapHashable, IEquatable<HashablePtr<T>> where T: unmanaged
{
    public Ptr<T> Ptr;
    public HashablePtr(Ptr<T> ptr) { Ptr = ptr; }
    public uint GetTypeHash() // FUN_140904980
    {
        uint iVar4 = (uint)((nint)Ptr.Value >> 4);
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
    public bool Equals(HashablePtr<T> other) => Ptr.Value == other.Ptr.Value;
}
public unsafe struct HashableInt : IMapHashable, IEquatable<HashableInt>
{
    public int Value;
    public HashableInt(int value) { Value = value; }
    public uint GetTypeHash() => (uint)Value;
    public bool Equals(HashableInt other) => other.Value == Value;
    public override string ToString() => Value.ToString();
}

[StructLayout(LayoutKind.Explicit, Size = 0x8)]
public unsafe struct HashableInt8 : IMapHashable, IEquatable<HashableInt8>
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

// Free list for TSet/TMap

public unsafe struct TMapFreeListIndex
{
    public int FirstFreeIndex;
    public int NumFreeIndices;
    public int* FreeIndexList;
}

// Look for edge cases where this isn't the case.
internal static class MapConstants
{
    internal const int SIZE_OF_ARRAY = 0x10;
    internal const int SIZE_OF_BIT_ALLOCATOR = 0x20;
    internal const int SIZE_OF_FREE_LIST = 0x10;
}

public class TMapElementAccessor<TElemKey, TElemValue> : IEnumerable<Ptr<TElemValue>>
    where TElemKey : unmanaged, IEquatable<TElemKey>, IMapHashable
    where TElemValue : unmanaged
{
    protected unsafe TArray<TMapElementHashable<TElemKey, TElemValue>>* Elements;

    public unsafe TMapElementAccessor(TArray<TMapElementHashable<TElemKey, TElemValue>>* _Self)
    {
        Self = _Self;
    }

    public unsafe TArray<TMapElementHashable<TElemKey, TElemValue>>* Self
    {
        get => Elements;
        set => Elements = value;
    }
    public unsafe TMapElementHashable<TElemKey, TElemValue>* Allocation
    {
        get => Elements->AllocatorInstance;
        set => Elements->AllocatorInstance = value;
    }
    public unsafe int Size
    {
        get => Elements->ArrayNum;
        set => Elements->ArrayNum = value;
    }
    public unsafe int Capacity
    {
        get => Elements->ArrayMax;
        set => Elements->ArrayMax = value;
    }

    public unsafe TElemValue* this[int Index]
    {
        get => &Allocation[Index].Value;
        set => Allocation[Index].Value = *value;
    }
    public unsafe TElemKey GetKey(int Index) => Allocation[Index].Key;
    public unsafe TElemKey SetKey(int Index, TElemKey Key) => Allocation[Index].Key = Key;
    public unsafe int GetNextHashId(int Index) => Allocation[Index].HashNextId;
    public unsafe int SetNextHashId(int Index, int Value) => Allocation[Index].HashNextId = Value;
    public unsafe int GetHashIndex(int Index) => Allocation[Index].HashIndex;
    public unsafe int SetHashIndex(int Index, int Value) => Allocation[Index].HashIndex = Value;
    internal unsafe nint GetAddress(int Index) => (nint)(&Allocation[Index]);
    public unsafe int SizeOf() => sizeof(TMapElementHashable<TElemKey, TElemValue>);
    public unsafe IEnumerator<Ptr<TElemValue>> GetEnumerator() => new TMapElementAccessorEnumerator<TElemKey, TElemValue>(Self);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
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
/// Maps a particular key to a particular value. This structure provides O(1) performance for finding objects. This requires that the
/// type defined in <c>TElemKey</c> is <c>IMapHashable</c> (GetTypeHash in Unreal)
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
    ///             <term><c>TMapFreeListIndex*</c> Free List</term>
    ///             <description>0x30</description>
    ///         </item>
    ///         <item>
    ///             <term><c>int</c> FirstFreeIndex</term>
    ///             <description>0x38</description>
    ///         </item>
    ///         <item>
    ///             <term><c>int</c> NumFreeIndices</term>
    ///             <description>0x3c</description>
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
    public unsafe nint Self { get; private set; }

    protected IUnrealMemoryInternal Allocator;
    protected TMapElementAccessor<TElemKey, TElemValue> Elements;
    protected TBitArrayList BitAllocator;
    protected Action<string>? DebugCallback;
    protected bool OwnsInstance;
    protected bool Disposed = false;

    private unsafe TArray<TMapElementHashable<TElemKey, TElemValue>>* ElementsRaw
    {
        get => (TArray<TMapElementHashable<TElemKey, TElemValue>>*)Self;
    }

    private unsafe byte* BitAllocatorRaw
    {
        get => (byte*)(Self + MapConstants.SIZE_OF_ARRAY);
    }

    private unsafe TMapFreeListIndex* FreeList
    {
        get => (TMapFreeListIndex*)(Self + MapConstants.SIZE_OF_ARRAY + MapConstants.SIZE_OF_BIT_ALLOCATOR);
    }

    private unsafe int* FirstFreeIndexPtr
    {
        get => (int*)(Self + MapConstants.SIZE_OF_ARRAY + MapConstants.SIZE_OF_BIT_ALLOCATOR + sizeof(nint));
    }
    private unsafe int FirstFreeIndex
    {
        get => *FirstFreeIndexPtr;
        set => *FirstFreeIndexPtr = value;
    }

    private unsafe int* NumFreeIndicesPtr
    {
        get => (int*)(Self + MapConstants.SIZE_OF_ARRAY + MapConstants.SIZE_OF_BIT_ALLOCATOR + sizeof(nint) + sizeof(int));
    }
    private unsafe int NumFreeIndices
    {
        get => *NumFreeIndicesPtr;
        set => *NumFreeIndicesPtr = value;
    }

    private unsafe int** HashesPtr
    {
        get => (int**)(Self + MapConstants.SIZE_OF_ARRAY + MapConstants.SIZE_OF_BIT_ALLOCATOR + MapConstants.SIZE_OF_FREE_LIST);
    }
    private unsafe int* Hashes
    {
        get => *HashesPtr;
        set => *HashesPtr = value;
    }

    private unsafe int* HashSizePtr
    {
        get => (int*)(Self + MapConstants.SIZE_OF_ARRAY + MapConstants.SIZE_OF_BIT_ALLOCATOR + MapConstants.SIZE_OF_FREE_LIST + sizeof(nint));
    }
    private unsafe int HashSize
    {
        get => *HashSizePtr;
        set => *HashSizePtr = value;
    }

    // 0x50
    private static int SizeOf => MapConstants.SIZE_OF_ARRAY + MapConstants.SIZE_OF_BIT_ALLOCATOR + MapConstants.SIZE_OF_FREE_LIST + sizeof(nint) + sizeof(nint);

    private const int DEFAULT_ARRAY_SIZE = 4;
    private static int MIN_SIZE_FOR_HASH_LIST = 0x4;
    private static int HASH_INITIAL_SIZE = 0x10;

    /// <summary>
    /// Wraps a <c>TArrayList</c> around an existing <c>TArray</c> created in C++
    /// </summary>
    /// <param name="_Self">Pointer to an existing <c>TArray</c></param>
    /// <param name="_Allocator">The Unreal allocator, used for methods that modify the <c>TArray</c></param>
    public unsafe TMapDictionary(TMap<TElemKey, TElemValue>* _Self, IUnrealMemoryInternal _Allocator, Action<string>? _DebugCallback = null)
    {
        Self = (nint)_Self;
        Elements = new(ElementsRaw);
        Allocator = _Allocator;
        OwnsInstance = false;
        BitAllocator = new(BitAllocatorRaw, Allocator);
        DebugCallback = _DebugCallback;
    }

    private unsafe TElemValue* TryGetLinear(TElemKey key)
    {
        if (Elements.Size == 0) return null;
        for (int i = 0; i < Elements.Size; i++)
        {
            if (Elements.GetKey(i).Equals(key))
            {
                return Elements[i];
            }
        }
        return null;
    }

    private unsafe TElemValue* TryGetByHash(TElemKey key)
    {
        TElemValue* value = null;
        // Hash alloc doesn't exist for single element maps,
        // so fallback to linear search
        if (Hashes == null) return TryGetLinear(key);
        var elementTarget = Hashes[key.GetTypeHash() & (HashSize - 1)];
        while (elementTarget != -1)
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
    private unsafe int GetBucketListTail(int HashIndex)
    {
        int currentIndex = Hashes[HashIndex];
        while (true)
        {
            if (Elements.GetNextHashId(currentIndex) == -1) break;
            currentIndex = Elements.GetNextHashId(currentIndex);
        }
        return currentIndex;
    }

    private unsafe void Rehash(int NewSize)
    {
        
        int* NewHashAlloc = (int*)Allocator.Malloc(sizeof(int) * NewSize);
        NativeMemory.Fill(NewHashAlloc, (nuint)(NewSize * sizeof(int)), 0xff);
        if (Hashes != null) Allocator.Free((nint)Hashes);
        Hashes = NewHashAlloc;
        for (int i = 0; i < Elements.Size; i++)
        {
            var newHashIndex = (int)Elements.GetKey(i).GetTypeHash() & (NewSize - 1);
            Elements.SetHashIndex(i, newHashIndex);
            if (Hashes[newHashIndex] == -1) Hashes[newHashIndex] = i;
            else Elements.SetNextHashId(GetBucketListTail(newHashIndex), i);
            Elements.SetNextHashId(i, -1);
        }
        HashSize = NewSize;
    }

    private unsafe void ResizeTo(int NewSize)
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

    int CalculateNewArraySize() => (Elements.Allocation != null) ? Elements.Capacity * 2 : DEFAULT_ARRAY_SIZE;

    public void Leak() => OwnsInstance = false;

    #region DICTIONARY INTERFACE

    public ICollection<TElemKey> Keys
    {
        get
        {
            ICollection<TElemKey> Keys = new List<TElemKey>();
            for (int i = 0; i < Elements.Size; i++)
                Keys.Add(Elements.GetKey(i));
            return Keys;
        }
    }

    public ICollection<Ptr<TElemValue>> Values
    {
        get
        {
            ICollection<Ptr<TElemValue>> Values = new List<Ptr<TElemValue>>();
            for (int i = 0; i < Elements.Size; i++)
                Values.Add(new(Elements[i]));
            return Values;
        }
    }

    public int Count => Elements.Size;

    public bool IsReadOnly => false;

    public unsafe Ptr<TElemValue> this[TElemKey key] 
    {
        get => new Ptr<TElemValue>(TryGetByHash(key));
        set => *TryGetByHash(key) = *value.Value;
    }

    public void Add(TElemKey key, Ptr<TElemValue> value)
    {
        if (ContainsKey(key)) return; // Don't allow duplicate keys
        if (Hashes == null && Elements.Size + 1 >= MIN_SIZE_FOR_HASH_LIST) Rehash(HASH_INITIAL_SIZE);
        else if (Hashes != null && Elements.Size == HashSize) Rehash(HashSize * 2);
        if (Elements.Size == Elements.Capacity) ResizeTo(CalculateNewArraySize());
        // Get hash index for new key
        if (Hashes != null)
        {
            var hashIndex = (int)(key.GetTypeHash() & (HashSize - 1));
            if (Hashes[hashIndex] == -1) Hashes[hashIndex] = Elements.Size;
            else Elements.SetNextHashId(GetBucketListTail(hashIndex), Elements.Size);
            Elements.SetNextHashId(Elements.Size, -1);
            Elements.SetHashIndex(Elements.Size, hashIndex);
        }
        else
        {
            Elements.SetNextHashId(Elements.Size, Elements.Size - 1);
            Elements.SetHashIndex(Elements.Size, 0);
        }
        // Add a new element to the array
        Elements.SetKey(Elements.Size, key);
        Elements[Elements.Size] = value.Value;
        // Update the bit allocator
        BitAllocator.Add(true);
        Elements.Size++;
    }

    public bool ContainsKey(TElemKey key) => Keys.Contains(key);

    public bool Remove(TElemKey key)
    {
        throw new NotImplementedException();
    }

    public bool TryGetValue(TElemKey key, [MaybeNullWhen(false)] out Ptr<TElemValue> value)
    {
        Ptr<TElemValue> GotValue = new(TryGetByHash(key));
        value = GotValue.Value != null ? GotValue : new(null);
        return GotValue.Value != null;
    }

    public void Add(KeyValuePair<TElemKey, Ptr<TElemValue>> item) => Add(item.Key, item.Value);

    public void Clear()
    {
        throw new NotImplementedException();
    }

    public bool Contains(KeyValuePair<TElemKey, Ptr<TElemValue>> item) => this[item.Key] == item.Value;

    public void CopyTo(KeyValuePair<TElemKey, Ptr<TElemValue>>[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public bool Remove(KeyValuePair<TElemKey, Ptr<TElemValue>> item)
    {
        throw new NotImplementedException();
    }

    public unsafe IEnumerator<KeyValuePair<TElemKey, Ptr<TElemValue>>> GetEnumerator() => new TMapAccessorEnumerator<TElemKey, TElemValue>(Elements.Self);

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