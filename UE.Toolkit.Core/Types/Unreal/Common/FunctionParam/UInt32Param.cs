using UE.Toolkit.Core.Types.Interfaces;

namespace UE.Toolkit.Core.Types.Unreal.Common.FunctionParam;

public class UInt32Param(Ptr<uint> rvalue, IUnrealMemoryInternal? memory = null) 
    : FunctionParamCopyable<uint>(rvalue, "UInt32Property", memory);