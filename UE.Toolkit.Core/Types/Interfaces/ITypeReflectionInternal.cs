namespace UE.Toolkit.Core.Types.Interfaces;

public interface ITypeReflectionInternal
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
}