using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UE.Toolkit.Core.Types.Interfaces;

public static class MemoryConstants
{
    /// <summary>
    /// Blocks >= 16 bytes are aligned to 16 bytes, otherwise aligned to 8 bytes
    /// </summary>
    public const int DEFAULT_ALIGNMENT = 0;
}

public interface IUnrealMemoryInternal
{
    /// <summary>
    /// Allocates a block of memory with the global <c>FMemory</c>.
    /// </summary>
    /// <param name="count">Number of bytes to allocate.</param>
    /// <param name="alignment">Alignment of allocation.</param>
    /// <returns>Pointer to the allocated memory.</returns>
    /// <remarks>Should only be used after Unreal has finished loading.</remarks>
    nint Malloc(nint count, int alignment = MemoryConstants.DEFAULT_ALIGNMENT);

    /// <summary>
    /// Deallocates a block of memory allocated with the global <c>FMemory</c>.
    /// </summary>
    /// <param name="original">Pointer to the memory block to free.</param>
    /// <remarks>Should only be used after Unreal has finished loading.</remarks>
    void Free(nint original);

    /// <summary>
    /// Reallocates a block of memory with a new size using the global <c>FMemory</c>.
    /// </summary>
    /// <param name="ptr">Old pointer to move data from.</param>
    /// <param name="count">Number of bytes to allocate.</param>
    /// <param name="alignment">Alignment of allocation.</param>
    /// <returns>Pointer to the reallocated memory.</returns>
    /// <remarks>Should only be used after Unreal has finished loading.</remarks>
    nint Realloc(nint ptr, nint count, int alignment = MemoryConstants.DEFAULT_ALIGNMENT);

    /// <summary>
    /// Queries the global <c>FMemory</c> to get the size of an allocation created using it.
    /// </summary>
    /// <param name="ptr">Old pointer to move data from.</param>
    /// <param name="size">Size of the allocation, if the function succeeds</param>
    /// <returns>Indicates if the pointer is to a memory block allocated by <c>FMemory</c></returns>
    /// <remarks>Should only be used after Unreal has finished loading.</remarks>
    bool GetAllocSize(nint ptr, ref nint size);

    /// <summary>
    /// Allocates and clears a block of memory with the global <c>FMemory</c>.
    /// </summary>
    /// <param name="count">Number of bytes to allocate.</param>
    /// <param name="alignment">Alignment of allocation.</param>
    /// <returns>Pointer to the allocated memory.</returns>
    /// <remarks>Should only be used after Unreal has finished loading.</remarks>
    nint MallocZeroed(nint count, int alignment = MemoryConstants.DEFAULT_ALIGNMENT);
    /// <summary>
    /// For some allocators this will return the actual size that should be requested to eliminate internal fragmentation.
    /// </summary>
    /// <param name="self">Instance of GMalloc</param>
    /// <param name="count">Minimum size of allocation</param>
    /// <param name="alignment">Alignment of allocation</param>
    /// <returns></returns>
    nint QuantizeSize(nint count, int alignment = MemoryConstants.DEFAULT_ALIGNMENT);
}