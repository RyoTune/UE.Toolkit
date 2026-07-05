using UE.Toolkit.Core.Types.Unreal.Factories;
using UE.Toolkit.Core.Types.Unreal.Factories.UE5_7_4;
using UE.Toolkit.Interfaces;
using UE.Toolkit.Reloaded.Reflection;
using UE.Toolkit.Reloaded.Reflection.UE5_7_4;
using UE.Toolkit.Reloaded.Unreal;

namespace UE.Toolkit.Reloaded.Common.GameConfigs.Games;

public class UE5_7_4: UE5_4_4_ClairObscur
{
    public override string Id => "UE5_7_4";
    public override IUnrealFactory Factory { get; } = new UnrealFactory();
    public override IUnrealMemory Memory { get; } = new UnrealMemory();
    public override IPropertyFlagsBuilder FlagsBuilder { get; } = new PropertyFlagsBuilder();

    public override BasePropertyFactory PropertyFactory(IUnrealClasses classes)
        => new PropertyFactory(Factory, Memory, classes, FlagsBuilder);

    public override BaseTypeFactory TypeFactory(IUnrealClasses classes)
        => new TypeFactory(Factory, Memory, classes, FlagsBuilder);
    
    public override Type GetFText() => typeof(UE.Toolkit.Core.Types.Unreal.UE5_4_4.FText);

    public override unsafe int GetFTextSize() => sizeof(UE.Toolkit.Core.Types.Unreal.UE5_4_4.FText);
}