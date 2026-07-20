using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;

public interface IUClass : IUStruct
{
    IUClass? GetSuperClass();

    IUFunction? GetFunction(string Name);

    IEnumerable<IUFunction> GetFunctions();

    IUObject? ClassDefaultObject { get; }

    nint Constructor { get; }

    EClassFlags ClassFlags { get; }
}