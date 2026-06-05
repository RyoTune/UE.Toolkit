using UE.Toolkit.Core.Types.Interfaces;
using UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;

namespace UE.Toolkit.Core.Types.Unreal.Factories;

/// <summary>
/// Factory for creating engine-agnostic wrappers around engine-specific implementations.
/// </summary>
public interface IUnrealFactory
{
    T Cast<T>(IPtr obj);
    nint SizeOf<T>();
    IUnrealMemoryInternal? Memory { get; set; }
    
    IFProperty CreateFProperty(nint ptr);
    IFBoolProperty CreateFBoolProperty(nint ptr);
    IFByteProperty CreateFByteProperty(nint ptr);
    IFEnumProperty CreateFEnumProperty(nint ptr);
    IFObjectProperty CreateFObjectProperty(nint ptr);
    IFSoftClassProperty CreateFSoftClassProperty(nint ptr);
    IFClassProperty CreateFClassProperty(nint ptr);
    IFStructProperty CreateFStructProperty(nint ptr);
    IFMapProperty CreateFMapProperty(nint ptr);
    IFInterfaceProperty CreateFInterfaceProperty(nint ptr);
    IFArrayProperty CreateFArrayProperty(nint ptr);
    IFSetProperty CreateFSetProperty(nint ptr);
    IFOptionalProperty CreateFOptionalProperty(nint ptr);
    IFDelegateProperty CreateFDelegateProperty(nint ptr);
    
    IUObjectArray CreateUObjectArray(nint ptr);
    IUObject CreateUObject(nint ptr);
    IUClass CreateUClass(nint ptr);
    IUScriptStruct CreateUScriptStruct(nint ptr);
    IUEnum CreateUEnum(nint ptr);
    IUField CreateUField(nint ptr);
    IUStruct CreateUStruct(nint ptr);
    IUUserDefinedEnum CreateUUserDefinedEnum(nint ptr);
    // IUPackage CreateUPackage(nint ptr); 
    IUFunction CreateUFunction(nint ptr);
    
    IFFieldClass CreateFFieldClass(nint ptr);
    IFField CreateFField(nint ptr);

    IFStructParams CreateFStructParams(nint ptr);
    IFPropertyParams CreateFPropertyParams(nint ptr);
    IFGenericPropertyParams CreateFGenericPropertyParams(nint ptr);

    IFWorldContext CreateFWorldContext(nint ptr);
    IUEngine CreateUEngine(nint ptr);
    IUGameInstance CreateUGameInstance(nint ptr);
    
    IFStaticConstructObjectParameters CreateFStaticConstructObjectParameters();
    IFActorSpawnParameters CreateFActorSpawnParameters();
}