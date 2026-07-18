using System.Runtime.InteropServices;
using UE.Toolkit.Core.Types.Unreal.Common;

namespace UE.Toolkit.Core.Types.Unreal.UE5_4_4;

[StructLayout(LayoutKind.Sequential)]
public struct FSoftObjectPath
{
    /** Asset path, patch to a top level object in a package. This is /package/path.assetname */
    public FTopLevelAssetPath AssetPath;

    /** Optional FString for subobject within an asset. This is the sub path after the : */
    public FString SubPathString;
}

public class SoftObjectPath(Ptr<FSoftObjectPath> inner) : ISoftObjectPath
{
    private Ptr<FSoftObjectPath> Inner { get; } = inner;
    
    public unsafe nint Ptr => (nint)Inner.Value;
    
    public void SetAssetPath(string Value)
    {
        var Parts = Value.Split(".");
        unsafe
        {
            Inner.Value->AssetPath.PackageName = new(Parts[0]);
            Inner.Value->AssetPath.AssetName = new(Parts[1]);   
        }
    }

    public unsafe int GetSizeOf() => sizeof(FSoftObjectPath);
}