using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;

public interface IUObject : IPtr
{
    nint VTable { get; }
    
    EObjectFlags ObjectFlags { get; }
    
    int InternalIndex { get; }
    
    IUClass ClassPrivate { get; }
    
    FName NamePrivate { get; }
    
    IUObject? OuterPrivate { get; }

    bool IsChildOf(string type);

    bool IsA(string type) => IsChildOf(type);

    bool IsChildOf<T>() => IsChildOf(typeof(T).Name);

    bool IsA<T>() => IsChildOf<T>();

    IUObject GetOutermost();

    string GetNativeName();

    string GetPathName();

    // IUObject GetWorld();
}