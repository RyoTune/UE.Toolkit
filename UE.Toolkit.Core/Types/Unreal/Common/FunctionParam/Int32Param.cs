using UE.Toolkit.Core.Types.Interfaces;

namespace UE.Toolkit.Core.Types.Unreal.Common.FunctionParam;

public class Int32Param(Ptr<int> rvalue, IUnrealMemoryInternal? memory = null) 
    : FunctionParamCopyable<int>(rvalue, "Int32Property", memory);