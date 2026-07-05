// ReSharper disable InvalidXmlDocComment

using System.Runtime.InteropServices;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Core.Types.Unreal.UE5_7_4;

[StructLayout(LayoutKind.Sequential, Size = 0x18)]
public struct FNameData
{
    public unsafe FName* TaggedNames;
    public unsafe int* TaggedValues;
    public int NumValues;
}

[StructLayout(LayoutKind.Sequential, Size = 0x70)]
public struct UEnum
{
    public UField Super;
    public FString CppType;
    public FNameData NameData;
    // public TArray<TPair<FName, long>> Names;

    /* How the enum was originally defined. */
    //ECppForm CppForm;

    /* Enum flags. */
    //EEnumFlags EnumFlags;

    /* pointer to function used to look up the enum's display name. Currently only assigned for UEnums generated for nativized blueprints */
    //FEnumDisplayNameFn EnumDisplayNameFn;

    /* Package name this enum was in when its names were being added to the primary list */
    //FName EnumPackage;

    /* lock to be taken when accessing AllEnumNames */
    //static FRWLock AllEnumNamesLock;

    /* global list of all value names used by all enums in memory, used for property text import */
    //static TMap<FName, TMap<FName, UEnum*> > AllEnumNames;
}