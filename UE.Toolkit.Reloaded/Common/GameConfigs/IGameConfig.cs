// ReSharper disable InconsistentNaming

using UE.Toolkit.Core.Types.Unreal.Factories;
using UE.Toolkit.Interfaces;
using UE.Toolkit.Reloaded.Reflection;

namespace UE.Toolkit.Reloaded.Common.GameConfigs;

public interface IGameConfig
{
    string Id { get; }
    IUnrealFactory Factory { get; }
    IUnrealMemory Memory { get; }
    IPropertyFlagsBuilder FlagsBuilder { get; }
    BasePropertyFactory PropertyFactory(IUnrealClasses classes);
    BaseTypeFactory TypeFactory(IUnrealClasses classes);
    Type GetFText();
    int GetFTextSize();
}