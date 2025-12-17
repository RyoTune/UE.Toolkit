using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace UE.Toolkit.Core.Types.Unreal.UE5_4_4;

[StructLayout(LayoutKind.Sequential)]
public struct FPersistentObjectPtr
{
    public FWeakObjectPtr WeakPtr;
    public int TagAtLastTest;
    public Guid ObjectId;
}

[StructLayout(LayoutKind.Sequential)]
public struct TPersistentObjectPtr<TObject> where TObject : unmanaged
{
    public FWeakObjectPtr WeakPtr;
    public int TagAtLastTest;
    public TObject ObjectId;
}