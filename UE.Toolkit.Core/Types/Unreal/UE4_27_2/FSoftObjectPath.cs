using System.Runtime.InteropServices;
using UE.Toolkit.Core.Types.Unreal.Common;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Core.Types.Unreal.UE4_27_2;

[StructLayout(LayoutKind.Sequential)]
public struct FSoftObjectPath
{
    /** Asset path, patch to a top level object in a package. This is /package/path.assetname */
    public FName AssetPathName;

    /** Optional FString for subobject within an asset. This is the sub path after the : */
    public FString SubPathString;
}

public class SoftObjectPath(Ptr<FSoftObjectPath> inner) : ISoftObjectPath
{
    private Ptr<FSoftObjectPath> Inner { get; } = inner;

    public unsafe nint Ptr => (nint)Inner.Value;
    
    public unsafe void SetAssetPath(string Value) => Inner.Value->AssetPathName = new(Value);
    
    public unsafe int GetSizeOf() => sizeof(FSoftObjectPath);
}