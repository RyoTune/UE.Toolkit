using System.Runtime.InteropServices;

// ReSharper disable GrammarMistakeInComment
// ReSharper disable InconsistentNaming
// ReSharper disable InvalidXmlDocComment

namespace UE.Toolkit.Core.Types.Unreal.UE5_4_4;

[StructLayout(LayoutKind.Sequential)]
public struct FUObjectArray
{	
    /** First index into objects array taken into account for GC.							*/
    public int ObjFirstGCIndex;
    
    /** Index pointing to last object created in range disregarded for GC.					*/
    public int ObjLastNonGCIndex;
    
    /** Maximum number of objects in the disregard for GC Pool */
    public int MaxObjectsNotConsideredByGC;

    /** If true this is the intial load and we should load objects int the disregarded for GC range.	*/
    public bool OpenForDisregardForGC;
    
    /** Array of all live objects.											*/
    public FChunkedFixedUObjectArray ObjObjects;
}

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct FUObjectArray_Pack4
{	
    /** First index into objects array taken into account for GC.							*/
    public int ObjFirstGCIndex;
    
    /** Index pointing to last object created in range disregarded for GC.					*/
    public int ObjLastNonGCIndex;
    
    /** Maximum number of objects in the disregard for GC Pool */
    public int MaxObjectsNotConsideredByGC;

    /** If true this is the intial load and we should load objects int the disregarded for GC range.	*/
    public bool OpenForDisregardForGC;
    
    /** Array of all live objects.											*/
    public FChunkedFixedUObjectArray_Pack4 ObjObjects;
}

[StructLayout(LayoutKind.Sequential)]
public struct FUObjectArray_UE5_7
{	
    /** First index into objects array taken into account for GC.							*/
    public int ObjFirstGCIndex;
    
    /** Index pointing to last object created in range disregarded for GC.					*/
    public int ObjLastNonGCIndex;
    
    /** Maximum number of objects in the disregard for GC Pool */
    public int MaxObjectsNotConsideredByGC;

    /** If true this is the intial load and we should load objects int the disregarded for GC range.	*/
    public bool OpenForDisregardForGC;
    
    /** Array of all live objects.											*/
    public FChunkedFixedUObjectArray_UE5_7 ObjObjects;
}

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public unsafe struct FChunkedFixedUObjectArray_Pack4
{
    const int NumElementsPerChunk = 64 * 1024;
    
    /** Primary table to chunks of pointers **/
    public FUObjectItem** Objects;
    
    /** If requested, a contiguous memory where all objects are allocated **/
    public FUObjectItem* PreAllocatedObjects;
    
    /** Maximum number of elements **/
    public int MaxElements;
    
    /** Number of elements we currently have **/
    public int NumElements;
    
    /** Maximum number of chunks **/
    public int MaxChunks;
    
    /** Number of chunks we currently have **/
    public int NumChunks;
    
    public readonly FUObjectItem* GetItem(int idx)
    {
        var chunkIndex = idx / NumElementsPerChunk;
        var withinChunkIndex = idx % NumElementsPerChunk;
        var chunk = Objects[chunkIndex];
        return chunk + withinChunkIndex;
    }
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct FChunkedFixedUObjectArray
{
    const int NumElementsPerChunk = 64 * 1024;
    
    /** Primary table to chunks of pointers **/
    public FUObjectItem** Objects;
    
    /** If requested, a contiguous memory where all objects are allocated **/
    public FUObjectItem* PreAllocatedObjects;
    
    /** Maximum number of elements **/
    public int MaxElements;
    
    /** Number of elements we currently have **/
    public int NumElements;
    
    /** Maximum number of chunks **/
    public int MaxChunks;
    
    /** Number of chunks we currently have **/
    public int NumChunks;
    
    public readonly FUObjectItem* GetItem(int idx)
    {
        var chunkIndex = idx / NumElementsPerChunk;
        var withinChunkIndex = idx % NumElementsPerChunk;
        var chunk = Objects[chunkIndex];
        return chunk + withinChunkIndex;
    }
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct FChunkedFixedUObjectArray_UE5_7
{
    const int NumElementsPerChunk = 64 * 1024;
    
    /** Primary table to chunks of pointers **/
    public FUObjectItem_5_7** Objects;
    
    /** If requested, a contiguous memory where all objects are allocated **/
    public FUObjectItem_5_7* PreAllocatedObjects;
    
    /** Maximum number of elements **/
    public int MaxElements;
    
    /** Number of elements we currently have **/
    public int NumElements;
    
    /** Maximum number of chunks **/
    public int MaxChunks;
    
    /** Number of chunks we currently have **/
    public int NumChunks;
    
    public readonly FUObjectItem_5_7* GetItem(int idx)
    {
        var chunkIndex = idx / NumElementsPerChunk;
        var withinChunkIndex = idx % NumElementsPerChunk;
        var chunk = Objects[chunkIndex];
        return chunk + withinChunkIndex;
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 8)] // Packing in source says 4, but 8 in Clair Obscur at least.
public unsafe struct FUObjectItem
{
    // Pointer to the allocated object
    public UObjectBase* Object;
    
    // Internal flags. These can only be changed via Set* and Clear* functions
    public EInternalObjectFlags Flags;
    
    // UObject Owner Cluster Index
    public int ClusterRootIndex;
    
    // Weak Object Pointer Serial number associated with the object
    public int SerialNumber;
}

[StructLayout(LayoutKind.Sequential, Pack = 8)] // Packing in source says 4, but 8 in Clair Obscur at least.
public unsafe struct FUObjectItem_5_7
{
    // int64 FlagsAndRefCount;
    public int RefCount;
    
    // Internal flags. These can only be changed via Set* and Clear* functions
    public EInternalObjectFlags Flags;
    
    // Pointer to the allocated object
    public UObjectBase* Object;
    
    // UObject Owner Cluster Index
    public int ClusterRootIndex;
    
    // Weak Object Pointer Serial number associated with the object
    public int SerialNumber;
}