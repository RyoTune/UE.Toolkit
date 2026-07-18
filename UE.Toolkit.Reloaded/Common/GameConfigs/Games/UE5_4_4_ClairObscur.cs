using UE.Toolkit.Core.Types.Unreal.Common;
using UE.Toolkit.Core.Types.Unreal.Factories;
using UE.Toolkit.Core.Types.Unreal.Factories.UE5_4_4;
using UE.Toolkit.Interfaces;
using UE.Toolkit.Reloaded.Reflection;
using UE.Toolkit.Reloaded.Reflection.UE5_4_4;
using UE.Toolkit.Reloaded.Unreal;

namespace UE.Toolkit.Reloaded.Common.GameConfigs.Games;

// ReSharper disable once InconsistentNaming
public class UE5_4_4_ClairObscur : IGameConfig
{
    public virtual string Id => "Clair Obscur";
    public virtual IUnrealFactory Factory { get; } = new UnrealFactory();
    public virtual IUnrealMemory Memory { get; } = new UnrealMemory();
    public virtual IPropertyFlagsBuilder FlagsBuilder { get; } = new PropertyFlagsBuilder();

    public virtual BasePropertyFactory PropertyFactory(IUnrealClasses classes)
        => new PropertyFactory(Factory, Memory, classes, FlagsBuilder);

    public virtual BaseTypeFactory TypeFactory(IUnrealClasses classes)
        => new TypeFactory(Factory, Memory, classes, FlagsBuilder);

    public virtual Type GetFText() => typeof(UE.Toolkit.Core.Types.Unreal.UE5_4_4.FText);

    public virtual unsafe int GetFTextSize() => sizeof(UE.Toolkit.Core.Types.Unreal.UE5_4_4.FText);

    public virtual unsafe ISoftObjectPath IntoSoftObjectPath(nint ptr)
        => new UE.Toolkit.Core.Types.Unreal.UE5_4_4.SoftObjectPath(
            new((UE.Toolkit.Core.Types.Unreal.UE5_4_4.FSoftObjectPath*)ptr));
}