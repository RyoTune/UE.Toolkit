using UE.Toolkit.Core.Types.Unreal.Common;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;

namespace UE.Toolkit.Interfaces;

/// <summary>
/// API for obtaining special type reflection info which cannot be obtained entirely through the runtime reflection system.
/// </summary>
public interface ITypeReflection
{
    
    #region FText
    
    /// <summary>
    /// Get the FText type used by the currently running version of the engine.
    /// UE 5.4 and later have a significantly different FText implement compared to earlier versions.
    /// </summary>
    /// <returns>Type information for the FText.</returns>
    Type GetFText();
    /// <summary>
    /// Get the size of the FText type used by the currently running version of the engine.
    /// UE 5.4 and later have a significantly different FText implement compared to earlier versions.
    /// </summary>
    /// <returns>Size of FText.</returns>
    int GetFTextSize();
    
    #endregion
    
    #region FSoftObjectPath
    
    /// <summary>
    /// Converts an untyped pointer into a managed SoftObjectPath for the currently running version of the engine.
    /// UE 5.1 and later use a FTopLevelAssetPath for the AssetPath instead of an FName
    /// </summary>
    /// <returns>Size of FText.</returns>
    ISoftObjectPath IntoSoftObjectPath(nint ptr);
    
    #endregion
    
    #region Type Names
    
    /// <summary>
    /// Gets the property type name, such as <c>byte</c> for <c>ByteProperty</c>.
    /// </summary>
    /// <param name="prop">Property to get type name from.</param>
    /// <returns>C++/C# property type name.</returns>
    string GetPropertyTypeName(IFProperty prop);
    
    #endregion
}