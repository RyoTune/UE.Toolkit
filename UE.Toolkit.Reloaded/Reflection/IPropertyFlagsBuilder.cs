using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using EPropertyFlags = UE.Toolkit.Core.Types.Unreal.UE5_4_4.EPropertyFlags;

namespace UE.Toolkit.Reloaded.Reflection;

public interface IPropertyFlagsBuilder
{
    EPropertyFlags CreatePropertyFlags(PropertyVisibility Visibility, PropertyBuilderFlags InFlags);
    void SkipInitializationForNextProperty();
}