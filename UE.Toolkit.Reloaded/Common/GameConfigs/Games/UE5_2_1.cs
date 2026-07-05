using UE.Toolkit.Core.Types.Unreal.Factories;
using UE.Toolkit.Core.Types.Unreal.Factories.UE5_2_1;
using UE.Toolkit.Interfaces;
using UE.Toolkit.Reloaded.Reflection;
using UE.Toolkit.Reloaded.Reflection.UE5_2_1;
using UE.Toolkit.Reloaded.Unreal;

namespace UE.Toolkit.Reloaded.Common.GameConfigs.Games;

public class UE5_2_1: UE5_4_4_ClairObscur
{
    public override string Id => "UE5_2_1";
    public override IUnrealFactory Factory { get; } = new UnrealFactory();
    public override IUnrealMemory Memory { get; } = new UnrealMemory();
    public override IPropertyFlagsBuilder FlagsBuilder { get; } = new PropertyFlagsBuilder();

    public override BasePropertyFactory PropertyFactory(IUnrealClasses classes)
        => new PropertyFactory(Factory, Memory, classes, FlagsBuilder);

    public override BaseTypeFactory TypeFactory(IUnrealClasses classes)
        => new TypeFactory(Factory, Memory, classes, FlagsBuilder);
    
    public override Type GetFText() => typeof(UE.Toolkit.Core.Types.Unreal.UE4_27_2.FText);

    public override unsafe int GetFTextSize() => sizeof(UE.Toolkit.Core.Types.Unreal.UE4_27_2.FText);
}