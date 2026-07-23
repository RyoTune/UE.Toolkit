using UE.Toolkit.Core.Types.Interfaces;

namespace UE.Toolkit.Core.Types.Unreal.Common.FunctionParam;

public class Int16Param(Ptr<short> rvalue, IUnrealMemoryInternal? memory = null)
    : FunctionParamCopyable<short>(rvalue, "Int16Property", memory);