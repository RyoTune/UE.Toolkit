using UE.Toolkit.Core.Types.Unreal.UE5_4_4;

namespace UE.Toolkit.Core.Types.Unreal.Factories.Interfaces;

public interface IUFunction : IUStruct
{
    EFunctionFlags FunctionFlags { get; }
    int ParamCount { get; }
    int ParamSize { get; }
    int ReturnValueOffset { get; }

    int GetTotalParameterSize();
    
    nint FunctionPtr { get; }
}