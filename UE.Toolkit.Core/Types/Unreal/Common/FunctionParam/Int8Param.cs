using UE.Toolkit.Core.Types.Interfaces;

namespace UE.Toolkit.Core.Types.Unreal.Common.FunctionParam;

public class Int8Param(Ptr<byte> rvalue, IUnrealMemoryInternal? memory = null)
    : FunctionParamCopyable<byte>(rvalue, "Int8Property", memory);