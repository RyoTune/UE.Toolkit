using System.Diagnostics.CodeAnalysis;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;

public interface IUEnum : IUField
{
    string CppType { get; }
    
    TArray<TPair<FName, long>> Names { get; }

    bool TryParse(string name, bool ignoreCase, [NotNullWhen(true)] out long? value);
}