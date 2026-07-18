namespace UE.Toolkit.Core.Types.Unreal.Common;

public interface ISoftObjectPath
{
    nint Ptr { get; }
    
    void SetAssetPath(string Value);
    int GetSizeOf();
}