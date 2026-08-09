using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;
using EPropertyFlags = UE.Toolkit.Core.Types.Unreal.UE5_4_4.EPropertyFlags;

namespace UE.Toolkit.Reloaded.Reflection.UE4_27_2;

public class PropertyFlagsBuilder : IPropertyFlagsBuilder
{
    private PropertyBuilderFlags FlagsForNextProperty;

    public void SkipInitializationForNextProperty() => FlagsForNextProperty |= PropertyBuilderFlags.Skip;

    public EPropertyFlags CreatePropertyFlags(PropertyVisibility Visibility, PropertyBuilderFlags InFlags)
    {
        var ListFlags = FlagsForNextProperty != PropertyBuilderFlags.None;
        InFlags |= FlagsForNextProperty;
        FlagsForNextProperty = PropertyBuilderFlags.None;
        var Flags = EPropertyFlags.CPF_Edit | EPropertyFlags.CPF_BlueprintVisible;
        if (InFlags.HasFlag(PropertyBuilderFlags.NoCtor)) Flags |= EPropertyFlags.CPF_ZeroConstructor;
        if (InFlags.HasFlag(PropertyBuilderFlags.Copy)) Flags |= EPropertyFlags.CPF_IsPlainOldData;
        if (InFlags.HasFlag(PropertyBuilderFlags.NoDtor)) Flags |= EPropertyFlags.CPF_NoDestructor;
        if (InFlags.HasFlag(PropertyBuilderFlags.Hash)) Flags |= EPropertyFlags.CPF_HasGetValueTypeHash;
        if (InFlags.HasFlag(PropertyBuilderFlags.Skip)) Flags |= EPropertyFlags.CPF_NonPIEDuplicateTransient | EPropertyFlags.CPF_ContainsInstancedReference;
        Flags |= Visibility switch
        {
            PropertyVisibility.Public => EPropertyFlags.CPF_NativeAccessSpecifierPublic,
            PropertyVisibility.Protected => EPropertyFlags.CPF_NativeAccessSpecifierProtected,
            _ => EPropertyFlags.CPF_NativeAccessSpecifierPrivate,
        };
        if (ListFlags)
        {
            Log.Debug($"CreatePropertyFlags: {Flags}");
        }
        return Flags;
    }
}