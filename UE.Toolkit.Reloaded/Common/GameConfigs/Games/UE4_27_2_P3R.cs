using UE.Toolkit.Core.Types.Unreal.Common;
using UE.Toolkit.Core.Types.Unreal.Factories;
using UE.Toolkit.Core.Types.Unreal.Factories.UE4_27_2;
using UE.Toolkit.Interfaces;
using UE.Toolkit.Reloaded.Reflection;
using UE.Toolkit.Reloaded.Reflection.UE4_27_2;
using UE.Toolkit.Reloaded.Unreal;

// ReSharper disable InconsistentNaming

namespace UE.Toolkit.Reloaded.Common.GameConfigs.Games;

public class UE4_27_2_P3R : UE5_4_4_ClairObscur
{
    public override string Id => "P3R";
    public override IUnrealFactory Factory { get; } = new UnrealFactory();
    public override IUnrealMemory Memory { get; } = new UnrealMemory();
    public override IPropertyFlagsBuilder FlagsBuilder { get; } = new PropertyFlagsBuilder();

    public override BasePropertyFactory PropertyFactory(IUnrealClasses classes)
        => new PropertyFactory(Factory, Memory, classes, FlagsBuilder);

    public override BaseTypeFactory TypeFactory(IUnrealClasses classes)
        => new TypeFactory(Factory, Memory, classes, FlagsBuilder);
    
    public override Type GetFText() => typeof(UE.Toolkit.Core.Types.Unreal.UE4_27_2.FText);

    public override unsafe int GetFTextSize() => sizeof(UE.Toolkit.Core.Types.Unreal.UE4_27_2.FText);
    
    public override unsafe ISoftObjectPath IntoSoftObjectPath(nint ptr)
        => new UE.Toolkit.Core.Types.Unreal.UE4_27_2.SoftObjectPath(
            new((UE.Toolkit.Core.Types.Unreal.UE4_27_2.FSoftObjectPath*)ptr));
}