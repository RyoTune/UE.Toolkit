using System.Runtime.InteropServices;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Core.Types.Unreal.UE5_7_4;

[StructLayout(LayoutKind.Sequential, Size = 0x208)]
public unsafe struct UClass
{
    public UStruct Super;
    public nint ClassConstructor;
    public nint ClassVTableHelperCtorCaller;
    public nint CppClassStaticFunctions;
    public int ClassUnique;
    public int FirstOwnedClassRep;
    public bool bCooked;
    public bool bLayoutChanging;
    public EClassFlags ClassFlags;
    public EClassCastFlags ClassCastFlags;
    public UClass* ClassWithin;
    //public UObjectBase* ClassGeneratedBy; // WITH_EDITORONLY_DATA
    //public FField* PropertiesPendingDestruction; // WITH_EDITORONLY_DATA
    public FName ClassConfigName;
    public TArray<FRepRecord> ClassReps;
    public TArray<UField> NetFields;
    public UObjectBase* ClassDefaultObject;
    public nint SparseClassData;
    public UScriptStruct* SparseClassDataStruct;
    public bool bNeedsDynamicSubobjectInstancing;
    public TMap<FName, nint> FuncMap;
    
    public readonly UClass* GetSuperClass() => (UClass*)Super.SuperStruct;
}