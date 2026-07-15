using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;

namespace UE.Toolkit.Interfaces;

public interface ITypeReflection
{
    /// <summary>
    /// Get the FText type used by the currently running version of the engine.
    /// UE 5.4 and later have a significantly different FText implemenet compared to earlier versions.
    /// </summary>
    /// <returns>Type information for the FText.</returns>
    Type GetFText();
    /// <summary>
    /// Get the size of the FText type used by the currently running version of the engine.
    /// UE 5.4 and later have a significantly different FText implemenet compared to earlier versions.
    /// </summary>
    /// <returns>Size of FText.</returns>
    int GetFTextSize();
    /// <summary>
    /// Gets the property type name, such as <c>byte</c> for <c>ByteProperty</c>.
    /// </summary>
    /// <param name="prop">Property to get type name from.</param>
    /// <returns>C++/C# property type name.</returns>
    string GetPropertyTypeName(IFProperty prop);
}