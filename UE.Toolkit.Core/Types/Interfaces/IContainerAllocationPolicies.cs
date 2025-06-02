using System.Numerics;

namespace UE.Toolkit.Core.Types.Interfaces;

// Advanced settings for how allocations should be controlled for Unreal data structures
// This is likely unnecessary, but it may come useful in the future.
// Runtime/Core/Containers/ContainerAllocationPolicies
public interface ContainerAllocationPolicy<TType, TSize>
    where TType : unmanaged where TSize : INumber<TSize>
{
    public bool HasAllocation();
    public TSize GetInitialCapacity();
    public nint GetAllocatedSize(TSize NumAllocatedElements, nint NumBytesPerElement);
    public unsafe TType* GetAllocation();
    public unsafe void SetAllocation(TType* Alloc);
}

/// <summary>
/// Elements are always allocated indirectly
/// Implemented in Unreal Engine using TAlignedHeapAllocator and TSizedHeapAllocator, which other than
/// differences in how they assign alignments for allocations act identically.
/// </summary>
public unsafe abstract class HeapAllocator<TType, TSize> : ContainerAllocationPolicy<TType, TSize>
    where TType : unmanaged where TSize : INumber<TSize>
{
    protected TType* Data;
    public abstract TSize GetInitialCapacity();
    public abstract nint GetAllocatedSize(TSize NumAllocatedElements, nint NumBytesPerElement);
    public bool HasAllocation() => Data != null;
    public unsafe TType* GetAllocation() => Data;
    public unsafe void SetAllocation(TType* Alloc) => Data = Alloc;
}

public unsafe class HeapAllocatorInt32<TType> : HeapAllocator<TType, int>
    where TType : unmanaged
{
    public override int GetInitialCapacity() => 0;
    public override nint GetAllocatedSize(int NumAllocatedElements, nint NumBytesPerElement) => NumAllocatedElements * NumBytesPerElement;
}

/// <summary>
/// Allocates up to a specified number of elements in the same allocation as the container.
/// Any allocation needed beyond this causes all data to be moved into an indirect allocation
/// This is the default allocation policy for TBitArray
/// </summary>
public unsafe abstract class InlineAllocator<TType, TSize> : ContainerAllocationPolicy<TType, TSize>
    where TType : unmanaged where TSize : INumber<TSize>
{
    protected TType* Heap;
    protected TSize NumInlineElements;

    public bool HasAllocation() => Heap != null;
    public TSize GetInitialCapacity() => NumInlineElements;
    public abstract nint GetAllocatedSize(TSize NumAllocatedElements, nint NumBytesPerElement);
    public unsafe TType* GetAllocation() => null;
    public unsafe void SetAllocation(TType* Alloc) { }
}

public unsafe class InlineAllocatorInt32<TType> : InlineAllocator<TType, int>
    where TType : unmanaged
{
    public override nint GetAllocatedSize(int NumAllocatedElements, nint NumBytesPerElement)
    {
        if (NumAllocatedElements > NumInlineElements)
        {
            return (NumAllocatedElements - NumInlineElements) * NumBytesPerElement;
        } else
        {
            return 0;
        }
    }
}


/// <summary>
/// Allocates up to a specified number of elements in the same allocation as the container like the
/// inline allocator. However, it won't provide secondary storage when the inline storage is filled
/// </summary>
public unsafe class FixedAllocator<TType, TSize> : ContainerAllocationPolicy<TType, TSize>
    where TType : unmanaged where TSize : INumber<TSize>
{
    protected TSize NumInlineElements;
    public TSize GetInitialCapacity() => NumInlineElements;

    public bool HasAllocation() => false;
    public nint GetAllocatedSize(TSize NumAllocatedElements, nint NumBytesPerElement) => 0;
    public unsafe TType* GetAllocation() => null;
    public unsafe void SetAllocation(TType* Alloc) { }
}