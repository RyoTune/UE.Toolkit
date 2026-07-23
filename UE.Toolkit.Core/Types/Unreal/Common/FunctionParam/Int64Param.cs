using UE.Toolkit.Core.Types.Interfaces;

namespace UE.Toolkit.Core.Types.Unreal.Common.FunctionParam;

public class Int64Param(Ptr<long> rvalue, IUnrealMemoryInternal? memory = null)
    : FunctionParamCopyable<long>(rvalue, "Int64Property", memory);